using DevExpress.XtraReports.UI;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports
{
    /// <summary>
    /// Báo cáo Mẫu số 07/BCNV (Báo cáo tổng hợp lãi thu hồi chi sai).
    /// Toàn bộ bố cục, căn lề, format số tiền và chữ ký được thiết kế 100% bằng KÉO THẢ trên DevExpress Designer.
    /// File code này chỉ làm nhiệm vụ nạp dữ liệu vào DataSource và Parameters.
    /// </summary>
    public partial class BaoCao07BcnvReport : XtraReport
    {
        /// <summary>
        /// Khởi tạo báo cáo:
        /// - Nếu truyền data -> hiển thị theo data truyền vào.
        /// - Nếu không truyền (hoặc truyền null) -> tự động nạp dữ liệu mẫu (Sample) để preview ngay trên Designer / API.
        /// </summary>
        public BaoCao07BcnvReport(BaoCao07BcnvReportData? data = null)
        {
            InitializeComponent();
            ApplyData(data ?? BaoCao07BcnvReportData.CreateSample());
        }

        private void ApplyData(BaoCao07BcnvReportData data)
        {
            // 1. Dữ liệu bảng chi tiết (Gắn với bảng tblDetail kéo thả trên Designer)
            this.DataSource = (data.Items != null && data.Items.Count > 0)
                ? data.Items
                : BaoCao07BcnvItemDto.GetSampleData();

            // 2. Gán giá trị vào Parameters (Các nhãn Header và Footer trên Designer tự động hiển thị qua Expression)
            if (!string.IsNullOrWhiteSpace(data.DonViCapTren)) pDonViCapTren.Value = data.DonViCapTren;
            if (!string.IsNullOrWhiteSpace(data.DonViBaoCao)) pDonViBaoCao.Value = data.DonViBaoCao;
            if (data.Quy > 0) pQuy.Value = data.Quy;
            if (data.Nam > 0) pNam.Value = data.Nam;
            if (!string.IsNullOrWhiteSpace(data.NgayBaoCao)) pNgayBaoCao.Value = data.NgayBaoCao;
            if (!string.IsNullOrWhiteSpace(data.NguoiLap)) pNguoiLap.Value = data.NguoiLap;
            if (!string.IsNullOrWhiteSpace(data.KeToanTruong)) pKeToanTruong.Value = data.KeToanTruong;
            if (!string.IsNullOrWhiteSpace(data.ThuTruongDonVi)) pThuTruongDonVi.Value = data.ThuTruongDonVi;
        }
    }
}
