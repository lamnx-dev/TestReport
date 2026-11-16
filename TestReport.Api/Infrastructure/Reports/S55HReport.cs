using System.Globalization;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public partial class S55HReport : DevExpress.XtraReports.UI.XtraReport
{
    public S55HReport()
    {
        InitializeComponent();
    }

    public S55HReport(S55HReportData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var rows = data.CreateRows();
        InitializeComponent();
        DataSource = rows;
        ReportTextApplier.Apply(this, new Dictionary<string, string>
        {
            ["DonViCapTren"] = data.DonViCapTren,
            ["DonViBaoCao"] = data.DonViBaoCao,
            ["CanCu"] = data.CanCu,
            ["KyBaoCao"] = $"Năm: {data.Nam}",
            ["NgayMoSo"] = "- Ngày mở sổ: " + (data.NgayMoSo?.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) ?? "……………………"),
            ["NgayKy"] = data.NgayKy?.ToString("'Ngày 'dd' tháng 'MM' năm 'yyyy", CultureInfo.InvariantCulture) ?? "Ngày ... tháng ... năm ...",
            ["KeToan"] = data.KeToan,
            ["KeToanTruong"] = data.KeToanTruong,
            ["ThuTruongDonVi"] = data.ThuTruongDonVi
        });
    }
}
