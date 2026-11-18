using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S34HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S34H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S34HReportData.CreateSample();
        var rows = data.CreateRows();
        Check(rows.Count == 10 && rows[2].DuNo == 50_000_000m && rows[3].DuNo == 30_000_000m && rows[5].DuNo == 60_000_000m, "sample running balances follow 50 - 20 + 30");
        Check(rows[^2] is { PhatSinhNo: 80_000_000m, PhatSinhCo: 20_000_000m } && rows[^1].DuNo == 60_000_000m && rows[7].DuNo == 60_000_000m, "monthly, annual and closing figures reconcile, including empty month");
        data.Items.Reverse();
        var original = data.Items.ToArray();
        Check(data.CreateRows()[2].SoChungTu == "PT001" && data.Items.SequenceEqual(original), "chronological posting order leaves source unchanged");
        var debit = new S34HPosting { NgayGhiSo = new(2026, 1, 5), TaiKhoanNo = "1311", NoDoiTuong = "A", TaiKhoanCo = "531", SoTien = 20 };
        data = new() { TuNgay = new(2026, 2, 1), NoDauNam = 100, CoDauNam = 20, Items =
        [
            debit, debit with { MaLoai = 16, TaiKhoanNo = "421", TaiKhoanCo = "1311", NoDoiTuong = "", CoDoiTuong = "A", SoTien = 10 },
            debit with { NgayGhiSo = new(2026, 2, 1), MaLoai = 16, SoTien = 5, SoChungTu = "ADJ" },
            debit with { NgayGhiSo = new(2026, 2, 10), TaiKhoanNo = "112", NoDoiTuong = "", TaiKhoanCo = "1311", CoDoiTuong = "A", SoTien = 15 }
        ] };
        rows = data.CreateRows();
        Check(rows[0].DuNo == 90 && rows[1].PhatSinhNo == 5 && rows[2] is { SoChungTu: "ADJ", DuNo: 95 } && rows[3].DuNo == 80, "prior movements included in opening and adjustment detail counted once");
        Check(rows[^2] is { PhatSinhNo: 20, PhatSinhCo: 15 } && rows[^1].DuNo == 80, "adjustments excluded from year-to-date turnover");
        data = new() { Items = [debit, debit with { NgayGhiSo = new(2026, 3, 1), MaLoai = 16, SoTien = 5 }] };
        rows = data.CreateRows();
        Check(rows[3].DuNo == 20 && rows[4].DuNo == 20 && rows[5].DuNo == 20 && rows[6].DuNo == 25,
            "late adjustment does not change earlier running or month-end balances");
        foreach (var account in new[] { "131", "331" })
        {
            data = new() { TaiKhoan = account, Items =
            [
                debit with { TaiKhoanNo = account + "1", TaiKhoanCo = account + "2", CoDoiTuong = "B", SoTien = 60 },
                debit with { TaiKhoanNo = account + "1", NoDoiTuong = "B", TaiKhoanCo = account + "2", CoDoiTuong = "A", SoTien = 20 },
                debit with { TaiKhoanNo = account, MaDonVi = "OTHER" }, debit with { TaiKhoanNo = account, NoDoiTuong = "OTHER" },
                debit with { TaiKhoanNo = account, NgayGhiSo = new(2025, 1, 1) }, debit with { TaiKhoanNo = account, NgayGhiSo = new(2026, 4, 1) }
            ] };
            rows = data.CreateRows();
            Check(rows[^2] is { PhatSinhNo: 60, PhatSinhCo: 20 } && rows[^1].DuNo == 40 && rows[2].TaiKhoanDoiUng == account + "2" && rows[3].TaiKhoanDoiUng == account + "1", "both sides filter selected object, unit, dates and account descendants: " + account);
        }
        data = new() { Items = [debit with { TaiKhoanCo = "1312", CoDoiTuong = "A" }] };
        rows = data.CreateRows();
        Check(rows[2].DuNo is null && rows[3].DuNo == 0 && rows[^2] is { PhatSinhNo: 20, PhatSinhCo: 20 }, "both matching sides retain separate rows and one final balance");
        data = new() { TaiKhoan = "331", CoDauNam = 100, Items = [debit with { TaiKhoanNo = "331", SoTien = 25 }] };
        Check(data.CreateRows()[0].DuCo == 100 && data.CreateRows()[^1] is { DuNo: null, DuCo: 75 }, "supplier credit balances split to credit column");
        Check(new S34HReportData().CreateRows().Count == 7, "empty quarter retains all monthly totals");
        data = new() { DenNgay = new(2026, 12, 31) };
        Check(data.CreateRows().Count == 16, "empty year retains twelve months");
        foreach (var invalid in new S34HReportData[]
        {
            new() { TaiKhoan = "1381" }, new() { TaiKhoan = "131x" }, new() { MaDoiTuong = "" }, new() { MaDonVi = "" },
            new() { TuNgay = new(2027, 1, 1) }, new() { DenNgay = new(2027, 1, 1) }, new() { Items = null! },
            new() { Items = [null!] }, new() { Items = [debit with { NgayGhiSo = default }] }, new() { Items = [debit with { TaiKhoanNo = "" }] }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid period, account, object or posting rejected");
        }
        using (var designer = new S34HReport())
        {
            Check(designer.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource, "designer exposes data source");
            Check(Text(designer).Contains("PT002"), "designer preview has data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var reloaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S34HReportData));
            try { reloaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S34HReportData)); }
            Check(reloaded.FindControl("tblBodyCell8", true) is XRTableCell { ExpressionBindings.Count: 2 }, "designer retains all nine columns and bindings");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s34h")!;
        Check(report is S34HReport && report.Landscape, "factory returns landscape S34H");
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        Check(report.Pages.Count == 1, "sample fits one page");
        var content = Text(report);
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text == "Số dư") &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Count(x => x.Text == "Nợ") == 2 &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Count(x => x.Text == "Có") == 2, "movement and balance column headings are printed");
        Check(content.Contains("THANH TOÁN VỚI KHÁCH HÀNG") && content.Contains("Đối tượng: A - Đơn vị A") && content.Contains("Tài khoản: 131") && content.Contains("Nguyễn Văn An") && content.Contains("Quý 1/2026"), "header and signatures populated");
        Check(content.Contains("PT002") && content.Contains("60,000,000") && content.Contains("Cộng lũy kế từ đầu năm"), "transactions and totals printed");
        Check(!content.Contains("CBX") && !content.Contains("mã loại") && !content.Contains("sodu"), "analysis notes excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "title appearance applied");
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Số dư cuối kỳ") && x.Font.Bold), "totals remain bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S34H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s34h.png"), new ImageExportOptions
            { Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 130,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1" });
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        using (var empty = new S34HReport(new()))
            Check(!Text(empty).Contains("PT001") && empty.Pages.Count == 1, "empty report never loads sample transactions");
        Check(System.Text.Json.JsonSerializer.Serialize(((OkObjectResult)controller.Catalog()).Value).Contains("s34h"), "catalog exposes S34H to frontend");
        Check(controller.Render("s34h", new()) is FileContentResult { ContentType: "application/pdf", FileDownloadName: "S34H.pdf" }, "API renders S34H");
        data = S34HReportData.CreateSample();
        for (var i = 0; i < 110; i++) data.Items.Add(data.Items[0] with { SoChungTu = $"LONG-{i}", DienGiai = "Công nợ khách hàng " + new string('x', 80) });
        using var longReport = new S34HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1, "long report paginates");
        Check(Bricks(longReport.Pages[1]).Any(x => x.Text == "Số phát sinh"), "headers repeat");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S34H checks passed.");
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
