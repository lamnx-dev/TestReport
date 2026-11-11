using System.Drawing;
using System.Text;
using DevExpress.Drawing;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Reports;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    Console.WriteLine($"PASS {message}");
}

static IEnumerable<TextBrick> TextBricks(BrickBase brick)
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
        foreach (var item in TextBricks(child)) yield return item;
}

// Child tags merge individual properties; explicit false must override true.
using (var report = new XtraReport())
{
    report.Bands.Add(new DetailBand { HeightF = 0 });
    var nested = new DetailReportBand();
    var detail = new DetailBand();
    var table = new XRTable { Tag = "TableBody" };
    var row = new XRTableRow { Tag = "TableHeader" };
    var cell = new XRTableCell
    {
        Tag = "ReportTitle", Text = "Value", TextFormatString = "{0:n2}",
        TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight,
        Font = new DXFont("Tahoma", 8, DXFontStyle.Italic)
    };
    cell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Font.Bold", "True"));
    row.Cells.Add(cell); table.Rows.Add(row); detail.Controls.Add(table);
    nested.Bands.Add(detail); report.Bands.Add(nested);
    var originalFont = cell.GetEffectiveFont();
    ReportAppearanceApplier.Apply(report, null);
    Check(cell.GetEffectiveFont().Equals(originalFont), "Omitted appearance keeps template");
    ReportAppearanceApplier.Apply(report, new()
    {
        TableBody = new() { FontFamily = "Arial", FontSize = 11, Bold = true, Color = "#123456" },
        TableHeader = new() { FontSize = 12, Bold = false },
        Title = new() { Italic = false }
    });
    Check(cell.Font.Name == "Arial" && cell.Font.SizeInPoints == 12, "Nested DetailReport, table and row inheritance");
    Check(!cell.Font.Bold && !cell.Font.Italic, "Child tags override false and merge remaining properties");
    Check(cell.ForeColor.ToArgb() == Color.FromArgb(0x12, 0x34, 0x56).ToArgb(), "Inherited color reaches cell");
    Check(cell.TextFormatString == "{0:n2}" && cell.TextAlignment == DevExpress.XtraPrinting.TextAlignment.MiddleRight, "Format and alignment preserved");
    Check(cell.ExpressionBindings.Count == 1, "Conditional binding preserved");
    report.CreateDocument();
    Check(report.Pages.Cast<Page>().SelectMany(TextBricks).Any(b => b.Text == "Value" && b.Font.Bold), "Conditional total emphasis wins during rendering");
}

foreach (var style in new ReportElementStyle[]
{
    new() { FontFamily = "Unsupported" }, new() { FontSize = 5 }, new() { FontSize = 25 },
    new() { FontSize = float.NaN }, new() { Color = "red" }, new() { Color = "#123456\n" }
})
{
    var rejected = false;
    try { ReportAppearanceApplier.Validate(new() { Title = style }); }
    catch (ArgumentException) { rejected = true; }
    Check(rejected, $"Invalid style rejected: {style}");
}

var factory = new SampleReportFactory();
var renderer = new ReportPdfRenderer();
foreach (var id in new[] { "s11h", "s106h" })
{
    using var report = factory.Create(id)!;
    var title = report.FindControl(id == "s11h" ? "xrLabel4" : "xrLabel5", true);
    Check(title.Tag?.ToString() == "ReportTitle", $"{id} title tag");
    var pageWidth = report.PageWidthF;
    var pageHeight = report.PageHeightF;
    var defaultFont = title.GetEffectiveFont();
    ReportAppearanceApplier.Apply(report, new());
    Check(title.GetEffectiveFont().Equals(defaultFont), $"{id} empty appearance keeps font");
    var originalPdf = renderer.Render(report);
    Check(Encoding.ASCII.GetString(originalPdf, 0, 5) == "%PDF-", $"{id} default PDF export");
    ReportAppearanceApplier.Apply(report, new()
    {
        Title = new() { FontFamily = "Tahoma", FontSize = 16, Color = "#123456" },
        TableHeader = new() { FontSize = 10, Bold = true },
        TableBody = new() { FontSize = 9, Bold = false }
    });
    Check(title.Font.Name == "Tahoma" && title.Font.SizeInPoints == 16, $"{id} title style");
    var tables = id == "s11h" ? new[] { "xrTable1", "xrTable2" }
        : new[] { "xrTable1", "xrTable2", "xrTable3", "xrTable4", "xrTable5", "xrTable6" };
    foreach (var name in tables)
    {
        var table = (XRTable)report.FindControl(name, true);
        Check(table.Rows[0].Cells[0].Font.SizeInPoints == (table.Tag?.ToString() == "TableHeader" ? 10 : 9), $"{id} {name} style reaches cells");
    }
    report.CreateDocument();
    Check(report.Pages.Count > 0 && pageWidth == report.PageWidthF && pageHeight == report.PageHeightF, $"{id} pages and geometry preserved");
    using var text = new MemoryStream();
    report.ExportToText(text);
    var content = Encoding.UTF8.GetString(text.ToArray());
    Check(content.Contains(id == "s11h" ? "PT001" : "KP01"), $"{id} sample transactions printed");
    if (id == "s106h")
    {
        Check(content.Contains("6001") && content.Contains("6003"), "S106H sub-item labels rendered");
        Check(report.Pages.Cast<Page>().SelectMany(TextBricks).Any(b => b.Text.Contains("Tổng cộng số đã sử dụng") && b.Font.Bold), "S106H final total stays bold with body bold=false");
    }
    Check(renderer.Render(report).Length > 1000, $"{id} styled PDF export");
    if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
    {
        report.ExportToImage(Path.Combine(AppContext.BaseDirectory, $"{id}.png"), new ImageExportOptions
        {
            Format = System.Drawing.Imaging.ImageFormat.Png,
            PageRange = "1",
            ExportMode = ImageExportMode.SingleFilePageByPage,
            Resolution = 100
        });
    }
}

// Multi-page sample exercises repeated headers and all report sections.
var data = S106HReportData.CreateSample();
var firstMonth = data.NhanVaNopTra.Thang[0];
var transaction = firstMonth.Items[0];
for (var i = 0; i < 100; i++) firstMonth.Items.Add(transaction with { SoChungTu = $"LONG-{i}" });
using (var report = new S106HReport(data))
{
    ReportAppearanceApplier.Apply(report, new() { TableBody = new() { FontSize = 10 } });
    report.CreateDocument();
    Check(report.Pages.Count > 1, "Multi-page S106H document generated");
    Check(renderer.Render(report).Length > 1000, "Multi-page PDF export");
    Check(TextBricks(report.Pages[1]).Any(b => b.Text.Contains("Ngày tháng ghi sổ")), "Table header repeats on second page");
}

var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
Check(controller.Render("unknown", new()) is NotFoundObjectResult, "Unknown report returns 404");
Check(controller.Render("s11h", new() { Appearance = new() { Title = new() { FontSize = 100 } } }) is BadRequestObjectResult, "Invalid appearance returns 400");
Console.WriteLine("All report checks passed.");
