using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Reports;
using TestReport.Api.Infrastructure.Renderers;

internal static class S101HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception("S101H " + message);
            Console.WriteLine("PASS S101H " + message);
        }
        static string Content(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        var data = S101HReportData.CreateSample();
        var rows = data.CreateRows();
        Check(rows.Count == 9 && rows[4].SoChungTu == "DT001" && rows[^1].MaChiTieu == "5", "template order and decision rows");
        data.ChiTieu.Single(r => r.MaChiTieu == "5").SoTien!.TongSoTien = 123m;
        Check(data.CreateRows()[^1].SoTien!.TongSoTien == 123m, "caller balances preserved");
        data.ChiTieu.Single(r => r.MaChiTieu == "5").SoTien!.TongSoTien = 150_000_000m;
        Check(data.ChiTieu.Count == 7 && data.QuyetDinh.Count == 2, "input collections unchanged");
        using (var empty = new S101HReport(new S101HReportData()))
        {
            var content = Content(empty);
            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "s101h-empty.txt"), content);
            Check(content.Contains("SỔ THEO DÕI DỰ TOÁN") && content.Contains("Số dư dự toán chuyển năm sau"), "empty data prints complete form");
            Check(!content.Contains("DT001") && !content.Contains("Nguyễn Văn An"), "empty data never loads sample");
            Check(empty.Pages.Count == 1, "empty form fits one page");
        }
        using (var preview = new S101HReport())
        {
            Check(Content(preview).Contains("DT001"), "Designer sample data");
            using var layout = new MemoryStream();
            preview.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var reloaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S101HReportData));
            try { reloaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S101HReportData)); }
            Check(reloaded.FindControl("bodyCell14", true) is XRTableCell { ExpressionBindings.Count: 2 }, "Designer round trip keeps amount bindings");
            Check(reloaded.FindControl("hdrK80", true) is XRLabel { Text: "Trong đó:", Tag: "TableHeader" }, "Designer round trip keeps merged header");
        }

        using (var report = new S101HReport(data))
        {
            ReportAppearanceApplier.Apply(report, new()
            {
                Title = new() { FontSize = 16, Color = "#123456" },
                TableHeader = new() { FontFamily = "Tahoma" },
                TableBody = new() { Bold = false }
            });
            var content = Content(report);
            Check(report.Landscape && report.PaperKind == DevExpress.Drawing.Printing.DXPaperKind.A3, "A3 landscape for 15 columns");
            Check(content.Contains("(10,000,000)") && content.Contains("DT001") && content.Contains("31 tháng 12 năm 2026"), "dates, negative amounts and labels rendered");
            Check(content.Contains("03/01/2026"), "document dates use literal slashes");
            Check(!content.Contains("Hướng dẫn lập") && !content.Contains("TK013") && !content.Contains("Các Khoản"), "source instructions excluded");
            Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "title appearance");
            Check(report.FindControl("hdrF80", true).Font.Name == "Tahoma", "merged header appearance");
            Check(Bricks(report.Pages[0]).Any(b => b.Text == "1. Dự toán năm trước sang" && b.Font.Bold), "totals stay bold with body bold=false");
            Check(report.Pages.Count == 1, "sample fits one page");
            report.ExportToPdf(Path.Combine(AppContext.BaseDirectory, "S101-H.pdf"));
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s101h.png"), new ImageExportOptions
            {
                Format = System.Drawing.Imaging.ImageFormat.Png, PageRange = "1",
                ExportMode = ImageExportMode.SingleFilePageByPage, Resolution = 100
            });
        }
        data = S101HReportData.CreateSample();
        data.QuyetDinh = Enumerable.Range(0, 65).Select(i => new S101HQuyetDinh
        {
            SoQuyetDinh = i + "/QĐ-BHXH " + string.Join(" ", Enumerable.Repeat("bổ sung dự toán", 10)),
            SoTien = new() { TongSoTien = i == 0 ? 0m : decimal.MaxValue }
        }).ToList();
        using (var report = new S101HReport(data))
        {
            report.CreateDocument();
            Check(report.Pages.Count > 1, "many decisions paginate");
            Check(Bricks(report.Pages[1]).Any(b => b.Text.Contains("Chỉ tiêu")), "table header repeats");
            Check(report.Pages.Cast<Page>().Count(p => Bricks(p).Any(b => b.Text == "KẾ TOÁN")) == 1, "signatures print once");
            Check(Content(report).Contains("79,228,162,514,264,337,593,543,950,335"), "large amount is not truncated");
            Check(Bricks(report.Pages[0]).Any(b => b.Text == "-"), "zero amounts use template dash");
            report.ExportToPdf(Path.Combine(AppContext.BaseDirectory, "S101-H-long.pdf"));
        }
        foreach (var code in new[] { "bad", "1" })
        {
            var invalid = new S101HReportData { ChiTieu = [new() { MaChiTieu = "1" }, new() { MaChiTieu = code }] };
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid or duplicate indicator rejected: " + code);
        }
        var controller = new ReportsController(new(), new(), NullLogger<ReportsController>.Instance);
        Check(System.Text.Json.JsonSerializer.Serialize(((OkObjectResult)controller.Catalog()).Value).Contains("s101h"), "catalog contains report");
        Check(controller.Render("s101h", new()) is FileContentResult { ContentType: "application/pdf", FileDownloadName: "S101H.pdf" }, "render endpoint exports PDF");
        Check(controller.Render("s101h", new() { Appearance = new() { Title = new() { FontSize = 100 } } }) is BadRequestObjectResult, "render endpoint validates style");
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
