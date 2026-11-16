using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S55HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S55H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S55HReportData.CreateSample();
        data.Items.Reverse();
        var original = data.Items.ToArray();
        var rows = data.CreateRows();
        Check(rows[2].SoChungTu == "MG001" && data.Items.SequenceEqual(original), "chronological rows without changing input");
        Check(rows.Count(x => x.DienGiai.StartsWith("Cộng phát sinh tháng")) == 2, "monthly totals");
        Check(rows[^2].DuocMienGiam == 18_000_000m && rows[^2].DaMienGiam == 18_000_000m, "year-to-date totals");
        Check(rows[^1].SoDu == 5_500_000m, "closing includes opening balance and adjustment");
        var empty = new S55HReportData();
        Check(empty.CreateRows().Count == 4 && empty.CreateRows()[^1].SoDu == 0, "empty year retains balances and totals");
        foreach (var invalid in new S55HReportData[]
        {
            new() { Nam = 0 }, new() { Items = null! }, new() { Items = [null!] },
            new() { Items = [new()] }, new() { Items = [new() { NgayGhiSo = new(2025, 12, 31) }] }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid year or transaction rejected");
        }
        using (var designer = new S55HReport())
        {
            Check(Text(designer).Contains("MG001"), "designer preview uses sample data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var loaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S55HReportData));
            try { loaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S55HReportData)); }
            Check(loaded.FindControl("lblKyBaoCao", true).Tag?.ToString() == "KyBaoCao" &&
                loaded.FindControl("tblBodyCell4", true) is XRTableCell { ExpressionBindings.Count: 2 }, "layout retains tags and bindings");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s55h")!;
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        var content = Text(report);
        Check(!report.Landscape && report.Pages.Count == 1, "sample fits A4 portrait page");
        Check(content.Contains("MG004") && content.Contains("Nguyễn Văn An") && content.Contains("Năm: 2026"), "transactions and tagged labels printed");
        Check(!content.Contains("Nợ 3331"), "analysis note excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16 &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Số dư cuối kỳ") && x.Font.Bold), "style applies while totals stay bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S55H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s55h.png"), new ImageExportOptions
            {
                Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 110,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1"
            });
        using (var blank = new S55HReport(empty))
            Check(!Text(blank).Contains("MG001") && blank.Pages.Count == 1, "empty report does not load sample");
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        Check(controller.Render("s55h", new()) is FileContentResult { FileDownloadName: "S55H.pdf", ContentType: "application/pdf" }, "render endpoint supports S55H");
        var first = data.Items[0];
        for (var i = 0; i < 120; i++) data.Items.Add(first with { SoChungTu = $"LONG-{i}" });
        using var longReport = new S55HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1 && Bricks(longReport.Pages[1]).Any(x => x.Text.Contains("Chứng từ")), "column headings repeat across pages");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S55H checks passed.");
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
