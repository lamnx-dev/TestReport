using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S33HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S33H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        var data = S33HReportData.CreateSample();
        var rows = data.CreateRows();
        Check(rows.Count == 11 && rows[0] is { No136: 20_000_000m, Co136: 0, No336: 0, Co336: 12_000_000m }, "separate opening balances for accounts 136 and 336");
        Check(rows[1].No136 == 2_000_000m && rows[2].SoChungTu == "DC001" && rows[^2] is { No136: 30_000_000m, Co136: 10_000_000m, No336: 0, Co336: 15_000_000m }, "adjustment details printed without entering normal turnover");
        Check(rows[^1] is { No136: 42_000_000m, Co136: 0, No336: 0, Co336: 27_000_000m }, "closing reconciles both account groups including adjustments");
        data.Items.Reverse();
        var original = data.Items.ToArray();
        Check(data.CreateRows()[3].SoChungTu == "NB001" && data.Items.SequenceEqual(original), "posting order without source mutation");
        var debit = new S33HPosting { NgayGhiSo = new(2026, 1, 10), TaiKhoanNo = "1361", NoDonViNoiBo = "001", TaiKhoanCo = "112", SoTien = 5 };
        data = new() { Items = [debit,
            debit with { TaiKhoanNo = "112", TaiKhoanCo = "1362", CoDonViNoiBo = "001", SoTien = 7 },
            debit with { TaiKhoanNo = "3361", SoTien = 11 },
            debit with { TaiKhoanNo = "611", TaiKhoanCo = "3362", CoDonViNoiBo = "001", SoTien = 13 }] };
        rows = data.CreateRows();
        Check(rows[^2] is { No136: 5, Co136: 7, No336: 11, Co336: 13 } && rows[^1] is { No136: 0, Co136: 2, No336: 0, Co336: 2 }, "four movement columns, descendants and credit net balances");
        Check(rows[2].TaiKhoanDoiUng == "112" && rows[3].TaiKhoanDoiUng == "112" && rows[5].TaiKhoanDoiUng == "611", "counterpart taken from opposite account side");
        data = new() { Items = [debit with { NoDonViNoiBo = "OTHER" }, debit with { MaDonVi = "OTHER" },
            debit with { TaiKhoanNo = "1381" }, debit with { NgayGhiSo = new(2025, 1, 1) }, debit with { NgayGhiSo = new(2026, 4, 1) },
            debit with { TaiKhoanNo = "112", TaiKhoanCo = "3361", CoDonViNoiBo = "OTHER" }] };
        Check(data.CreateRows()[^2] is { No136: 0, Co136: 0, No336: 0, Co336: 0 }, "reporting unit, internal unit, account and period filters");
        data = new() { Items = [debit with { TaiKhoanCo = "3361", CoDonViNoiBo = "001", SoTien = 10 }] };
        rows = data.CreateRows();
        Check(rows.Count(x => x.NgayGhiSo is not null) == 2 && rows[^2] is { No136: 10, Co336: 10 } && rows[2].TaiKhoanDoiUng == "3361" && rows[3].TaiKhoanDoiUng == "1361", "inter-group transfers preserve each matching side and counterpart");
        data.Items = [debit with { TaiKhoanCo = "3361", NoDonViNoiBo = "OTHER", CoDonViNoiBo = "001", SoTien = 10 }];
        Check(data.CreateRows()[^2] is { No136: 0, Co336: 10 }, "internal-unit dimension applied independently to each side");
        data = new() { TuNgay = new(2026, 2, 1), No136DauNam = 100, Co136DauNam = 20, Items =
        [
            debit with { SoTien = 10 }, debit with { TaiKhoanNo = "3361", MaLoai = 16 },
            debit with { NgayGhiSo = new(2026, 2, 1), TaiKhoanNo = "112", TaiKhoanCo = "1361", CoDonViNoiBo = "001", MaLoai = 16, SoTien = 2, SoChungTu = "ADJ" },
            debit with { NgayGhiSo = new(2026, 2, 10), TaiKhoanNo = "3361", SoTien = 7 }
        ] };
        rows = data.CreateRows();
        Check(rows[0] is { No136: 90, No336: 5 } && rows[1].Co136 == 2 && rows[2].SoChungTu == "ADJ" && rows[^2] is { No136: 10, Co136: 0, No336: 7 } && rows[^1] is { No136: 88, No336: 12 }, "prior movements, current adjustment details, year-to-date and closing use correct scopes");
        data = new() { No136DauNam = 5, Co136DauNam = 10, No336DauNam = 20, Co336DauNam = 5 };
        Check(data.CreateRows()[0] is { No136: 0, Co136: 5, No336: 15, Co336: 0 }, "reverse balances supported for either account group");
        data = new() { Items = [debit, debit with { SoTien = 2.5m }] };
        Check(data.CreateRows().Count(x => x.NgayGhiSo is not null) == 2 && data.CreateRows()[^2].No136 == 7.5m, "separate detail rows and decimal amounts preserved");
        Check(new S33HReportData().CreateRows().Count == 7, "empty quarter retains monthly totals");
        Check(new S33HReportData { DenNgay = new(2026, 12, 31) }.CreateRows().Count == 16, "empty year retains twelve months");
        foreach (var invalid in new S33HReportData[]
        {
            new() { MaDonViNoiBo = "" }, new() { MaDonVi = "" }, new() { TuNgay = new(2027, 1, 1) }, new() { DenNgay = new(2027, 1, 1) },
            new() { Items = null! }, new() { Items = [null!] }, new() { Items = [debit with { NgayGhiSo = default }] }, new() { Items = [debit with { TaiKhoanNo = "" }] }
        })
        {
            var rejected = false;
            try { invalid.CreateRows(); } catch (ArgumentException) { rejected = true; }
            Check(rejected, "invalid period, unit or posting rejected");
        }
        using (var designer = new S33HReport())
        {
            Check(designer.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource, "designer exposes data source");
            Check(Text(designer).Contains("NB003"), "designer preview has data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var reloaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S33HReportData));
            try { reloaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S33HReportData)); }
            Check(reloaded.FindControl("tblBodyCell9", true) is XRTableCell { ExpressionBindings.Count: 2 }, "designer retains all ten columns and bindings");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s33h")!;
        Check(report is S33HReport && report.Landscape, "factory returns landscape S33H");
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        Check(report.Pages.Count == 1, "sample fits one page");
        var content = Text(report);
        Check(content.Contains("PHẢI THU, PHẢI TRẢ NỘI BỘ") && content.Contains("Đơn vị: 001") && content.Contains("136, 336") && content.Contains("Nguyễn Văn An") && content.Contains("Quý 1/2026"), "header and signatures populated");
        Check(content.Contains("NB003") && content.Contains("42,000,000") && content.Contains("Cộng lũy kế từ đầu năm") && content.Contains("Chi hộ theo đề nghị 01"), "transactions, notes and totals printed");
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text == "Phải thu nội bộ") &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text == "Phải trả nội bộ") &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Count(x => x.Text == "Nợ") == 2 &&
            report.Pages.Cast<Page>().SelectMany(Bricks).Count(x => x.Text == "Có") == 2, "all four movement column headings printed");
        Check(!content.Contains("CBX") && !content.Contains("mã loại") && !content.Contains("sodu"), "analysis notes excluded");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "title appearance applied");
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Số dư cuối kỳ") && x.Font.Bold), "totals remain bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S33H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s33h.png"), new ImageExportOptions
            { Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 130,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1" });
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        using (var empty = new S33HReport(new()))
            Check(!Text(empty).Contains("NB001") && empty.Pages.Count == 1, "empty report never loads sample transactions");
        Check(System.Text.Json.JsonSerializer.Serialize(((OkObjectResult)controller.Catalog()).Value).Contains("s33h"), "catalog exposes S33H to frontend");
        Check(controller.Render("s33h", new()) is FileContentResult { ContentType: "application/pdf", FileDownloadName: "S33H.pdf" }, "API renders S33H");
        data = S33HReportData.CreateSample();
        for (var i = 0; i < 110; i++) data.Items.Add(data.Items[1] with { SoChungTu = $"LONG-{i}", DienGiai = "Thanh toán nội bộ " + new string('x', 80) });
        using var longReport = new S33HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1, "long report paginates");
        Check(Bricks(longReport.Pages[1]).Any(x => x.Text == "Phải thu nội bộ") && Bricks(longReport.Pages[1]).Any(x => x.Text == "Phải trả nội bộ"), "headers repeat");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S33H checks passed.");
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
