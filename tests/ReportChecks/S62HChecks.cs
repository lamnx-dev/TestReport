using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S62HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S62H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S62HReportData.CreateSample();
        var original = data.Items.ToArray();
        var rows = data.CreateRows();
        Check(rows.Count(x => x.DienGiai.StartsWith("Cộng phát sinh tháng")) == 3, "quarter has three monthly totals");
        var total = rows.Single(x => x.DienGiai == "Cộng phát sinh trong kỳ");
        Check(total.TongNo == 37_000_000m && total.GhiCo == 30_000_000m, "four debit groups and credit total");
        Check(rows[^1].SoDu == 28_000_000m, "closing balance includes opening and adjustment");
        Check(data.Items.SequenceEqual(original), "input remains unchanged");
        data.TuNgay = new(2026, 2, 1); data.DenNgay = new(2026, 2, 28);
        data.Items = original.Where(x => x.NgayGhiSo!.Value.Month == 2).ToList();
        data.LuyKeTruocKy = new() { NguyenVatLieu = 12_000_000m, NhanCong = 18_000_000m };
        rows = data.CreateRows();
        Check(!rows.Any(x => x.DienGiai == "Cộng phát sinh trong kỳ"), "monthly report omits quarter total");
        Check(rows.Single(x => x.DienGiai == "Luỹ kế từ đầu năm").TongNo == 37_000_000m, "year-to-date includes prior months");
        data.Items.Clear();
        Check(data.CreateRows().Count(x => x.DienGiai.StartsWith("Cộng phát sinh tháng")) == 1, "empty month retains total");
        data.Items.Add(original[0]);
        var rejected = false;
        try { data.CreateRows(); } catch (ArgumentException) { rejected = true; }
        Check(rejected, "out-of-period transaction rejected");

        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using (var designerReport = new S62HReport())
        {
            Check(designerReport.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource,
                "designer exposes object data source");
            Check(Text(designerReport).Contains("VT001"), "designer preview loads sample rows");
            using var layout = new MemoryStream();
            designerReport.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var reloaded = new XtraReport();
            // Only the known local DTO is trusted for this serialization check.
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S62HReportData));
            try { reloaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S62HReportData)); }
            Check(reloaded.FindControl("tblBodyCell5", true) is XRTableCell { ExpressionBindings.Count: 2 },
                "designer layout round trip retains bindings");
            Check(reloaded.FindControl("headingDebit", true) is XRLabel { Text: "Ghi Nợ TK 154" },
                "designer layout round trip retains header");
            Check(reloaded.FindControl("lblSanPham", true).Tag?.ToString() == "SanPham",
                "designer layout retains text tag");
        }
        using var report = factory.Create("s62h")!;
        Check(report is S62HReport && report.Landscape, "factory returns landscape report");
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        Check(report.Pages.Count == 1, "sample fits one page");
        var content = Text(report);
        Check(content.Contains("DV01") && content.Contains("Nguyễn Văn An") &&
            content.Contains("31 tháng 03 năm 2026"), "header and footer populated through tags");
        Check(content.Contains("VT001") && content.Contains("KC003") && content.Contains("Số dư cuối kỳ"), "transactions and balances printed");
        Check(!content.Contains("no_ct1") && !content.Contains("dm_chitiet"), "analysis instructions excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "appearance reaches title");
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Số dư cuối kỳ") && x.Font.Bold), "total remains bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S62H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s62h.png"), new ImageExportOptions
        {
            Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 100,
            ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1"
        });
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        Check(controller.Render("s62h", new()) is FileContentResult { ContentType: "application/pdf", FileDownloadName: "S62H.pdf" }, "render API supports S62H");
        data = S62HReportData.CreateSample();
        var item = data.Items[0];
        for (var i = 0; i < 110; i++) data.Items.Add(item with { SoChungTu = $"LONG-{i}", DienGiai = "Chi phí dịch vụ " + new string('x', 80) });
        using var longReport = new S62HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1, "long report paginates");
        Check(Bricks(longReport.Pages[1]).Any(x => x.Text.Contains("Ghi Nợ TK 154")), "column headers repeat");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S62H checks passed.");
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
