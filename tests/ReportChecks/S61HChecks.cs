using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TestReport.Api.Controllers;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

internal static class S61HChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS S61H " + message);
        }
        static string Text(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.ExportToText(stream, new TextExportOptions { Encoding = Encoding.UTF8 });
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        static S61HRow Total(S61HReportData data) => data.CreateRows().Single(x => x.DienGiai == "Cộng số phát sinh");
        var data = S61HReportData.CreateSample();
        Check(Total(data).TongNo == 73_000_000m && Total(data).TongCo == 1_000_000m, "normal totals exclude transfer to 911");
        Check(data.CreateRows()[^1] is { TaiKhoanDoiUng: "911", CoLuong: 50_000_000m }, "transfer retained after totals");
        var original = data.Items.ToArray();
        data.TuNgay = new(2026, 3, 1);
        Check(Total(data).TongNo == 3_000_000m, "period filters posting date");
        Check(data.CreateRows().Single(x => x.DienGiai == "Cộng lũy kế từ đầu năm").TongNo == 73_000_000m, "year to date includes earlier months");
        Check(data.Items.SequenceEqual(original), "source entries unchanged");
        foreach (var account in new[] { "611", "612", "642" })
        {
            data = new() { TaiKhoan = account };
            foreach (var group in new[] { '1', '2', '3', '8' })
            {
                data.Items.Add(new() { NgayGhiSo = new(2026, 1, 5), TaiKhoanNo = account + group, TaiKhoanCo = "334", SoTien = 100 });
                data.Items.Add(new() { NgayGhiSo = new(2026, 1, 6), TaiKhoanNo = "112", TaiKhoanCo = account + group, SoTien = 10 });
            }
            var total = Total(data);
            Check(total is { NoLuong: 100, NoVatTu: 100, NoKhauHao: 100, NoKhac: 100,
                CoLuong: 10, CoVatTu: 10, CoKhauHao: 10, CoKhac: 10 }, "four debit and credit groups for " + account);
        }
        data = new();
        var posting = new S61HPosting { NgayGhiSo = new(2026, 1, 1), TaiKhoanNo = "6111", TaiKhoanCo = "1361", SoTien = 100 };
        data.Items = [posting, posting with { TaiKhoanCo = "5151" },
            posting with { MaDonVi = "HUYEN", TaiKhoanCo = "334" },
            posting with { MaDonVi = "NGOAI_TINH", TaiKhoanCo = "334" },
            posting with { NgayGhiSo = new(2025, 12, 31), TaiKhoanCo = "334" },
            posting with { NgayGhiSo = new(2026, 4, 1), TaiKhoanCo = "334" }];
        Check(Total(data).TongNo == 0, "office excludes 136, 515, other units and dates");
        data.ToanTinh = true;
        Check(Total(data).TongNo == 300, "province includes 136 and 515 and scoped subordinate units");
        data = new() { Items = [posting with { TaiKhoanCo = "334" }, posting with { TaiKhoanCo = "334" }] };
        Check(data.CreateRows().Count(x => x.TaiKhoanDoiUng == "334") == 2, "compound postings remain separate rows");
        data = new();
        Check(data.CreateRows().Count == 3, "empty period retains totals and transfer heading");
        data.TaiKhoan = "154";
        var rejected = false;
        try { data.CreateRows(); } catch (ArgumentException) { rejected = true; }
        Check(rejected, "unsupported account rejected");
        data = new() { TuNgay = new(2025, 12, 1) };
        rejected = false;
        try { data.CreateRows(); } catch (ArgumentException) { rejected = true; }
        Check(rejected, "cross year period rejected");

        using (var designer = new S61HReport())
        {
            Check(designer.DataSource is DevExpress.DataAccess.ObjectBinding.ObjectDataSource, "designer exposes data source");
            Check(Text(designer).Contains("VT002"), "designer preview has data");
            using var layout = new MemoryStream();
            designer.SaveLayoutToXml(layout);
            layout.Position = 0;
            using var reloaded = new XtraReport();
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(S61HReportData));
            try { reloaded.LoadLayoutFromXml(layout); }
            finally { DevExpress.Utils.DeserializationSettings.UnregisterTrustedClass(typeof(S61HReportData)); }
            Check(reloaded.FindControl("tblBodyCell14", true) is XRTableCell { ExpressionBindings.Count: 2 }, "designer retains all 15 columns and bindings");
        }
        var factory = new SampleReportFactory();
        var renderer = new ReportPdfRenderer();
        using var report = factory.Create("s61h")!;
        Check(report is S61HReport && report.Landscape, "factory returns landscape S61H");
        ReportAppearanceApplier.Apply(report, new() { Title = new() { FontSize = 16 }, TableBody = new() { Bold = false } });
        report.CreateDocument();
        Check(report.Pages.Count == 1, "sample fits one page");
        var content = Text(report);
        Check(content.Contains("SỔ CHI TIẾT CHI PHÍ") && content.Contains("Tài khoản: 611") && content.Contains("Nguyễn Văn An"), "header and signatures populated");
        Check(content.Contains("VT002") && content.Contains("KC003") && content.Contains("Cộng lũy kế từ đầu năm"), "transactions and totals printed");
        Check(!content.Contains("Ng?y") && !content.Contains("id_dkhoan") && !content.Contains("Số dư cuối kỳ"), "report excludes analysis text and unrelated balance rows");
        Check(report.FindControl("lblTitle", true).Font.SizeInPoints == 16, "title appearance applied");
        Check(report.Pages.Cast<Page>().SelectMany(Bricks).Any(x => x.Text.Contains("Cộng số phát sinh") && x.Font.Bold), "totals remain bold");
        var pdf = renderer.Render(report);
        Check(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF exported");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "S61H.pdf"), pdf);
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            report.ExportToImage(Path.Combine(AppContext.BaseDirectory, "s61h.png"), new ImageExportOptions
            { Format = System.Drawing.Imaging.ImageFormat.Png, Resolution = 130,
                ExportMode = ImageExportMode.SingleFilePageByPage, PageRange = "1" });
        var controller = new ReportsController(factory, renderer, NullLogger<ReportsController>.Instance);
        Check(controller.Render("s61h", new()) is FileContentResult { ContentType: "application/pdf", FileDownloadName: "S61H.pdf" }, "API renders S61H");
        data = S61HReportData.CreateSample();
        for (var i = 0; i < 110; i++) data.Items.Add(data.Items[0] with { SoChungTu = $"LONG-{i}", DienGiai = "Chi phí hoạt động " + new string('x', 80) });
        using var longReport = new S61HReport(data);
        longReport.CreateDocument();
        Check(longReport.Pages.Count > 1, "long report paginates");
        Check(Bricks(longReport.Pages[1]).Any(x => x.Text == "Ghi Có tài khoản"), "headers repeat");
        Check(longReport.Pages.Cast<Page>().Count(p => Bricks(p).Any(x => x.Text == "THỦ TRƯỞNG ĐƠN VỊ")) == 1, "signature printed once");
        Console.WriteLine("All S61H checks passed.");
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
