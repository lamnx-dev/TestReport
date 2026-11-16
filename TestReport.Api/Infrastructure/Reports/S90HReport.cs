using System.Globalization;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public partial class S90HReport : DevExpress.XtraReports.UI.XtraReport
{
    public S90HReport() => InitializeComponent();

    public S90HReport(S90HReportData data)
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
            ["KyBaoCao"] = data.KyBaoCao,
            ["DonViTinh"] = "Đơn vị tính: " + data.DonViTinh,
            ["NgayMoSo"] = "Ngày mở sổ: " + (data.NgayMoSo?.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) ?? "………………."),
            ["NgayKy"] = data.NgayKy?.ToString("'Ngày 'dd' tháng 'MM' năm 'yyyy", CultureInfo.InvariantCulture) ?? "Ngày … tháng … năm …",
            ["ChucDanhKeToan"] = data.ChucDanhKeToan,
            ["ChucDanhKeToanTruong"] = data.ChucDanhKeToanTruong,
            ["ChucDanhThuTruong"] = data.ChucDanhThuTruong,
            ["KeToan"] = data.KeToan,
            ["KeToanTruong"] = data.KeToanTruong,
            ["ThuTruongDonVi"] = data.ThuTruongDonVi
        });
    }
}
