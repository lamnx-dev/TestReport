using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S31HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S31H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S31HReportData.CreateSample();
        var rows = data.CreateRows();
        Check(rows.Count == 11 && rows[0].DuNo == 50_000_000m && rows[1].PhatSinhNo == 2_000_000m && rows[2].SoChungTu == "DC001", "opening and adjustment summary/detail rows");
        Check(rows[4] is { SoChungTu: "KP001", TaiKhoanDoiUng: "511", PhatSinhNo: 100_000_000m, DuNo: null } && rows[5] is { TaiKhoanDoiUng: "112", PhatSinhCo: 70_000_000m }, "voucher fields and opposite-side accounts");
        Check(rows[7] is { PhatSinhNo: 130_000_000m, PhatSinhCo: 70_000_000m, DuNo: null } && rows[8].DuNo == 112_000_000m && rows[^2] is { PhatSinhNo: 130_000_000m, PhatSinhCo: 70_000_000m } && rows[^1].DuNo == 112_000_000m, "separate monthly turnover and month-end balance, annual turnover and closing");
        data.Items.Reverse();
        var original = data.Items.ToArray();
        Check(data.CreateRows()[4].SoChungTu == "KP001" && data.Items.SequenceEqual(original), "posting-date order leaves input unchanged");
        var debit = new S31HPosting { NgayGhiSo = new(2026, 1, 5), TaiKhoanNo = "135311", TaiKhoanCo = "511", SoTien = 20 };
        data = new() { Items = [debit, debit with { TaiKhoanNo = "13532" }, debit with { MaDonVi = "OTHER" },
            debit with { NgayGhiSo = new(2025, 1, 1) }, debit with { NgayGhiSo = new(2026, 2, 1) }] };
        Check(data.CreateRows()[^2].PhatSinhNo == 20, "selected source descendants, reporting unit and date filters");
        data.TaiKhoan = "1353";
        Check(data.CreateRows()[^2].PhatSinhNo == 40, "parent source aggregates matching descendants");
        data = new() { Items = [debit with { TaiKhoanCo = "135312" }] };
        rows = data.CreateRows();
        Check(rows[3].TaiKhoanDoiUng == "135312" && rows[4].TaiKhoanDoiUng == "135311" && rows[^2] is { PhatSinhNo: 20, PhatSinhCo: 20 } && rows[^1].DuNo == 0, "both matching account sides retained without changing net balance");
        data = new() { TuNgay = new(2026, 2, 1), DenNgay = new(2026, 3, 31), NoDauNam = 100, CoDauNam = 20, Items =
        [
            debit, debit with { MaLoai = 16, TaiKhoanNo = "421", TaiKhoanCo = "13531", SoTien = 10 },
            debit with { NgayGhiSo = new(2026, 2, 1), MaLoai = 16, SoTien = 5, SoChungTu = "ADJ" },
            debit with { NgayGhiSo = new(2026, 2, 10), TaiKhoanNo = "112", TaiKhoanCo = "13531", SoTien = 15 }
        ] };
        rows = data.CreateRows();
        Check(rows[0].DuNo == 90 && rows[1].PhatSinhNo == 5 && rows[2].SoChungTu == "ADJ" && rows[^2] is { PhatSinhNo: 20, PhatSinhCo: 15 } && rows[^1].DuNo == 80, "prior balances, adjustment details and year-to-date scopes avoid double counting");
        data = new() { DenNgay = new(2026, 3, 31), Items = [debit, debit with { NgayGhiSo = new(2026, 3, 1), MaLoai = 16, SoTien = 5 }] };
        rows = data.CreateRows();
        Check(rows[6].DuNo == 20 && rows[8].DuNo == 20 && rows[10].DuNo == 25, "late adjustment affects only corresponding month-end and subsequent balances");
        data = new() { CoDauNam = 100, Items = [debit with { SoTien = 25 }] };
        Check(data.CreateRows()[0].DuCo == 100 && data.CreateRows()[^1] is { DuNo: null, DuCo: 75 }, "credit opening and closing balances preserved");
        Check(new S31HReportData().CreateRows().Count == 7 && new S31HReportData { DenNgay = new(2026, 3, 31) }.CreateRows().Count == 11 &&
            new S31HReportData { DenNgay = new(2026, 12, 31) }.CreateRows().Count == 29, "month, quarter and year retain all month-end rows, including empty months");
        foreach (var invalid in new S31HReportData[]
        {
            new() { TaiKhoan = "1352" }, new() { TaiKhoan = "1353x" }, new() { TaiKhoan = "" }, new() { MaDonVi = "" },
            new() { TuNgay = new(2027, 1, 1) }, new() { DenNgay = new(2027, 1, 1) }, new() { Items = null! }, new() { Items = [null!] },
            new() { Items = [debit with { NgayGhiSo = default }] }, new() { Items = [debit with { TaiKhoanNo = "" }] }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid period, source, unit or posting rejected");
        }
        using (var designer = new S31HReport())
        {
            Check(designer.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource, "designer exposes data source");
            Check(Text(designer).Contains("KP002"), "designer preview has data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var reloaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S31HReportData));
            try { reloaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S31HReportData)); }
            Check(reloaded.FindControl("tblBodyCell9", true) is XRTableCell { ExpressionBindings.Count: 2 }, "designer retains all ten columns and bindings");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s31h")!;
        Check(report is S31HReport && report.Landscape, "factory returns landscape S31H");
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        Check(report.Pages.Count == 1, "sample fits one page");
        var content = Text(report);
        Check(content.Contains("PHẢI THU KINH PHÍ ĐƯỢC CẤP") && content.Contains("Nguồn: 13531") && content.Contains("Nguyễn Văn An") && content.Contains("Tháng 01/2026"), "source, period and signatures populated");
        Check(content.Contains("KP002") && content.Contains("112,000,000") && content.Contains("Cộng lũy kế từ đầu năm") && content.Contains("Quyết định cấp kinh phí 01"), "transactions, notes and totals printed");
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text == "Số dư") &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Count(x => x.Text == "Nợ") == 2 &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Count(x => x.Text == "Có") == 2, "all movement and balance column headings printed");
        Check(!content.Contains("CBX") && !content.Contains("mã loại") && !content.Contains("select ma") && !content.Contains("captheodoi"), "analysis notes excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "title appearance applied");
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Số dư cuối kỳ") && x.Font.Bold), "totals remain bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S31H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s31h.png"), new ImageExportOptions
            { Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 130,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1" });
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        using (var empty = new S31HReport(new()))
            Check(!Text(empty).Contains("KP001") && empty.Pages.Count == 1, "empty report never loads sample transactions");
        Check(System.Text.Json.JsonSerializer.Serialize(((OkObjectResult)controller.Catalog()).Value).Contains("s31h"), "catalog exposes S31H to frontend");
        Check(controller.Render("s31h", new()) is FileContentResult { ContentType: "application/pdf", FileDownloadName: "S31H.pdf" }, "API renders S31H");
        data = S31HReportData.CreateSample();
        for (var i = 0; i < 110; i++) data.Items.Add(data.Items[1] with { SoChungTu = $"LONG-{i}", DienGiai = "Phải thu kinh phí " + new string('x', 80) });
        using var longReport = new S31HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1, "long report paginates");
        Check(Bricks(longReport.Pages[1]).Any(x => x.Text == "Số phát sinh") && Bricks(longReport.Pages[1]).Any(x => x.Text == "Số dư"), "headers repeat");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S31H checks passed.");
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
