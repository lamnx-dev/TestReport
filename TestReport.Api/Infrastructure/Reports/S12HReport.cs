using System.Globalization;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public partial class S12HReport : DevExpress.XtraReports.UI.XtraReport
{
    public S12HReport()
    {
        InitializeComponent();
    }

    public S12HReport(S12HReportData data)
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
            ["KyBaoCao"] = $"Tháng {data.Thang:00} năm {data.Nam}",
            ["NoiMoTaiKhoan"] = "Nơi mở tài khoản giao dịch: " + data.NoiMoTaiKhoan,
            ["SoTaiKhoan"] = "Số hiệu tài khoản tại nơi gửi: " + data.SoTaiKhoan,
            ["LoaiTienGui"] = "Loại tiền gửi: " + data.LoaiTienGui,
            ["NgayMoSo"] = "- Ngày mở sổ: " + (data.NgayMoSo?.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) ?? "……………………"),
            ["NgayKy"] = data.NgayKy?.ToString("'Ngày 'dd' tháng 'MM' năm 'yyyy", CultureInfo.InvariantCulture) ?? "Ngày ... tháng ... năm ..."
        };
        Signature(texts, "KeToan", data.ChucDanhKeToan, data.KeToan, "(Ký, họ tên)");
        Signature(texts, "KeToanTruong", data.ChucDanhKeToanTruong, data.KeToanTruong, "(Ký, họ tên)");
        Signature(texts, "ThuTruongDonVi", data.ChucDanhThuTruong, data.ThuTruongDonVi, "(Ký, họ tên, đóng dấu)");
        ReportTextApplier.Apply(this, texts);
    }

    private static void Signature(Dictionary<string, string> texts, string tag, string title, string name, string hint)
    {
        var declared = !string.IsNullOrWhiteSpace(title);
        texts["ChucDanh" + tag] = declared ? title : "";
        texts["HuongDanKy" + tag] = declared ? hint : "";
        texts[tag] = declared ? name : "";
    }
}
