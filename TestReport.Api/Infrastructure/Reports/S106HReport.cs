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
        ApplyLabels(data);
    }

    private void ApplyLabels(S106HReportData data)
    {
        xrLabel1.Text = data.DonViCapTren;
        xrLabel2.Text = data.DonViBaoCao;
        xrLabel6.Text = data.KyBaoCao;
        xrLabel7.Text = data.NgayMoSo is { } opened
            ? "Ngày mở sổ: " + opened.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)
            : "Ngày mở sổ: ……………….";
        xrLabel8.Text = data.NgayKy is { } signed
            ? signed.ToString("'Ngày 'dd' tháng 'MM' năm 'yyyy", CultureInfo.InvariantCulture)
            : "Ngày … tháng … năm …";
        xrLabel11.Text = data.KeToan;
        xrLabel14.Text = data.KeToanTruong;
        xrLabel17.Text = data.ThuTruongDonVi;
    }

    private void GroupHeader5_BeforePrint(object sender, CancelEventArgs e)
    {
        xrLabel20.Text = DetailReport1.GetCurrentColumnValue(nameof(S106HUsedRow.TenTieuMuc))?.ToString() ?? "";
        e.Cancel = string.IsNullOrEmpty(xrLabel20.Text);
    }
}
