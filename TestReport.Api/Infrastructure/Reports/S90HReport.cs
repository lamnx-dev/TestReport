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
        lblDonViCapTren.Text = data.DonViCapTren;
        lblDonViBaoCao.Text = data.DonViBaoCao;
        lblCanCu.Text = data.CanCu;
        lblKyBaoCao.Text = data.KyBaoCao;
        lblDonViTinh.Text = "Đơn vị tính: " + data.DonViTinh;
        lblNgayMoSo.Text = "Ngày mở sổ: " + (data.NgayMoSo?.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) ?? "……………….");
        lblNgayKy.Text = data.NgayKy?.ToString("'Ngày 'dd' tháng 'MM' năm 'yyyy", CultureInfo.InvariantCulture) ?? "Ngày … tháng … năm …";
        lblChucDanhKeToan.Text = data.ChucDanhKeToan;
        lblChucDanhKeToanTruong.Text = data.ChucDanhKeToanTruong;
        lblChucDanhThuTruong.Text = data.ChucDanhThuTruong;
        lblTenKeToan.Text = data.KeToan;
        lblTenKeToanTruong.Text = data.KeToanTruong;
        lblTenThuTruong.Text = data.ThuTruongDonVi;
    }
}
