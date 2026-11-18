using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S54HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S54H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S54HReportData.CreateSample();
        data.Items.Reverse();
        var original = data.Items.ToArray();
        var rows = data.CreateRows();
        Check(rows[3].SoChungTu == "HT001" && data.Items.SequenceEqual(original), "chronological rows without changing input");
        Check(rows[1].DuocHoanLai == 500_000m && rows[^2].DuocHoanLai == 18_000_000m && rows[^2].DaHoanLai == 18_000_000m, "type 16 excluded from normal totals");
        Check(rows[^1].DuocHoanLai == 5_500_000m && rows[^1].DaHoanLai == 0, "debit closing includes opening and adjustment");
        var empty = new S54HReportData();
        Check(empty.CreateRows().Count == 6, "empty report retains headings and totals");
        foreach (var invalid in new S54HReportData[]
        {
            new() { TaiKhoan = "3331" }, new() { Items = null! }, new() { Items = [null!] },
            new() { TuNgay = new(2027, 1, 1) }, new() { DenNgay = new(2027, 1, 1) }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid account, period or transactions rejected");
        }
        data.TuNgay = new(2026, 2, 1);
        data.DenNgay = new(2026, 2, 28);
        data.Items.Add(new() { NgayGhiSo = new(2026, 2, 5), TaiKhoan = "3331", PhatSinhNo = 999m });
        data.Items.Add(new() { NgayGhiSo = new(2026, 3, 1), PhatSinhNo = 999m });
        rows = data.CreateRows();
        Check(rows[3].SoChungTu == "HT003" && rows[^3].DuocHoanLai == 6_000_000m && rows[^2].DuocHoanLai == 18_000_000m && rows[^1].DuocHoanLai == 5_500_000m, "period, account and year-to-date filtering");
        var credit = new S54HReportData { CoDauNam = 10, Items = [new() { NgayGhiSo = new(2026, 1, 1), MaLoai = 16, PhatSinhCo = 2 }] }.CreateRows();
        Check(credit[0].DaHoanLai == 10 && credit[^1].DaHoanLai == 12 && credit[^1].DuocHoanLai == 0, "credit balances print in second amount column");
        data = S54HReportData.CreateSample();
        using (var designer = new S54HReport())
        {
            Check(Text(designer).Contains("HT001"), "designer preview uses sample data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var loaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S54HReportData));
            try { loaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S54HReportData)); }
            Check(loaded.FindControl("lblKyBaoCao", true).Tag?.ToString() == "KyBaoCao" &&
                loaded.FindControl("tblBodyCell4", true) is XRTableCell { ExpressionBindings.Count: 2 }, "layout retains tags and bindings");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s54h")!;
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        var content = Text(report);
        Check(!report.Landscape && report.Pages.Count == 1, "sample fits A4 portrait page");
        Check(content.Contains("HT004") && content.Contains("Nguyễn Văn An") && content.Contains("Năm: 2026"), "transactions and tagged labels printed");
        Check(content.Contains("ĐƯỢC HOÀN LẠI") && content.Contains("Tài khoản: 133") && !content.Contains("MIỄN GIẢM"), "refund title and selected account printed");
        Check(!content.Contains("Nợ 3331"), "analysis note excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16 &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Số dư cuối kỳ") && x.Font.Bold), "style applies while totals stay bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S54H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s54h.png"), new ImageExportOptions
            {
                Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 110,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1"
            });
        using (var blank = new S54HReport(empty))
            Check(!Text(blank).Contains("HT001") && blank.Pages.Count == 1, "empty report does not load sample");
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        var catalog = (OkObjectResult)controller.Catalog();
        Check(System.Text.Json.JsonSerializer.Serialize(catalog.Value).Contains("s54h"), "catalog includes S54H for frontend selection");
        Check(controller.Render("s54h", new()) is FileContentResult { FileDownloadName: "S54H.pdf", ContentType: "application/pdf" }, "render endpoint supports S54H");
        var first = data.Items[1];
        for (var i = 0; i < 120; i++) data.Items.Add(first with { SoChungTu = $"LONG-{i}" });
        using var longReport = new S54HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1 && Bricks(longReport.Pages[1]).Any(x => x.Text.Contains("Chứng từ")), "column headings repeat across pages");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S54H checks passed.");
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
