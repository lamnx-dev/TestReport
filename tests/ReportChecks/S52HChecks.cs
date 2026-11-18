using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S52HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S52H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        static S52HRow Total(S52HReportData data) => data.CreateRows()[^3];
        var data = S52HReportData.CreateSample();
        Check(Total(data) is { SoLuong: 23, DonGia: null, ThanhTien: 37_000_000m, GiamTru: 2_000_000m }, "quantity and amount totals exclude closing transfers");
        Check(data.CreateRows()[^1] is { GiamTru: 35_000_000m, InDam: true }, "closing transfer summed separately");
        Check(data.CreateRows().Count(x => x.SoChungTu == "DT001") == 2, "individual detail lines retained");
        data.Items.Reverse();
        var original = data.Items.ToArray();
        Check(data.CreateRows()[0] is { SoChungTu: "DT001", SoLuong: 10, DonGia: 1_500_000m, ThanhTien: 15_000_000m }, "posting date order and source detail prices");
        Check(data.Items.SequenceEqual(original), "input order unchanged");
        data.TuNgay = new(2026, 3, 1);
        Check(Total(data) is { SoLuong: 0, ThanhTien: 0, GiamTru: 2_000_000m } && data.CreateRows()[^2].ThanhTien == 37_000_000m, "period and year-to-date totals use different date scopes");
        Check(data.CreateRows()[0] is { SoLuong: null, DonGia: null, TaiKhoanDoiUng: "131" }, "reductions omit quantity and price");
        var revenue = S52HReportData.CreateSample().Items[0];
        var reduction = S52HReportData.CreateSample().Items[2];
        data = new() { Items = [revenue with { MaDonVi = "OTHER" }, revenue with { CoChiTiet1 = "OTHER" }, revenue with { CoChiTiet2 = "OTHER" },
            revenue with { TaiKhoanCo = "5312" }, revenue with { NgayGhiSo = new(2025, 12, 31) }, revenue with { NgayGhiSo = new(2026, 4, 1) },
            reduction with { NoChiTiet1 = "OTHER" }, reduction with { NoChiTiet2 = "OTHER" }] };
        Check(Total(data) is { ThanhTien: 0, GiamTru: 0 }, "unit, debit and credit dimensions, account and date filtering");
        data = new() { Items = [revenue with { ChiTiet = [new() { SoLuong = 2.5m, DonGia = 11.2m, SoTien = 27.9m }] }] };
        Check(Total(data) is { SoLuong: 2.5m, ThanhTien: 27.9m }, "decimal quantity and authoritative detail amount retained");
        data = new() { Items = [revenue with { TaiKhoanNo = "5311", NoChiTiet1 = "DV", NoChiTiet2 = "DV01" }] };
        Check(Total(data) is { ThanhTien: 25_000_000m, GiamTru: 25_000_000m, SoLuong: 15 }, "both matching account sides retained independently");
        var closing = S52HReportData.CreateSample().Items[3];
        data = new() { Items = [closing with { TaiKhoanNo = "53111", TaiKhoanCo = "9111" }, closing with { NgayGhiSo = new(2026, 2, 1) }] , TuNgay = new(2026, 3, 1) };
        Check(Total(data).GiamTru == 0 && data.CreateRows()[^2].GiamTru == 0 && data.CreateRows()[^1].GiamTru == 35_000_000m, "911 descendants excluded from reductions and prior closing excluded");
        Check(new S52HReportData().CreateRows().Count == 3, "empty report preserves totals and transfer row");
        foreach (var invalid in new S52HReportData[]
        {
            new() { TaiKhoan = "511" }, new() { TaiKhoan = "531x" }, new() { TuNgay = new(2025, 1, 1) },
            new() { TuNgay = new(2026, 4, 1) }, new() { MaDonVi = "" }, new() { MaLoaiHoatDong = "" }, new() { MaSanPham = "" },
            new() { Items = null! }, new() { Items = [null!] }, new() { Items = [revenue with { ChiTiet = null! }] },
            new() { Items = [revenue with { ChiTiet = [null!] }] }, new() { Items = [revenue with { NgayGhiSo = default }] }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid selection or posting rejected");
        }
        using (var designer = new S52HReport())
        {
            Check(designer.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource, "designer exposes data source");
            Check(Text(designer).Contains("DT002"), "designer preview has data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var reloaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S52HReportData));
            try { reloaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S52HReportData)); }
            Check(reloaded.FindControl("tblBodyCell8", true) is XRTableCell { ExpressionBindings.Count: 2 }, "designer retains all nine columns and bindings");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s52h")!;
        Check(report is S52HReport && report.Landscape, "factory returns landscape S52H");
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        Check(report.Pages.Count == 1, "sample fits one page");
        var content = Text(report);
        Check(content.Contains("DOANH THU SẢN XUẤT") && content.Contains("DV01") && content.Contains("Hoạt động dịch vụ") && content.Contains("Nguyễn Văn An"), "header and signatures populated");
        Check(content.Contains("DT002") && content.Contains("35,000,000") && content.Contains("Cộng lũy kế từ đầu năm"), "transactions and totals printed");
        Check(!content.Contains("Ng?y") && !content.Contains("id_dkhoan") && !content.Contains("Số dư cuối kỳ"), "report excludes analysis text and unrelated balance rows");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "title appearance applied");
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Cộng số phát sinh") && x.Font.Bold), "totals remain bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S52H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s52h.png"), new ImageExportOptions
            { Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 130,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1" });
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        Check(controller.Render("s52h", new()) is FileContentResult { ContentType: "application/pdf", FileDownloadName: "S52H.pdf" }, "API renders S52H");
        data = S52HReportData.CreateSample();
        for (var i = 0; i < 110; i++) data.Items.Add(data.Items[0] with { SoChungTu = $"LONG-{i}", DienGiai = "Doanh thu dịch vụ " + new string('x', 80) });
        using var longReport = new S52HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1, "long report paginates");
        Check(Bricks(longReport.Pages[1]).Any(x => x.Text == "Doanh thu cung cấp dịch vụ"), "headers repeat");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S52H checks passed.");
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
