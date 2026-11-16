using System.Globalization;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public partial class S62HReport : DevExpress.XtraReports.UI.XtraReport
{
    public S62HReport()
    {
        InitializeComponent();
    }

    public S62HReport(S62HReportData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var rows = data.CreateRows();
        InitializeComponent();
        DataSource = rows;
        var texts = new Dictionary<string, string>
        {
            ["DonViCapTren"] = data.DonViCapTren,
            ["DonViBaoCao"] = data.DonViBaoCao,
            ["CanCu"] = data.CanCu,
            ["KyBaoCao"] = $"Từ ngày {data.TuNgay:dd'/'MM'/'yyyy} đến ngày {data.DenNgay:dd'/'MM'/'yyyy}",
            ["SanPham"] = $"Tên sản phẩm, dịch vụ: {data.MaSanPham} — {data.TenSanPham}",
            ["NgayMoSo"] = "- Ngày mở sổ: " + (data.NgayMoSo?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "……………………"),
            ["NgayKy"] = data.NgayKy?.ToString("'Ngày 'dd' tháng 'MM' năm 'yyyy", CultureInfo.InvariantCulture) ?? "Ngày ... tháng ... năm ...",
            ["KeToan"] = data.KeToan,
            ["KeToanTruong"] = data.KeToanTruong,
            ["ThuTruongDonVi"] = data.ThuTruongDonVi
        };
        ReportTextApplier.Apply(this, texts);
    }
}
