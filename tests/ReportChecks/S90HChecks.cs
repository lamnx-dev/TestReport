using System.Text;
using DevExpress.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Reports;

internal static class S90HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S90H " + message);
        }
        static string Content(XtraReport report)
        {
            using var text = new MemoryStream();
            report.ExportToText(text, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(text.ToArray());
        }

        var data = new S90HReportData
        {
            ChiTieu =
            [
                new() { MaChiTieu = "03", SoTien = 200m },
                new() { MaChiTieu = "02", SoTien = 123m },
                new() { MaChiTieu = "04", SoTien = -5m },
                new() { MaChiTieu = "05", SoTien = 0m },
                new() { MaChiTieu = "10", SoTien = 999m },
                new() { MaChiTieu = "09", SoTien = 456m },
                new() { MaChiTieu = "khau-hao-bo-sung", SoTien = 17m }
            ]
        };
        var rows = data.CreateRows();
        Check(rows.Count == 16 && rows[0].Stt == "I" && rows[^1].Stt == "7.3", "fixed rows and order");
        Check(rows.Single(r => r.MaChiTieu == "02").SoTien == 123m &&
              rows.Single(r => r.MaChiTieu == "10").SoTien == 999m &&
              rows.Single(r => r.MaChiTieu == "09").SoTien == 456m, "caller totals preserved");
        Check(rows.Single(r => r.MaChiTieu == "khau-hao-bo-sung").MaIn == "", "internal code hidden");
        Check(data.ChiTieu[0].MaChiTieu == "03" && data.ChiTieu.Count == 7, "input unchanged");

        using (var report = new S90HReport(data))
        {
            report.CreateDocument();
            var content = Content(report);
            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "s90h-content.txt"), content);
            Check(report.PageWidth == 827 && report.PageHeight == 1169 && !report.Landscape, "A4 portrait");
            Check(content.Contains("(5)") && content.Contains("123") && content.Contains("0"), "amount formatting");
            Check(!content.Contains("khau-hao-bo-sung") && !content.Contains("Nợ TK") && !content.Contains("S101-H"), "no source instructions");
            Check(content.Contains("KẾ TOÁN TRƯỞNG") && content.Contains("7.1"), "template labels");
            var table = (XRTable)report.FindControl("tblBody", true);
            Check(table.Rows[0].Cells[3].TextFormatString == "{0:#,##0;(#,##0);0}", "numeric format includes explicit zero");
        }
        using (var report = new S90HReport(new S90HReportData()))
        {
            Check(Content(report).Contains("Quỹ phúc lợi") && report.Pages.Count == 1, "empty data prints full form on one page");
            Check(!Content(report).Contains("Nguyễn Văn An"), "empty data never loads sample");
        }
        using (var report = new S90HReport())
        {
            Check(Content(report).Contains("950,000,000"), "Designer sample data");
        }
        using (var report = new S90HReport(S90HReportData.CreateSample()))
        {
            ReportAppearanceApplier.Apply(report, new() { TableBody = new() { Bold = false }, Title = new() { FontSize = 16 } });
            report.CreateDocument();
            Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "appearance reaches title");
            var sampleText = Content(report);
            Check(sampleText.Contains("Nguyễn Văn An") && sampleText.Contains("31 tháng 12 năm 2026"), "sample labels and dates");
            var pdf = Path.Combine(AppContext.BaseDirectory, "S90-H.pdf");
            report.ExportToPdf(pdf);
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s90h.png"), new ImageExportOptions
                {
                    Format = System.Drawing.Imaging.ImageFormat.Png, PageRange = "1",
                    ExportMode = ImageExportMode.SingleFilePageByPage, Resolution = 120
                });
        }
        var longData = S90HReportData.CreateSample();
        foreach (var item in longData.ChiTieu) item.GhiChu = string.Join(" ", Enumerable.Repeat("Ghi chú nhiều dòng", 55));
        longData.ChiTieu[0].SoTien = decimal.MaxValue;
        using (var report = new S90HReport(longData))
        {
            ReportAppearanceApplier.Apply(report, new() { TableBody = new() { FontSize = 11, Bold = false } });
            report.CreateDocument();
            Check(report.Pages.Count > 1, "long notes generate multiple pages");
            // Inspect actual rendered bricks rather than the template controls.
            var secondPage = Bricks(report.Pages[1]).ToList();
            Check(secondPage.Any(b => b.Text == "CHỈ TIÊU"), "table header repeats on second page");
            var all = report.Pages.Cast<Page>().SelectMany(Bricks).ToList();
            Check(all.Any(b => b.Text == "Kinh phí được giao tự chủ" && b.Font.Bold), "section emphasis survives body bold=false");
            Check(report.Pages.Cast<Page>().Count(p => Bricks(p).Any(b => b.Text == "KẾ TOÁN")) == 1, "signature block printed once");
            Check(all.Where(b => b.Text.Length > 0).All(b => b.Rect.Right <= 2242), "rendered text stays within printable width (300 dpi)");
            report.ExportToPdf(Path.Combine(AppContext.BaseDirectory, "S90-H-long.pdf"));
        }
        var controller = new ReportController(null!, null!)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        Check(controller.ExportS90HPdf(null!) is BadRequestObjectResult, "null body rejected");
        foreach (var items in new List<S90HChiTieu>[]
        {
            [new() { MaChiTieu = "unknown" }],
            [new() { MaChiTieu = "01" }, new() { MaChiTieu = "01" }],
            [null!], null!
        })
            Check(controller.ExportS90HPdf(new() { ChiTieu = items }) is BadRequestObjectResult, "invalid indicators rejected");
        var file = (FileStreamResult)controller.ExportS90HPdf(new());
        Check(file.ContentType == "application/pdf" && file.FileDownloadName == "S90-H.pdf", "export response metadata");
        file.FileStream.Dispose();
        var preview = (FileStreamResult)controller.PreviewS90HPdf();
        Check(controller.Response.Headers.ContentDisposition.ToString().Contains("inline"), "preview inline");
        preview.FileStream.Dispose();
    }

    private static IEnumerable<TextBrick> Bricks(BrickBase brick)
    {
        if (brick is TextBrick text) yield return text;
        System.Collections.IEnumerable? children = brick switch
        {
            Page page => page.InnerBricks,
            CompositeBrick composite => composite.InnerBricks,
            Brick container => container.Bricks,
            _ => null
        };
        if (children is null) yield break;
        foreach (BrickBase child in children)
            foreach (var item in Bricks(child)) yield return item;
    }
}
