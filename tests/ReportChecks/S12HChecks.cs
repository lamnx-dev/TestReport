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

internal static class S12HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S12H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S12HReportData.CreateSample();
        var original = data.Items.ToArray();
        data.Items = [original[3], original[0], original[1], original[2]];
        var unsorted = data.Items.ToArray();
        var rows = data.CreateRows();
        Check(rows[2].SoChungTu == "BC001" && rows[3].SoChungTu == "BN001" && data.Items.SequenceEqual(unsorted), "sort by posting date, preserve same-day order and input");
        Check(rows[2].ConLai == 150_000_000m && rows[3].ConLai == 138_000_000m, "running balances");
        var days = rows.Where(x => x.DienGiai == "Cộng cuối ngày").ToArray();
        Check(days.Length == 3 && days[0].GuiVao == 100_000_000m && days[0].RutRa == 12_000_000m && days[0].ConLai == 138_000_000m, "daily totals");
        Check(rows[^3].GuiVao == 105_000_000m && rows[^3].RutRa == 32_000_000m, "monthly totals exclude daily totals");
        Check(rows[^2].GuiVao == 405_000_000m && rows[^2].RutRa == 282_000_000m, "year-to-date includes prior months");
        Check(rows[^1].ConLai == 123_000_000m, "closing balance excludes prior-month turnover");
        var empty = new S12HReportData { SoDuDauThang = 42m };
        Check(empty.CreateRows().Count == 5 && empty.CreateRows()[^1].ConLai == 42m, "empty month preserves balance");
        foreach (var invalid in new S12HReportData[]
        {
            new() { Thang = 0 }, new() { Nam = 10000 }, new() { Items = null! }, new() { Items = [null!] },
            new() { Items = [new()] }, new() { Items = [new() { NgayGhiSo = new(2026, 10, 31) }] },
            new() { Items = [new() { NgayGhiSo = new(2025, 11, 1) }] }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid period or transaction rejected");
        }
        using (var designer = new S12HReport())
        {
            Check(Text(designer).Contains("BC001"), "designer preview loads sample rows");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var loaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S12HReportData));
            try { loaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S12HReportData)); }
            Check(loaded.FindControl("lblNoiMoTaiKhoan", true).Tag?.ToString() == "NoiMoTaiKhoan" &&
                loaded.FindControl("tblBodyCell7", true) is XRTableCell { ExpressionBindings.Count: 2 }, "layout retains tags and bindings");
        }
        using (var blank = new S12HReport(empty))
        {
            var content = Text(blank);
            Check(!content.Contains("BC001") && !content.Contains("KẾ TOÁN") && !content.Contains("Ký, họ tên"), "empty data and undeclared signatures stay blank");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s12h")!;
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        var text = Text(report);
        Check(report.Landscape && report.Pages.Count == 1, "sample fits A4 landscape page");
        Check(text.Contains("3713.0.1234567") && text.Contains("Kho bạc Nhà nước Hà Nội") && text.Contains("VNĐ") && text.Contains("Tháng 11 năm 2026"), "account details populated by tags");
        Check(text.Contains("BC002") && text.Contains("Nguyễn Văn An") && text.Contains("THỦ TRƯỞNG ĐƠN VỊ"), "transactions and configured signatures printed");
        Check(!text.Contains("S12_H") && !text.Contains("DM_dvql") && !text.Contains("userdate"), "analysis instructions excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16 &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Số dư cuối kỳ") && x.Font.Bold), "appearance applies and totals stay bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S12H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s12h.png"), new ImageExportOptions
            {
                Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 110,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1"
            });
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        Check(JsonSerializer.Serialize(((OkObjectResult)controller.Catalog()).Value).Contains("s12h"), "catalog includes S12H");
        Check(controller.Render("s12h", new()) is FileContentResult { FileDownloadName: "S12H.pdf", ContentType: "application/pdf" }, "render endpoint supports S12H");
        var first = data.Items[0];
        for (var i = 0; i < 110; i++) data.Items.Add(first with { SoChungTu = $"LONG-{i}", DienGiai = "Giao dịch nhiều dòng " + new string('x', 120) });
        using var longReport = new S12HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1 && Bricks(longReport.Pages[1]).Any(x => x.Text == "Số tiền"), "headers repeat on subsequent pages");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        var negative = new S12HReportData { Items = [new() { NgayGhiSo = new(2026, 11, 1), RutRa = 500m }] };
        using var negativeReport = new S12HReport(negative);
        Check(Text(negativeReport).Contains("(500)"), "negative balances use parentheses");
        Console.WriteLine("All S12H checks passed.");
    }

    private static IEnumerable<TextBrick> Bricks(BrickBase brick)
    {
        if (brick is TextBrick text) yield return text;
        System.Collections.IEnumerable? children = brick switch
        {
            Page p => p.InnerBricks, CompositeBrick c => c.InnerBricks, Brick b => b.Bricks, _ => null
        };
        if (children is null) yield break;
        foreach (BrickBase child in children)
            foreach (var item in Bricks(child)) yield return item;
    }
}
