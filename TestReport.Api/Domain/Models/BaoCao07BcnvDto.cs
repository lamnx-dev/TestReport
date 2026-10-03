using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TestReport.Api.Domain.Models
{
    /// <summary>
    /// Model đóng gói TOÀN BỘ dữ liệu báo cáo 07/BCNV (Header + Footer + Danh sách Items)
    /// Khi gọi từ API chỉ cần: var report = new BaoCao07BcnvReport(reportData);
    /// </summary>
    public class BaoCao07BcnvReportData
    {
        // 1. Đơn vị quản lý cấp trên
        public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
        public string DonViQuanLy { get => DonViCapTren; set => DonViCapTren = value; }

        // 2. Đơn vị báo cáo
        public string DonViBaoCao { get; set; } = "BHXH THÀNH PHỐ HÀ NỘI";

        // 3. Kỳ báo cáo & Ngày báo cáo
        public int Quy { get; set; } = 1;
        public int Nam { get; set; } = 2026;
        public string NgayBaoCao { get; set; } = $"Ngày 31 tháng 03 năm 2026";

        // 4. Khối chữ ký
        public string NguoiLap { get; set; } = "Nguyễn Văn A";
        public string NguoiLapPhieu { get => NguoiLap; set => NguoiLap = value; }

        public string KeToanTruong { get; set; } = "Trần Thị B";

        public string ThuTruongDonVi { get; set; } = "Lê Hoàng C";
        public string ThuTruong { get => ThuTruongDonVi; set => ThuTruongDonVi = value; }

        // 5. Mảng data bảng chi tiết
        public List<BaoCao07BcnvItemDto> Items { get; set; } = new();
        public List<BaoCao07BcnvItemDto> Data { get => Items; set => Items = value; }

        public static BaoCao07BcnvReportData CreateSample()
        {
            return new BaoCao07BcnvReportData
            {
                DonViCapTren = "BẢO HIỂM XÃ HỘI VIỆT NAM",
                DonViBaoCao = "BHXH THÀNH PHỐ HÀ NỘI",
                Quy = 1,
                Nam = 2026,
                NgayBaoCao = "Ngày 31 tháng 03 năm 2026",
                NguoiLap = "Nguyễn Văn A",
                KeToanTruong = "Trần Thị B",
                ThuTruongDonVi = "Lê Hoàng C",
                Items = BaoCao07BcnvItemDto.GetSampleData()
            };
        }
    }

    /// <summary>
    /// DTO chứa thông tin tiêu đề, kỳ báo cáo và chữ ký của Mẫu số 07/BCNV
    /// </summary>
    public class BaoCao07BcnvHeaderDto
    {
        public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
        public string DonViBaoCao { get; set; } = "BHXH TỈNH / THÀNH PHỐ...";
        public int Quy { get; set; } = 1;
        public int Nam { get; set; } = 2026;
        public string NgayBaoCao { get; set; } = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
        public string NguoiLap { get; set; } = "";
        public string KeToanTruong { get; set; } = "";
        public string ThuTruongDonVi { get; set; } = "";
    }

    /// <summary>
    /// DTO đại diện cho một dòng chỉ tiêu trong bảng Mẫu số 07/BCNV
    /// (Báo cáo tổng hợp lãi thu hồi chi sai - 15 cột)
    /// </summary>
    public class BaoCao07BcnvItemDto
    {
        [DisplayName("STT")]
        public string Stt { get; set; } = string.Empty; // "I", "II", "1", "2", "3", "III", "IV"

        [DisplayName("Chỉ tiêu")]
        public string ChiTieu { get; set; } = string.Empty;

        [DisplayName("Mã số")]
        public string MaSo { get; set; } = string.Empty; // "01", "02", ...

        [DisplayName("In đậm")]
        public bool IsBold { get; set; } = false; // Phân biệt cấp cha (in đậm) và cấp con

        // --- THỰC HIỆN THU HỒI CỦA ĐƠN VỊ (Cột 1 đến Cột 5) ---
        [DisplayName("Số còn phải thu lãi kỳ trước chuyển sang (1)")]
        public decimal? Col1_SoPhaiThuKyTruoc { get; set; }

        [DisplayName("Điều chỉnh số còn phải thu lãi kỳ trước chuyển sang (2)")]
        public decimal? Col2_DieuChinhKyTruoc { get; set; }

        [DisplayName("Số phải thu lãi trong kỳ (3)")]
        public decimal? Col3_SoPhaiThuTrongKy { get; set; }

        [DisplayName("Số đã thu lãi trong kỳ (4)")]
        public decimal? Col4_SoDaThuTrongKy { get; set; }

        [DisplayName("Số còn phải thu lãi chuyển kỳ sau (5)")]
        public decimal? Col5_SoConPhaiThuChuyenKySau { get; set; }

        // --- NỘP SỐ THU HỒI CHI SAI VỀ ĐƠN VỊ QUẢN LÝ CẤP TRÊN (Cột 6 đến Cột 12) ---
        [DisplayName("Số còn phải nộp về cấp trên kỳ trước chuyển sang (6)")]
        public decimal? Col6_SoPhaiNopKyTruoc { get; set; }

        [DisplayName("Điều chỉnh số còn phải nộp kỳ trước chuyển sang (7)")]
        public decimal? Col7_DieuChinhNopKyTruoc { get; set; }

        [DisplayName("Số phải nộp trong kỳ (8)")]
        public decimal? Col8_SoPhaiNopTrongKy { get; set; }

        [DisplayName("Số đã nộp trong kỳ (9)")]
        public decimal? Col9_SoDaNopTrongKy { get; set; }

        [DisplayName("Số còn phải nộp chuyển kỳ sau (10)")]
        public decimal? Col10_SoConPhaiNopChuyenKySau { get; set; }

        // Trong đó:
        [DisplayName("Số tiền đã thu chưa nộp (11)")]
        public decimal? Col11_TrongDoDaThuChuaNop { get; set; }

        [DisplayName("Số còn phải thu hồi (12)")]
        public decimal? Col12_TrongDoConPhaiThuHoi { get; set; }

        public BaoCao07BcnvItemDto()
        {
        }

        /// <summary>
        /// Dữ liệu mẫu chuẩn đúng theo danh mục các quỹ trong hình ảnh Mẫu 07/BCNV
        /// </summary>
        public static List<BaoCao07BcnvItemDto> GetSampleData()
        {
            return new List<BaoCao07BcnvItemDto>
            {
                new BaoCao07BcnvItemDto
                {
                    Stt = "I", ChiTieu = "NSNN đảm bảo", MaSo = "01", IsBold = true,
                    Col1_SoPhaiThuKyTruoc = 15000000, Col2_DieuChinhKyTruoc = 0, Col3_SoPhaiThuTrongKy = 5000000,
                    Col4_SoDaThuTrongKy = 8000000, Col5_SoConPhaiThuChuyenKySau = 12000000,
                    Col6_SoPhaiNopKyTruoc = 10000000, Col7_DieuChinhNopKyTruoc = 0, Col8_SoPhaiNopTrongKy = 5000000,
                    Col9_SoDaNopTrongKy = 7000000, Col10_SoConPhaiNopChuyenKySau = 8000000,
                    Col11_TrongDoDaThuChuaNop = 1000000, Col12_TrongDoConPhaiThuHoi = 7000000
                },
                new BaoCao07BcnvItemDto
                {
                    Stt = "II", ChiTieu = "Quỹ BHXH", MaSo = "02", IsBold = true,
                    Col1_SoPhaiThuKyTruoc = 85000000, Col2_DieuChinhKyTruoc = 0, Col3_SoPhaiThuTrongKy = 25000000,
                    Col4_SoDaThuTrongKy = 40000000, Col5_SoConPhaiThuChuyenKySau = 70000000,
                    Col6_SoPhaiNopKyTruoc = 50000000, Col7_DieuChinhNopKyTruoc = 0, Col8_SoPhaiNopTrongKy = 25000000,
                    Col9_SoDaNopTrongKy = 35000000, Col10_SoConPhaiNopChuyenKySau = 40000000,
                    Col11_TrongDoDaThuChuaNop = 5000000, Col12_TrongDoConPhaiThuHoi = 35000000
                },
                new BaoCao07BcnvItemDto
                {
                    Stt = "1", ChiTieu = "Quỹ ốm đau, thai sản", MaSo = "03", IsBold = false,
                    Col1_SoPhaiThuKyTruoc = 25000000, Col2_DieuChinhKyTruoc = 0, Col3_SoPhaiThuTrongKy = 8000000,
                    Col4_SoDaThuTrongKy = 12000000, Col5_SoConPhaiThuChuyenKySau = 21000000,
                    Col6_SoPhaiNopKyTruoc = 15000000, Col7_DieuChinhNopKyTruoc = 0, Col8_SoPhaiNopTrongKy = 8000000,
                    Col9_SoDaNopTrongKy = 10000000, Col10_SoConPhaiNopChuyenKySau = 13000000,
                    Col11_TrongDoDaThuChuaNop = 2000000, Col12_TrongDoConPhaiThuHoi = 11000000
                },
                new BaoCao07BcnvItemDto
                {
                    Stt = "2", ChiTieu = "Quỹ TNLĐ-BNN", MaSo = "04", IsBold = false,
                    Col1_SoPhaiThuKyTruoc = 10000000, Col2_DieuChinhKyTruoc = 0, Col3_SoPhaiThuTrongKy = 2000000,
                    Col4_SoDaThuTrongKy = 5000000, Col5_SoConPhaiThuChuyenKySau = 7000000,
                    Col6_SoPhaiNopKyTruoc = 5000000, Col7_DieuChinhNopKyTruoc = 0, Col8_SoPhaiNopTrongKy = 2000000,
                    Col9_SoDaNopTrongKy = 5000000, Col10_SoConPhaiNopChuyenKySau = 2000000,
                    Col11_TrongDoDaThuChuaNop = 0, Col12_TrongDoConPhaiThuHoi = 2000000
                },
                new BaoCao07BcnvItemDto
                {
                    Stt = "3", ChiTieu = "Quỹ hưu trí, tử tuất", MaSo = "05", IsBold = false,
                    Col1_SoPhaiThuKyTruoc = 50000000, Col2_DieuChinhKyTruoc = 0, Col3_SoPhaiThuTrongKy = 15000000,
                    Col4_SoDaThuTrongKy = 23000000, Col5_SoConPhaiThuChuyenKySau = 42000000,
                    Col6_SoPhaiNopKyTruoc = 30000000, Col7_DieuChinhNopKyTruoc = 0, Col8_SoPhaiNopTrongKy = 15000000,
                    Col9_SoDaNopTrongKy = 20000000, Col10_SoConPhaiNopChuyenKySau = 25000000,
                    Col11_TrongDoDaThuChuaNop = 3000000, Col12_TrongDoConPhaiThuHoi = 22000000
                },
                new BaoCao07BcnvItemDto
                {
                    Stt = "III", ChiTieu = "Quỹ BH thất nghiệp", MaSo = "06", IsBold = true,
                    Col1_SoPhaiThuKyTruoc = 12000000, Col2_DieuChinhKyTruoc = 0, Col3_SoPhaiThuTrongKy = 3000000,
                    Col4_SoDaThuTrongKy = 6000000, Col5_SoConPhaiThuChuyenKySau = 9000000,
                    Col6_SoPhaiNopKyTruoc = 8000000, Col7_DieuChinhNopKyTruoc = 0, Col8_SoPhaiNopTrongKy = 3000000,
                    Col9_SoDaNopTrongKy = 5000000, Col10_SoConPhaiNopChuyenKySau = 6000000,
                    Col11_TrongDoDaThuChuaNop = 1000000, Col12_TrongDoConPhaiThuHoi = 5000000
                },
                new BaoCao07BcnvItemDto
                {
                    Stt = "IV", ChiTieu = "Quỹ BHYT", MaSo = "07", IsBold = true,
                    Col1_SoPhaiThuKyTruoc = 30000000, Col2_DieuChinhKyTruoc = 0, Col3_SoPhaiThuTrongKy = 10000000,
                    Col4_SoDaThuTrongKy = 15000000, Col5_SoConPhaiThuChuyenKySau = 25000000,
                    Col6_SoPhaiNopKyTruoc = 20000000, Col7_DieuChinhNopKyTruoc = 0, Col8_SoPhaiNopTrongKy = 10000000,
                    Col9_SoDaNopTrongKy = 14000000, Col10_SoConPhaiNopChuyenKySau = 16000000,
                    Col11_TrongDoDaThuChuaNop = 1000000, Col12_TrongDoConPhaiThuHoi = 15000000
                }
            };
        }
    }
}
