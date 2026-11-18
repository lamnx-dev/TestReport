using System.Globalization;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public partial class S52HReport : DevExpress.XtraReports.UI.XtraReport
{
    public S52HReport()
    {
        InitializeComponent();
    }

    public S52HReport(S52HReportData data)
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
            ["LoaiHoatDong"] = $"Loại hoạt động: {data.MaLoaiHoatDong} - {data.TenLoaiHoatDong}",
            ["SanPham"] = $"Tên sản phẩm, hàng hóa, dịch vụ: {data.MaSanPham} - {data.TenSanPham}",
            ["KyBaoCao"] = data.KyBaoCao,
            ["NgayMoSo"] = "- Ngày mở sổ: " + (data.NgayMoSo?.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) ?? "……………………"),
            ["NgayKy"] = data.NgayKy?.ToString("'Ngày 'dd' tháng 'MM' năm 'yyyy", CultureInfo.InvariantCulture) ?? "Ngày ... tháng ... năm ...",
            ["KeToan"] = data.KeToan,
            ["KeToanTruong"] = data.KeToanTruong,
            ["ThuTruongDonVi"] = data.ThuTruongDonVi
        });
    }
}
