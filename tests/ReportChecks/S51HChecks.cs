using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S51HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S51H " + message);
        }

        var data = S51HReportData.CreateSample();
        var rows = data.CreateRows();
        Check(data.KyBaoCao == "Quý 1/2026" && rows.Count == 6, "quarterly sample renders three transactions and three summary rows");
        Check(rows[0] is { PhatSinhNo: 0, PhatSinhCo: 120_000_000m, TaiKhoanDoiUng: "135" } &&
            rows[2] is { PhatSinhNo: 5_000_000m, PhatSinhCo: 0, TaiKhoanDoiUng: "135", GhiChu: "Theo biên bản" },
            "account sides, counterpart and notes are retained");

        data.TuNgay = new(2026, 3, 1);
        var credit = data.Items[0];
        var transfer = data.Items[^1];
        data.Items.AddRange(
        [
            credit with { MaDonVi = "OTHER", NgayGhiSo = new(2026, 3, 1) },
            credit with { TaiKhoanCo = "5111", NgayGhiSo = new(2026, 3, 1) },
            credit with { NgayGhiSo = new(2025, 12, 31) },
            credit with { NgayGhiSo = new(2026, 4, 1) },
            transfer with { NgayGhiSo = new(2026, 2, 28) },
            transfer with { TaiKhoanCo = "9111", NgayGhiSo = new(2026, 3, 1), SoTien = 10m }
        ]);
        rows = data.CreateRows();
        Check(data.KyBaoCao == "Tháng 03/2026" && rows.Count == 4 &&
            rows[^3] is { PhatSinhNo: 5_000_000m, PhatSinhCo: 0 } &&
            rows[^2] is { PhatSinhNo: 5_000_000m, PhatSinhCo: 200_000_000m } &&
            rows[^1] is { PhatSinhNo: 195_000_010m, PhatSinhCo: 0 },
            "monthly, year-to-date and 911 transfer totals use their respective scopes");

        data.DenNgay = new(2026, 12, 31);
        data.TuNgay = new(2026, 1, 1);
        Check(data.KyBaoCao == "Năm 2026" && new S51HReportData().CreateRows().All(x => x.InDam),
            "yearly period and empty summary");
        using (var designer = new S51HReport())
        {
            Check(designer.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource &&
                designer.FindControl("tblBody", true) is XRTable table && table.Rows[0].Cells.Count == 8,
                "designer exposes eight bound columns");
            Check(Content(designer).Contains("DT001"), "designer preview loads sample data");
        }

        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s51h")!;
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 } });
        var content = Content(report);
        Check(report is S51HReport { Landscape: true } && report.Pages.Count == 1 &&
            content.Contains("SỔ CHI TIẾT DOANH THU") && content.Contains("Quý 1/2026") &&
            content.Contains("Ghi chú") && content.Contains("Theo biên bản") && content.Contains("195,000,000") &&
            content.Contains("Lê Văn Minh") && report.FindControl("lblTitle", true).Font.SizeInPoints == 16,
            "styled sample prints report headings, transactions, totals and signatures");

        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        Check(controller.Catalog() is OkObjectResult catalog &&
            System.Text.Json.JsonSerializer.Serialize(catalog.Value).Contains("\"s51h\""), "catalog includes S51H");
        var pdf = controller.Render("s51h", new()) as FileContentResult;
        Check(pdf is { ContentType: "application/pdf", FileDownloadName: "S51H.pdf" } &&
            pdf.FileContents.AsSpan().StartsWith("%PDF-"u8), "API exports an S51H PDF");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S51H.pdf"), pdf!.FileContents);

        data = S51HReportData.CreateSample();
        for (var i = 0; i < 60; i++)
            data.Items.Add(credit with { SoChungTu = $"CT-{i:00}", DienGiai = string.Join(" ", Enumerable.Repeat("Ghi nhận doanh thu", 10)) });
        using var longReport = new S51HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1 &&
            longReport.Pages.Cast<Page>().All(p => Bricks(p).Any(x => x.Text == "Số phát sinh")) &&
            longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1,
            "column headings repeat and signatures print once across pages");
    }

    private static string Content(XtraReport report)
    {
        using var stream = new MemoryStream();
        report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
        return Encoding.UTF8.GetString(stream.ToArray());
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
            foreach (var item in Bricks(child))
                yield return item;
    }
}
