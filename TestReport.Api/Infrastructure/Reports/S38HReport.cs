using System.Globalization;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public partial class S38HReport : DevExpress.XtraReports.UI.XtraReport
{
    public S38HReport()
    {
        InitializeComponent();
    }

    public S38HReport(S38HReportData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var rows = data.CreateRows();
        InitializeComponent();
        DataSource = rows;
        var account = string.IsNullOrWhiteSpace(data.TenTaiKhoan)
            ? data.TaiKhoan : $"{data.TaiKhoan} - {data.TenTaiKhoan}";
        ReportTextApplier.Apply(this, new Dictionary<string, string>
        {
            ["DonViCapTren"] = data.DonViCapTren,
            ["DonViBaoCao"] = data.DonViBaoCao,
            ["CanCu"] = data.CanCu,
            ["ReportTitle"] = $"SỔ CHI TIẾT TÀI KHOẢN: {account}",
            ["KyBaoCao"] = data.KyBaoCao,
            ["NgayKy"] = data.NgayKy?.ToString("'Ngày 'dd' tháng 'MM' năm 'yyyy", CultureInfo.InvariantCulture) ?? "Ngày ... tháng ... năm ...",
            ["KeToan"] = data.KeToan,
            ["KeToanTruong"] = data.KeToanTruong,
            ["ThuTruongDonVi"] = data.ThuTruongDonVi
        });
    }
}
