using DevExpress.XtraReports.UI;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports
{
    /// <summary>
    /// Báo cáo Mẫu số 12-CBH:
    /// "Danh sách báo giảm hưởng, tạm dừng hưởng các chế độ BHXH hàng tháng, trợ cấp thất nghiệp"
    /// Thiết kế trực quan theo GroupHeaderBand và DetailReportBand
    /// </summary>
    public partial class BaoCao12CbhReport : XtraReport
    {
        public BaoCao12CbhReport(BaoCao12CbhReportData? data = null)
        {
            InitializeComponent();
            ApplyData(data ?? BaoCao12CbhReportData.CreateSample());
        }

        private void ApplyData(BaoCao12CbhReportData data)
        {
            // Gán trực tiếp dữ liệu nhóm mới (không chia nhánh 2 trường hợp)
            this.DataSource = data.Groups;
            this.DetailReportItems.DataSource = data.Groups;

            // Gán các giá trị Header Parameter
            if (!string.IsNullOrWhiteSpace(data.CoQuanBhxh)) pCoQuanBhxh.Value = data.CoQuanBhxh;
            if (data.Thang > 0) pThang.Value = data.Thang;
            if (data.Nam > 0) pNam.Value = data.Nam;
            if (!string.IsNullOrWhiteSpace(data.BhxhCoSo)) pBhxhCoSo.Value = data.BhxhCoSo;
            if (!string.IsNullOrWhiteSpace(data.XaPhuongToChiTra)) pXaPhuongToChiTra.Value = data.XaPhuongToChiTra;
            if (!string.IsNullOrWhiteSpace(data.HinhThucChiTra)) pHinhThucChiTra.Value = data.HinhThucChiTra;
            if (!string.IsNullOrWhiteSpace(data.NguonKinhPhi)) pNguonKinhPhi.Value = data.NguonKinhPhi;
        }
    }
}
