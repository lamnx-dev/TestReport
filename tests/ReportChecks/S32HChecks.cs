using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S32HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S32H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S32HReportData.CreateSample();
        var rows = data.CreateRows();
        Check(rows.Count == 12 && rows[0].SoDu == 24_000_000m && rows[1].PhatSinhNo == 1_000_000m && rows[2] is { SoChungTu: "DC001", SoDu: 25_000_000m }, "opening and adjustment detail balances");
        Check(rows[4] is { SoChungTu: "CP001", TaiKhoanDoiUng: "153", SoDu: 37_000_000m } && rows[6] is { TaiKhoanDoiUng: "611", SoDu: 34_000_000m } && rows[8].SoDu == 31_000_000m, "signed remaining balance after each purchase or allocation");
        Check(rows[^2] is { PhatSinhNo: 12_000_000m, PhatSinhCo: 6_000_000m, SoDu: null } && rows[^1].SoDu == 31_000_000m && rows[5].SoDu is null, "monthly and annual turnover excludes adjustment; closing includes it once");
        data.Items.Reverse();
        var original = data.Items.ToArray();
        Check(data.CreateRows()[4].SoChungTu == "CP001" && data.Items.SequenceEqual(original), "chronological order without mutating inputs");
        var debit = new S32HPosting { NgayGhiSo = new(2026, 1, 5), TaiKhoanNo = "2421", NoLoaiPhanBo = "01", TaiKhoanCo = "153", SoTien = 20 };
        Check(S32HReportData.CacLoaiPhanBo.Count == 5, "five allocation types from group 123");
        foreach (var code in S32HReportData.CacLoaiPhanBo.Keys)
        {
            data = new() { MaLoaiPhanBo = code, Items = [debit with { NoLoaiPhanBo = code },
                debit with { TaiKhoanNo = "611", TaiKhoanCo = "2422", CoLoaiPhanBo = code, SoTien = 5 }] };
            Check(data.CreateRows()[^2] is { PhatSinhNo: 20, PhatSinhCo: 5 } && data.CreateRows()[^1].SoDu == 15 &&
                data.TenLoaiPhanBo == S32HReportData.CacLoaiPhanBo[code], "debit and credit classification and label for allocation " + code);
        }
        data = new() { Items = [debit with { MaDonVi = "OTHER" }, debit with { NoLoaiPhanBo = "02" }, debit with { TaiKhoanNo = "241" },
            debit with { NgayGhiSo = new(2025, 1, 1) }, debit with { NgayGhiSo = new(2026, 4, 1) },
            debit with { TaiKhoanNo = "611", TaiKhoanCo = "2421", CoLoaiPhanBo = "02" }] };
        Check(data.CreateRows()[^2] is { PhatSinhNo: 0, PhatSinhCo: 0 }, "unit, account, allocation type and year/period filters");
        data = new() { Items = [debit with { TaiKhoanCo = "2422", CoLoaiPhanBo = "02", SoTien = 7 }] };
        Check(data.CreateRows()[^2] is { PhatSinhNo: 7, PhatSinhCo: 0 }, "transfer debit allocation matched independently");
        data.MaLoaiPhanBo = "02";
        Check(data.CreateRows()[^2] is { PhatSinhNo: 0, PhatSinhCo: 7 } && data.CreateRows()[^1].SoDu == -7, "transfer credit allocation and negative remaining balance preserved");
        data = new() { Items = [debit with { TaiKhoanCo = "2422", CoLoaiPhanBo = "01" }] };
        rows = data.CreateRows();
        Check(rows[3].SoDu is null && rows[4].SoDu == 0 && rows[^2] is { PhatSinhNo: 20, PhatSinhCo: 20 }, "both matching sides retained with one final balance");
        data = new() { TuNgay = new(2026, 2, 1), SoDuDauNam = 100, Items =
        [
            debit, debit with { MaLoai = 16, TaiKhoanNo = "421", TaiKhoanCo = "2421", CoLoaiPhanBo = "01", SoTien = 10 },
            debit with { NgayGhiSo = new(2026, 2, 1), MaLoai = 16, SoTien = 5 },
            debit with { NgayGhiSo = new(2026, 2, 10), TaiKhoanNo = "611", TaiKhoanCo = "2421", CoLoaiPhanBo = "01", SoTien = 15 }
        ] };
        rows = data.CreateRows();
        Check(rows[0].SoDu == 110 && rows[2].SoDu == 115 && rows[^2] is { PhatSinhNo: 20, PhatSinhCo: 15 } && rows[^1].SoDu == 100,
            "opening includes prior movements; adjustments excluded from year-to-date and counted once in closing");
        data = new() { Items = [debit, debit with { NgayGhiSo = new(2026, 3, 1), MaLoai = 16, SoTien = 5 }] };
        Check(data.CreateRows()[4].SoDu == 20, "later adjustment does not alter earlier transaction balance");
        Check(new S32HReportData().CreateRows().Count == 8 && new S32HReportData { DenNgay = new(2026, 12, 31) }.CreateRows().Count == 17, "empty quarter/year retains monthly totals");
        foreach (var invalid in new S32HReportData[]
        {
            new() { MaLoaiPhanBo = "06" }, new() { MaLoaiPhanBo = "" }, new() { MaDonVi = "" }, new() { TuNgay = new(2027, 1, 1) },
            new() { DenNgay = new(2027, 1, 1) }, new() { Items = null! }, new() { Items = [null!] },
            new() { Items = [debit with { NgayGhiSo = default }] }, new() { Items = [debit with { TaiKhoanNo = "" }] }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid period, allocation type or posting rejected");
        }
        using (var designer = new S32HReport())
        {
            Check(designer.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource, "designer exposes data source");
            Check(Text(designer).Contains("PB003"), "designer preview has data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var reloaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S32HReportData));
            try { reloaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S32HReportData)); }
            Check(reloaded.FindControl("tblBodyCell8", true) is XRTableCell { ExpressionBindings.Count: 2 }, "designer retains all nine columns and bindings");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s32h")!;
        Check(report is S32HReport && report.Landscape, "factory returns landscape S32H");
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        Check(report.Pages.Count == 1, "sample fits one page");
        var content = Text(report);
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text == "Số dư (Số còn phải phân bổ)") &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Count(x => x.Text == "Nợ") == 1 &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Count(x => x.Text == "Có") == 1, "movement and remaining balance headings printed");
        Check(content.Contains("SỔ THEO DÕI CHI PHÍ TRẢ TRƯỚC") && content.Contains("Loại phân bổ: 01 - Công cụ, dụng cụ") && content.Contains("Nguyễn Văn An") && content.Contains("Quý 1/2026"), "header and signatures populated");
        Check(content.Contains("PB003") && content.Contains("31,000,000") && content.Contains("Cộng lũy kế từ đầu năm") && content.Contains("Công cụ CCDC01"), "transactions, notes and totals printed");
        Check(!content.Contains("CBX") && !content.Contains("mã loại") && !content.Contains("sodu"), "analysis notes excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "title appearance applied");
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Số dư cuối kỳ") && x.Font.Bold), "totals remain bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S32H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s32h.png"), new ImageExportOptions
            { Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 130,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1" });
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        using (var empty = new S32HReport(new()))
            Check(!Text(empty).Contains("CP001") && empty.Pages.Count == 1, "empty report never loads sample transactions");
        Check(System.Text.Json.JsonSerializer.Serialize(((OkObjectResult)controller.Catalog()).Value).Contains("s32h"), "catalog exposes S32H to frontend");
        Check(controller.Render("s32h", new()) is FileContentResult { ContentType: "application/pdf", FileDownloadName: "S32H.pdf" }, "API renders S32H");
        data = S32HReportData.CreateSample();
        for (var i = 0; i < 110; i++) data.Items.Add(data.Items[1] with { SoChungTu = $"LONG-{i}", DienGiai = "Chi phí trả trước " + new string('x', 80) });
        using var longReport = new S32HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1, "long report paginates");
        Check(Bricks(longReport.Pages[1]).Any(x => x.Text == "Số chi phí đã phân bổ cho từng kỳ"), "headers repeat");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S32H checks passed.");
    }

    private static IEnumerable<TextBrick> Bricks(BrickBase brick)
    {
        if (brick is TextBrick text) yield return text;
        System.Collections.IEnumerable? children = brick switch
        { Page p => p.InnerBricks, CompositeBrick c => c.InnerBricks, Brick b => b.Bricks, _ => null };
        if (children is null) yield break;
        foreach (BrickBase child in children)
            foreach (var item in Bricks(child)) yield return item;
    }
}
