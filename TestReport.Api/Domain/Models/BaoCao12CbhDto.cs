using System;
using System.Collections.Generic;
using System.Linq;

namespace TestReport.Api.Domain.Models
{
    /// <summary>
    /// Model đóng gói TOÀN BỘ dữ liệu Mẫu số 12-CBH
    /// "Danh sách báo giảm hưởng, tạm dừng hưởng các chế độ BHXH hàng tháng, trợ cấp thất nghiệp"
    /// </summary>
    public class BaoCao12CbhReportData
    {
        // 1. Header cơ quan & địa bàn
        public string DonViChuQuan { get; set; } = "TÊN CƠ QUAN, TỔ CHỨC CHỦ QUẢN";
        public string CoQuanBhxh { get; set; } = "HÀ NỘI";
        public string MauSo { get; set; } = "Mẫu số 12-CBH";

        // 2. Kỳ báo cáo & Địa bàn chi trả
        public int Thang { get; set; } = 10;
        public int Nam { get; set; } = 2026;
        public string BhxhCoSo { get; set; } = "BHXH Ba Đình";
        public string XaPhuongToChiTra { get; set; } = "UBND Phường Cống Vị";
        public string HinhThucChiTra { get; set; } = "Chi trả tiền mặt qua Bưu điện";
        public string NguonKinhPhi { get; set; } = "Nguồn quỹ BHXH & Quỹ BHTN";

        // 3. Danh sách các nhóm chế độ (Mỗi nhóm chứa các dòng chi tiết)
        public List<BaoCao12CbhGroupDto> Groups { get; set; } = new();

        public static BaoCao12CbhReportData CreateSample()
        {
            return new BaoCao12CbhReportData
            {
                DonViChuQuan = "TÊN CƠ QUAN, TỔ CHỨC CHỦ QUẢN",
                CoQuanBhxh = "HÀ NỘI",
                Thang = 10,
                Nam = 2026,
                BhxhCoSo = "BHXH Ba Đình",
                XaPhuongToChiTra = "UBND Phường Cống Vị",
                HinhThucChiTra = "Chi trả tiền mặt qua Bưu điện",
                NguonKinhPhi = "Nguồn quỹ BHXH & Quỹ BHTN",
                Groups = BaoCao12CbhGroupDto.GetSampleData()
            };
        }
    }

    /// <summary>
    /// Đại diện cho 1 nhóm chế độ (Nhóm I, II, VIII, XI...)
    /// Dùng cho GroupHeaderBand trong DevExpress Report
    /// </summary>
    public class BaoCao12CbhGroupDto
    {
        public string Stt { get; set; } = string.Empty; // "I", "II", "VIII", "XI"
        public string TenNhom { get; set; } = string.Empty; // "Chế độ hưu trí", "Chế độ tử tuất"...
        public List<BaoCao12CbhItemDto> Items { get; set; } = new();

        // Thuộc tính tính tổng phục vụ tổng cộng cột trên ReportFooter
        public decimal? TongSoTienConPhaiTra => Items?.Sum(x => x.SoTienConPhaiTra ?? 0) ?? 0;
        public decimal? TongSoTienPhaiThuHoi => Items?.Sum(x => x.SoTienPhaiThuHoi ?? 0) ?? 0;

        public static List<BaoCao12CbhGroupDto> GetSampleData()
        {
            return new List<BaoCao12CbhGroupDto>
            {
                new BaoCao12CbhGroupDto
                {
                    Stt = "I",
                    TenNhom = "Chế độ hưu trí",
                    Items = new List<BaoCao12CbhItemDto>
                    {
                        new BaoCao12CbhItemDto
                        {
                            Stt = "1",
                            HoVaTen = "Nguyễn Văn An",
                            MaBhxh = "7912345678",
                            LyDoGiam = "Từ trần ngày 05/10/2026",
                            NgayGiam = "05/10/2026",
                            DaChiTraHetNgay = "30/09/2026",
                            SoTienConPhaiTra = 500000,
                            SoTienPhaiThuHoi = 0
                        },
                        new BaoCao12CbhItemDto
                        {
                            Stt = "2",
                            HoVaTen = "Trần Thị Bình",
                            MaBhxh = "7912345679",
                            LyDoGiam = "Chuyển nơi hưởng về TP.HCM",
                            NgayGiam = "01/10/2026",
                            DaChiTraHetNgay = "30/09/2026",
                            SoTienConPhaiTra = 0,
                            SoTienPhaiThuHoi = 1500000
                        }
                    }
                },
                new BaoCao12CbhGroupDto
                {
                    Stt = "II",
                    TenNhom = "Chế độ tử tuất",
                    Items = new List<BaoCao12CbhItemDto>
                    {
                        new BaoCao12CbhItemDto
                        {
                            Stt = "1",
                            HoVaTen = "Lê Thị Cúc",
                            MaBhxh = "7923456780",
                            LyDoGiam = "Hết tuổi hưởng tuất định kỳ",
                            NgayGiam = "10/10/2026",
                            DaChiTraHetNgay = "30/09/2026",
                            SoTienConPhaiTra = 1250000,
                            SoTienPhaiThuHoi = 0
                        }
                    }
                },
                new BaoCao12CbhGroupDto
                {
                    Stt = "VIII",
                    TenNhom = "Chế độ lương hưu tự nguyện",
                    Items = new List<BaoCao12CbhItemDto>
                    {
                        new BaoCao12CbhItemDto
                        {
                            Stt = "1",
                            HoVaTen = "Phạm Văn Dũng",
                            MaBhxh = "7934567891",
                            LyDoGiam = "Tạm dừng do chưa xác minh thông tin",
                            NgayGiam = "01/10/2026",
                            DaChiTraHetNgay = "30/09/2026",
                            SoTienConPhaiTra = 0,
                            SoTienPhaiThuHoi = 2100000
                        },
                        new BaoCao12CbhItemDto
                        {
                            Stt = "2",
                            HoVaTen = "Hoàng Thị Em",
                            MaBhxh = "7934567892",
                            LyDoGiam = "Xuất cảnh định cư",
                            NgayGiam = "12/10/2026",
                            DaChiTraHetNgay = "30/09/2026",
                            SoTienConPhaiTra = 0,
                            SoTienPhaiThuHoi = 0
                        }
                    }
                },
                new BaoCao12CbhGroupDto
                {
                    Stt = "XI",
                    TenNhom = "Trợ cấp thất nghiệp",
                    Items = new List<BaoCao12CbhItemDto>
                    {
                        new BaoCao12CbhItemDto
                        {
                            Stt = "1",
                            HoVaTen = "Vũ Đình Giang",
                            MaBhxh = "7945678903",
                            LyDoGiam = "Có việc làm mới theo QĐ số 45/QĐ-SLĐ",
                            NgayGiam = "08/10/2026",
                            DaChiTraHetNgay = "30/09/2026",
                            SoTienConPhaiTra = 0,
                            SoTienPhaiThuHoi = 3850000
                        }
                    }
                }
            };
        }
    }

    /// <summary>
    /// Đại diện cho 1 dòng dữ liệu chi tiết trong bảng Mẫu số 12-CBH (8 cột)
    /// </summary>
    public class BaoCao12CbhItemDto
    {
        public string Stt { get; set; } = string.Empty;
        public string HoVaTen { get; set; } = string.Empty;
        public string MaBhxh { get; set; } = string.Empty;
        public string LyDoGiam { get; set; } = string.Empty;
        public string NgayGiam { get; set; } = string.Empty;
        public string DaChiTraHetNgay { get; set; } = string.Empty;
        public decimal? SoTienConPhaiTra { get; set; }
        public decimal? SoTienPhaiThuHoi { get; set; }
    }
}
