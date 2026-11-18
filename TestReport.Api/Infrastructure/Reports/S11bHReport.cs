using System.Globalization;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public partial class S11bHReport : DevExpress.XtraReports.UI.XtraReport
{
    public S11bHReport()
    {
        InitializeComponent();
    }

    public S11bHReport(S11bHReportData data)
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
            ["NgayMoSo"] = "- Ngày mở sổ: " + (data.NgayMoSo?.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) ?? "……………………"),
            ["NgayKy"] = data.NgayKy?.ToString("'Ngày 'dd' tháng 'MM' năm 'yyyy", CultureInfo.InvariantCulture) ?? "Ngày ... tháng ... năm ..."
        };
        Signature(texts, "ThuQuy", data.ChucDanhThuQuy, data.ThuQuy, "(Ký, họ tên)");
        Signature(texts, "KeToanTruong", data.ChucDanhKeToanTruong, data.KeToanTruong, "(Ký, họ tên)");
        Signature(texts, "ThuTruongDonVi", data.ChucDanhThuTruongDonVi, data.ThuTruongDonVi, "(Ký, họ tên, đóng dấu)");
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
