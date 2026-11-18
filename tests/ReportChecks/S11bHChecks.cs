using System.Text;
using System.Text.Json;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S11bHChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S11bH " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S11bHReportData.CreateSample();
        var original = data.Items.ToArray();
        data.Items = [original[3], original[0], original[1], original[2]];
        var input = data.Items.ToArray();
        var rows = data.CreateRows();
        Check(rows[0] is { DienGiai: "Số dư đầu năm", Ton: 20_000_000m }, "annual opening balance follows template");
        Check(rows[2].SoPhieuThu == "PT001" && rows[3].SoPhieuChi == "PC001" && data.Items.SequenceEqual(input), "sort by posting date, retain same day order and source");
        Check(rows[2].Ton == 150_000_000m && rows[3].Ton == 147_500_000m, "running balance includes prior months once");
        var days = rows.Where(x => x.DienGiai == "Cộng số phát sinh ngày").ToArray();
        Check(days.Length == 3 && days[0] is { Thu: 100_000_000m, Chi: 2_500_000m, Ton: 147_500_000m }, "daily totals use posting date rather than voucher date");
        Check(rows[^2] is { Thu: 101_200_000m, Chi: 7_500_000m, Ton: 143_700_000m }, "month totals exclude daily summaries");
        Check(rows[^1] is { Thu: 401_200_000m, Chi: 277_500_000m, Ton: 143_700_000m }, "year to date excludes opening from turnover");
        var empty = new S11bHReportData { SoDuDauNam = 42, LuyKeThuTruocThang = 10, LuyKeChiTruocThang = 5 };
        Check(empty.CreateRows().Count == 4 && empty.CreateRows()[^1].Ton == 47, "empty month retains annual opening and actual balance");
        var january = new S11bHReportData { Thang = 1, SoDuDauNam = 100, Items = [new() { NgayGhiSo = new(2026, 1, 1), Chi = 20 }] };
        Check(january.CreateRows()[^1] is { Thu: 0, Chi: 20, Ton: 80 }, "January totals equal monthly totals");
        foreach (var invalid in new S11bHReportData[]
        {
            new() { Thang = 0 }, new() { Nam = 10000 }, new() { Items = null! }, new() { Items = [null!] },
            new() { Items = [new()] }, new() { Items = [new() { NgayGhiSo = new(2026, 10, 31) }] },
            new() { Items = [new() { NgayGhiSo = new(2025, 11, 1) }] }, new() { Thang = 1, LuyKeThuTruocThang = 1 }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid period or transaction rejected");
        }
        using (var designer = new S11bHReport())
        {
            Check(designer.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource && Text(designer).Contains("PT001"), "designer preview loads sample data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var loaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S11bHReportData));
            try { loaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S11bHReportData)); }
            Check(loaded.FindControl("lblThuQuy", true).Tag?.ToString() == "ThuQuy" &&
                loaded.FindControl("tblBodyCell8", true) is XRTableCell { ExpressionBindings.Count: 2 }, "layout retains signature tags and nine column bindings");
        }
        using (var blank = new S11bHReport(empty))
        {
            var text = Text(blank);
            Check(!text.Contains("PT001") && !text.Contains("THỦ QUỸ") && !text.Contains("Ký, họ tên"), "empty report omits sample and undeclared signatures");
        }
        var undeclared = new S11bHReportData { ThuQuy = "Hidden cashier", KeToanTruong = "Hidden accountant", ThuTruongDonVi = "Hidden director" };
        using (var blank = new S11bHReport(undeclared))
            Check(!Text(blank).Contains("Hidden"), "names hidden without declared signature titles");
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s11bh")!;
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        var content = Text(report);
        Check(report is S11bHReport && report.Landscape && report.Pages.Count == 1, "sample fits one landscape A4 page");
        Check(content.Contains("SỔ QUỸ TIỀN MẶT") && content.Contains("Tháng 11 năm 2026") && content.Contains("S11b-H"), "report identity and month populated");
        Check(content.Contains("PT001") && content.Contains("PC002") && content.Contains("Nguyễn Thị Lan"), "receipt and payment numbers and notes printed");
        Check(content.Contains("THỦ QUỸ") && content.Contains("Nguyễn Văn An") && content.Contains("30 tháng 11 năm 2026"), "configured signature labels, names and date printed");
        Check(!content.Contains("DM_dvql") && !content.Contains("userdate") && !content.Contains("Tài khoản đối ứng"), "analysis instructions and accountant-only column excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16 &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Cộng số lũy kế tháng") && x.Font.Bold), "appearance applies while totals remain bold");
        var body = (XRTable)report.FindControl("tblBody", true);
        Check(body.Rows[0].Cells.Count == 9 && body.Rows[0].Cells.Cast<XRTableCell>().All(x => !x.Borders.HasFlag(BorderSide.Top)) &&
            body.Rows[0].Cells.Cast<XRTableCell>().Skip(1).All(x => !x.Borders.HasFlag(BorderSide.Left)), "shared body borders drawn once");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S11bH.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s11bh.png"), new ImageExportOptions
            { Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 120,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1" });
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        Check(JsonSerializer.Serialize(((OkObjectResult)controller.Catalog()).Value).Contains("s11bh"), "catalog includes S11bH");
        Check(controller.Render("s11bh", new()) is FileContentResult { FileDownloadName: "S11BH.pdf", ContentType: "application/pdf" }, "render endpoint supports S11bH");
        for (var i = 0; i < 110; i++) data.Items.Add(original[0] with { SoPhieuThu = $"LONG-{i}", DienGiai = "Giao dịch nhiều dòng " + new string('x', 120) });
        using var longReport = new S11bHReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1 && Bricks(longReport.Pages[1]).Any(x => x.Text == "Số tiền"), "column headers repeat on subsequent pages");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ QUỸ")) == 1, "signature printed once");
        using var negative = new S11bHReport(new() { Items = [new() { NgayGhiSo = new(2026, 11, 1), Chi = 500 }] });
        Check(Text(negative).Contains("(500)"), "negative balances use parentheses");
        using var zero = new S11bHReport(new());
        Check(Text(zero).Contains("0"), "zero balances remain visible");
        Console.WriteLine("All S11bH checks passed.");
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
