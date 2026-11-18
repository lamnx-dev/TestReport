using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S53HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S53H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S53HReportData.CreateSample();
        data.Items.Reverse();
        var original = data.Items.ToArray();
        var rows = data.CreateRows();
        Check(rows[3].SoChungTu == "GT001" && data.Items.SequenceEqual(original), "chronological rows without changing input");
        Check(rows[1].PhaiNop == 500_000m && rows[^2].DaNop == 18_000_000m && rows[^2].PhaiNop == 18_000_000m, "type 16 excluded from normal totals");
        Check(rows[^1].PhaiNop == 5_500_000m && rows[^1].DaNop == 0, "credit closing includes opening and adjustment");
        Check(rows.Count(x => x.DienGiai.StartsWith("Cộng phát sinh tháng")) == 3 && rows[^3].DaNop == 18_000_000m, "quarter has monthly and period totals");
        var empty = new S53HReportData();
        Check(empty.CreateRows().Count == 9 && empty.CreateRows()[^1].PhaiNop == 0, "empty quarter retains all monthly totals");
        foreach (var invalid in new S53HReportData[]
        {
            new() { TaiKhoan = "1331" }, new() { TaiKhoan = "3331x" }, new() { Items = null! }, new() { Items = [null!] },
            new() { Items = [new()] }, new() { Items = [new() { NgayGhiSo = new(2026, 1, 1), TaiKhoan = "" }] },
            new() { TuNgay = new(2027, 1, 1) }, new() { DenNgay = new(2027, 1, 1) },
            new() { TuNgay = new(2026, 1, 2) }, new() { DenNgay = new(2026, 3, 30) },
            new() { DenNgay = new(2026, 2, 28) }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid account, period or transactions rejected");
        }
        data.TuNgay = new(2026, 2, 1);
        data.DenNgay = new(2026, 2, 28);
        data.CoDauKy = 9_500_000m;
        data.Items.Add(new() { NgayGhiSo = new(2026, 2, 5), TaiKhoan = "1331", PhatSinhNo = 999m });
        data.Items.Add(new() { NgayGhiSo = new(2025, 2, 5), PhatSinhNo = 999m });
        data.Items.Add(new() { NgayGhiSo = new(2026, 2, 1), MaLoai = 16, PhatSinhNo = 100_000m, PhatSinhCo = 200_000m });
        rows = data.CreateRows();
        Check(rows[3].SoChungTu == "GT003" && rows[^3].PhaiNop == 6_000_000m && rows[^2].DaNop == 8_000_000m && rows[^2].PhaiNop == 18_000_000m, "period, account and year-to-date filtering");
        Check(rows[0].PhaiNop == 9_500_000m && rows[1].DaNop == 100_000m && rows[1].PhaiNop == 200_000m && rows[^1].PhaiNop == 15_600_000m, "period opening and adjustments are counted once");
        Check(!rows.Any(x => x.DienGiai == "Cộng phát sinh trong kỳ") && data.KyBaoCao == "Tháng: 02/2026", "month omits period total");
        data.TuNgay = new(2026, 1, 1);
        data.DenNgay = new(2026, 12, 31);
        rows = data.CreateRows();
        Check(rows.Count(x => x.DienGiai.StartsWith("Cộng phát sinh tháng")) == 12 && !rows.Any(x => x.DienGiai == "Cộng phát sinh trong kỳ"), "year has twelve month totals and no period total");
        var debit = new S53HReportData { NoDauKy = 10, Items = [new() { NgayGhiSo = new(2026, 1, 1), MaLoai = 16, PhatSinhNo = 2 }] }.CreateRows();
        Check(debit[0].DaNop == 10 && debit[^1].DaNop == 12 && debit[^1].PhaiNop == 0, "debit balances print in first amount column");
        data.TaiKhoan = "33312";
        Check(data.CreateRows()[^2].DaNop == 0 && data.CreateRows()[^2].PhaiNop == 0, "child account excludes sibling account");
        var lastYear = new S53HReportData { TuNgay = new(9999, 12, 1), DenNgay = new(9999, 12, 31) };
        Check(lastYear.CreateRows().Count == 6, "last supported calendar month does not overflow");
        data = S53HReportData.CreateSample();
        using (var designer = new S53HReport())
        {
            Check(Text(designer).Contains("GT001"), "designer preview uses sample data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var loaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S53HReportData));
            try { loaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S53HReportData)); }
            Check(loaded.FindControl("lblKyBaoCao", true).Tag?.ToString() == "KyBaoCao" &&
                loaded.FindControl("tblBodyCell4", true) is XRTableCell { ExpressionBindings.Count: 2 }, "layout retains tags and bindings");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s53h")!;
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        var content = Text(report);
        Check(!report.Landscape && report.Pages.Count == 1, "sample fits A4 portrait page");
        Check(content.Contains("GT004") && content.Contains("Nguyễn Văn An") && content.Contains("Quý: 1 - Năm: 2026"), "transactions and tagged labels printed");
        Check(content.Contains("SỔ THEO DÕI THUẾ GIÁ TRỊ GIA TĂNG") && content.Contains("Tài khoản: 3331") && !content.Contains("HOÀN LẠI"), "tax title and selected account printed");
        Check(!content.Contains("3331%") && !content.Contains("mã loại") && !content.Contains("CBX"), "analysis notes excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16 &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Số dư cuối kỳ") && x.Font.Bold), "style applies while totals stay bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S53H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s53h.png"), new ImageExportOptions
            {
                Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 110,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1"
            });
        using (var blank = new S53HReport(empty))
            Check(!Text(blank).Contains("GT001") && blank.Pages.Count == 1, "empty report does not load sample");
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        var catalog = (OkObjectResult)controller.Catalog();
        Check(System.Text.Json.JsonSerializer.Serialize(catalog.Value).Contains("s53h"), "catalog includes S53H for frontend selection");
        Check(controller.Render("s53h", new()) is FileContentResult { FileDownloadName: "S53H.pdf", ContentType: "application/pdf" }, "render endpoint supports S53H");
        var first = data.Items[1];
        for (var i = 0; i < 120; i++) data.Items.Add(first with { SoChungTu = $"LONG-{i}" });
        using var longReport = new S53HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1 && Bricks(longReport.Pages[1]).Any(x => x.Text.Contains("Chứng từ")), "column headings repeat across pages");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S53H checks passed.");
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
