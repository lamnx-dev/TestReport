using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S36HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S36H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S36HReportData.CreateSample();
        var rows = data.CreateRows();
        Check(rows[0] is { SoLuongPhatSinh: 2, GiaTriPhatSinh: 20_000_000m, GiaTriXuLy: 0 }, "opening quantity and net credit balance");
        Check(rows[1] is { SoLuongPhatSinh: 0, GiaTriPhatSinh: 1_000_000m } && rows[^2] is { SoLuongPhatSinh: 4, GiaTriPhatSinh: 42_000_000m, SoLuongXuLy: 3, GiaTriXuLy: 30_000_000m }, "type 16 adjustment excluded from turnover");
        Check(rows[^1] is { SoLuongPhatSinh: 3, GiaTriPhatSinh: 33_000_000m, SoLuongXuLy: 0, GiaTriXuLy: 0 }, "closing quantity and value reconcile including adjustment");
        Check(rows.Count(x => x.InDam) == 7 && rows.Count == 11, "two monthly totals and annual summaries");
        data.Items.Reverse();
        var original = data.Items.ToArray();
        rows = data.CreateRows();
        Check(rows[3] is { SoChungTu: "KK001", TaiKhoanDoiUng: "211", GiaTriPhatSinh: 30_000_000m, GiaTriXuLy: 0 } && rows[4] is { SoChungTu: "XL001", TaiKhoanDoiUng: "711", GiaTriPhatSinh: 0, GiaTriXuLy: 10_000_000m } && data.Items.SequenceEqual(original), "credit records surplus, debit records settlement, with correct counterparts and no mutation");
        data.SoDuCuoiKy = new() { SoLuong = 4, Co = 40_000_000m, No = 2_000_000m };
        Check(data.CreateRows()[^1] is { SoLuongPhatSinh: 4, GiaTriPhatSinh: 38_000_000m }, "authoritative closing snapshot retained");
        var posting = S36HReportData.CreateSample().Items[1];
        data = new() { Items = [posting, posting with { SoLuong = 0.5m, SoTien = 5_000_000m }] };
        Check(data.CreateRows().Count(x => x.SoChungTu == "KK001") == 2 && data.CreateRows()[^2].SoLuongPhatSinh == 3.5m, "same voucher detail lines remain separate");
        data = new() { Items = [posting with { MaDonVi = "OTHER" }, posting with { CoMaTaiSan = "OTHER" }, posting with { TaiKhoanCo = "1388" }, posting with { NgayGhiSo = new(2025, 1, 1) }, posting with { NgayGhiSo = new(2027, 1, 1) }] };
        Check(data.CreateRows()[^2].GiaTriPhatSinh == 0, "unit, asset, account and year filters");
        data = new() { Items = [posting with { TaiKhoanNo = "3381", NoMaTaiSan = "TS001", SoLuong = 1.5m, SoTien = 12.25m }] };
        Check(data.CreateRows()[^2] is { SoLuongPhatSinh: 1.5m, GiaTriPhatSinh: 12.25m, SoLuongXuLy: 1.5m, GiaTriXuLy: 12.25m } && data.CreateRows()[^1].GiaTriPhatSinh == 0, "both matching sides retain fractional detail and net to zero");
        data.Items = [posting with { TaiKhoanNo = "3381", CoMaTaiSan = "OTHER", NoMaTaiSan = "TS001", MaLoai = 16 }];
        Check(data.CreateRows()[1] is { GiaTriPhatSinh: 0, GiaTriXuLy: 30_000_000m } && data.CreateRows()[^2].GiaTriXuLy == 0, "debit adjustment uses debit asset dimension");
        data = new() { SoDuDauKy = new() { SoLuong = -1, Co = 5, No = 10 } };
        Check(data.CreateRows()[0].GiaTriPhatSinh == -5 && data.CreateRows()[^1].SoLuongPhatSinh == -1, "signed net balances preserved");
        Check(new S36HReportData().CreateRows().Count == 5, "empty year retains opening, adjustment, heading, annual total and closing");
        foreach (var invalid in new S36HReportData[]
        {
            new() { Nam = 0 }, new() { Nam = 10000 }, new() { MaDonVi = "" }, new() { MaTaiSan = "" },
            new() { SoDuDauKy = null! }, new() { Items = null! }, new() { Items = [null!] },
            new() { Items = [posting with { NgayGhiSo = default }] }, new() { Items = [posting with { TaiKhoanCo = "" }] }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid year, selection or posting rejected");
        }
        using (var designer = new S36HReport())
        {
            Check(designer.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource, "designer exposes data source");
            Check(Text(designer).Contains("KK002"), "designer preview has data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var reloaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S36HReportData));
            try { reloaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S36HReportData)); }
            Check(reloaded.FindControl("tblBodyCell9", true) is XRTableCell { ExpressionBindings.Count: 2 }, "designer retains all ten columns and bindings");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s36h")!;
        Check(report is S36HReport && report.Landscape, "factory returns landscape S36H");
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        Check(report.Pages.Count == 1, "sample fits one page");
        var content = Text(report);
        Check(content.Contains("TÀI SẢN THỪA CHỜ XỬ LÝ") && content.Contains("TS001") && content.Contains("3381") && content.Contains("Nguyễn Văn An") && content.Contains("Năm: 2026"), "header and signatures populated");
        Check(content.Contains("KK002") && content.Contains("33,000,000") && content.Contains("Cộng luỹ kế từ đầu năm") && content.Contains("Biên bản kiểm kê 01"), "transactions, notes and totals printed");
        Check(!content.Contains("CBX") && !content.Contains("mã loại") && !content.Contains("sodu"), "analysis notes excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "title appearance applied");
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Số dư cuối kỳ") && x.Font.Bold), "totals remain bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S36H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s36h.png"), new ImageExportOptions
            { Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 130,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1" });
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        using (var empty = new S36HReport(new()))
            Check(!Text(empty).Contains("KK001") && empty.Pages.Count == 1, "empty report never loads sample transactions");
        Check(System.Text.Json.JsonSerializer.Serialize(((OkObjectResult)controller.Catalog()).Value).Contains("s36h"), "catalog exposes S36H to frontend");
        Check(controller.Render("s36h", new()) is FileContentResult { ContentType: "application/pdf", FileDownloadName: "S36H.pdf" }, "API renders S36H");
        data = S36HReportData.CreateSample();
        for (var i = 0; i < 110; i++) data.Items.Add(data.Items[1] with { SoChungTu = $"LONG-{i}", DienGiai = "Tài sản thừa " + new string('x', 80) });
        using var longReport = new S36HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1, "long report paginates");
        Check(Bricks(longReport.Pages[1]).Any(x => x.Text == "Tài sản thừa chờ xử lý phát sinh"), "headers repeat");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S36H checks passed.");
    }

    private static IEnumerable<TextBrick> Bricks(BrickBase brick)
    {
        if (brick is TextBrick text) yield return text;
        System.Collections.IEnumerable? children = brick switch
        { Page p => p.InnerBricks, CompositeBrick c => c.InnerBricks, Brick b => b.Bricks, _ => null };
        if (children is null) yield break;
        foreach (BrickBase child in children)
            foreach (var item in Bricks(child)) yield return item;
    }
}
