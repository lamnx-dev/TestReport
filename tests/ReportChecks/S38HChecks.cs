using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S38HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S38H " + message);
        }

        var data = S38HReportData.CreateSample();
        var rows = data.CreateRows();
        Check(data.KyBaoCao == "Quý 2/2026" && rows.Count == 9 &&
            rows[0] is { DuNo: null, DuCo: 33_000_000m } && rows[1] is { PhatSinhNo: 0, PhatSinhCo: 0 },
            "opening includes prior movements and adjustments");
        Check(rows[3] is { SoChungTu: "MH042", PhatSinhCo: 25_000_000m, TaiKhoanDoiUng: "152",
            DuCo: 58_000_000m, GhiChu: "Hợp đồng 04" } &&
            rows[4] is { SoChungTu: "UNC051", PhatSinhNo: 30_000_000m, TaiKhoanDoiUng: "112", DuCo: 28_000_000m },
            "voucher fields, posting sides, counterpart and running balances");
        Check(rows[^3] is { PhatSinhNo: 30_000_000m, PhatSinhCo: 33_000_000m } &&
            rows[^2] is { PhatSinhNo: 45_000_000m, PhatSinhCo: 33_000_000m } &&
            rows[^1] is { DuNo: null, DuCo: 36_000_000m },
            "period movements, year-to-date and closing balance are reconciled");
        data.TuNgay = new(2026, 1, 1);
        data.DenNgay = new(2026, 12, 31);
        rows = data.CreateRows();
        Check(data.KyBaoCao == "Năm 2026" && rows[0].DuCo == 50_000_000m && rows[1].PhatSinhNo == 2_000_000m &&
            rows[^2].PhatSinhNo == 45_000_000m && rows[^1].DuCo == 36_000_000m,
            "adjustment appears separately in the period and stays out of turnover");

        data = new() { TuNgay = new(2026, 4, 1), DenNgay = new(2026, 4, 30) };
        var debit = new S38HPosting { NgayGhiSo = new(2026, 4, 1), TaiKhoanNo = "3311", TaiKhoanCo = "112", SoTien = 100m };
        data.Items =
        [
            debit with { NgayGhiSo = new(2026, 4, 2), SoChungTu = "NB002", TaiKhoanCo = "3312", SoTien = 30m },
            debit with { MaDonVi = "OTHER" },
            debit with { NgayGhiSo = new(2025, 4, 1) },
            debit with { NgayGhiSo = new(2026, 5, 1) },
            debit
        ];
        rows = data.CreateRows();
        Check(data.KyBaoCao == "Tháng 04/2026", "monthly period label");
        Check(rows.Count == 9 && rows[3].DuNo == 100m &&
            rows[4] is { TaiKhoanDoiUng: "3312", PhatSinhNo: 30m, DuNo: null } &&
            rows[5] is { TaiKhoanDoiUng: "3311", PhatSinhCo: 30m, DuNo: 100m } &&
            rows[^1].DuNo == 100m && data.Items[0].SoChungTu == "NB002",
            "account prefix, same-account transfers, unit/date filters and posting order");
        Check(new S38HReportData().CreateRows() is [ { DuNo: 0 }, _, _, _, _, { DuNo: 0 } ],
            "empty periods retain the six summary rows with zero opening and closing");

        using (var designer = new S38HReport())
        {
            Check(designer.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource &&
                designer.FindControl("tblBody", true) is XRTable table && table.Rows[0].Cells.Count == 10 &&
                Content(designer).Contains("MH042"), "designer binds and previews all ten columns");
        }

        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s38h")!;
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 } });
        var content = Content(report);
        Check(report is S38HReport { Landscape: true } && report.Pages.Count == 1 &&
            content.Contains("SỔ CHI TIẾT TÀI KHOẢN: 331") && content.Contains("Quý 2/2026") &&
            content.Contains("Hợp đồng 04") && content.Contains("36,000,000") && content.Contains("Lê Văn Minh") &&
            report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "styled landscape sample prints account, notes, totals and signatures");

        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        Check(controller.Catalog() is OkObjectResult catalog &&
            System.Text.Json.JsonSerializer.Serialize(catalog.Value).Contains("\"s38h\""), "catalog includes S38H");
        var pdf = controller.Render("s38h", new()) as FileContentResult;
        Check(pdf is { ContentType: "application/pdf", FileDownloadName: "S38H.pdf" } &&
            pdf.FileContents.AsSpan().StartsWith("%PDF-"u8), "API exports an S38H PDF");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S38H.pdf"), pdf!.FileContents);

        for (var i = 0; i < 60; i++)
            data.Items.Add(debit with { SoChungTu = $"CT-{i:00}", DienGiai = string.Join(" ", Enumerable.Repeat("Ghi nhận công nợ", 10)) });
        using var longReport = new S38HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1 &&
            longReport.Pages.Cast<Page>().All(p => Bricks(p).Any(x => x.Text == "Số dư")) &&
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
