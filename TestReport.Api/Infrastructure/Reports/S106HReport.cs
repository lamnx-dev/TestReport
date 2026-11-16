using System.ComponentModel;
using System.Globalization;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public partial class S106HReport : DevExpress.XtraReports.UI.XtraReport
{
    // Constructor không tham số phục vụ Designer Preview.
    public S106HReport()
    {
        InitializeComponent();
    }

    public S106HReport(S106HReportData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        InitializeComponent();
        DataSource = new[] { new S106HReportDocument(data) };
        DetailReport.DataSource = DataSource;
        DetailReport1.DataSource = DataSource;
        DetailReport2.DataSource = DataSource;
        ApplyTexts(data);
    }

    private void ApplyTexts(S106HReportData data)
    {
        ReportTextApplier.Apply(this, new Dictionary<string, string>
        {
            ["DonViCapTren"] = data.DonViCapTren,
            ["DonViBaoCao"] = data.DonViBaoCao,
            ["KyBaoCao"] = data.KyBaoCao,
            ["NgayMoSo"] = data.NgayMoSo is { } opened
                ? "Ngày mở sổ: " + opened.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)
                : "Ngày mở sổ: ……………….",
            ["NgayKy"] = data.NgayKy is { } signed
                ? signed.ToString("'Ngày 'dd' tháng 'MM' năm 'yyyy", CultureInfo.InvariantCulture)
                : "Ngày … tháng … năm …",
            ["KeToan"] = data.KeToan,
            ["KeToanTruong"] = data.KeToanTruong,
            ["ThuTruongDonVi"] = data.ThuTruongDonVi
        });
    }

    private void GroupHeader5_BeforePrint(object sender, CancelEventArgs e)
    {
        var tenTieuMuc = DetailReport1.GetCurrentColumnValue(nameof(S106HUsedRow.TenTieuMuc))?.ToString() ?? "";
        ReportTextApplier.Apply(this, new Dictionary<string, string> { ["TenTieuMuc"] = tenTieuMuc });
        e.Cancel = string.IsNullOrEmpty(tenTieuMuc);
    }
}
