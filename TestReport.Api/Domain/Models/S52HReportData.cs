using System.Globalization;

namespace TestReport.Api.Domain.Models;

public sealed class S52HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public string TaiKhoan { get; set; } = "5311";
    public string MaDonVi { get; set; } = "VP";
    // Mã/tên được lấy từ danh mục chi tiết 1, 2 của tài khoản đã chọn, theo đơn vị quản lý.
    public string MaLoaiHoatDong { get; set; } = "DV";
    public string TenLoaiHoatDong { get; set; } = "Hoạt động dịch vụ";
    public string MaSanPham { get; set; } = "DV01";
    public string TenSanPham { get; set; } = "Dịch vụ đào tạo";
    public DateTime TuNgay { get; set; } = new(2026, 1, 1);
    public DateTime DenNgay { get; set; } = new(2026, 3, 31);
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Dữ liệu từ đầu năm đến hết kỳ, giữ từng chi tiết của từng định khoản.
    public List<S52HPosting> Items { get; set; } = [];

    public string KyBaoCao => $"Từ ngày {TuNgay.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)} đến ngày {DenNgay.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)}";

    public List<S52HRow> CreateRows()
    {
        if (string.IsNullOrWhiteSpace(TaiKhoan) || !TaiKhoan.StartsWith("531", StringComparison.Ordinal) || !TaiKhoan.All(char.IsAsciiDigit))
            throw new ArgumentException("Chọn tài khoản có mã bắt đầu bằng 531.");
        if (TuNgay.Date > DenNgay.Date || TuNgay.Year != DenNgay.Year)
            throw new ArgumentException("Kỳ báo cáo phải nằm trong cùng năm và ngày bắt đầu không sau ngày kết thúc.");
        if (string.IsNullOrWhiteSpace(MaDonVi) || string.IsNullOrWhiteSpace(MaLoaiHoatDong) || string.IsNullOrWhiteSpace(MaSanPham) ||
            Items is null || Items.Any(x => x is null || x.NgayGhiSo == default || string.IsNullOrWhiteSpace(x.MaDonVi) ||
                string.IsNullOrWhiteSpace(x.TaiKhoanNo) || string.IsNullOrWhiteSpace(x.TaiKhoanCo) || x.ChiTiet is null || x.ChiTiet.Any(d => d is null)))
            throw new ArgumentException("Thiếu phạm vi đơn vị, hoạt động, sản phẩm hoặc dữ liệu định khoản, chi tiết.");

        var period = new List<S52HRow>();
        var year = new List<S52HRow>();
        decimal transfer = 0;
        foreach (var entry in Items.Where(x => x.MaDonVi == MaDonVi && x.NgayGhiSo.Year == TuNgay.Year && x.NgayGhiSo.Date <= DenNgay.Date)
            .OrderBy(x => x.NgayGhiSo))
        {
            var inPeriod = entry.NgayGhiSo.Date >= TuNgay.Date;
            var credit = entry.TaiKhoanCo == TaiKhoan && entry.CoChiTiet1 == MaLoaiHoatDong && entry.CoChiTiet2 == MaSanPham;
            var debit = entry.TaiKhoanNo == TaiKhoan && entry.NoChiTiet1 == MaLoaiHoatDong && entry.NoChiTiet2 == MaSanPham;
            var closing = entry.TaiKhoanNo.StartsWith(TaiKhoan, StringComparison.Ordinal) && entry.TaiKhoanCo.StartsWith("911", StringComparison.Ordinal) &&
                entry.NoChiTiet1 == MaLoaiHoatDong && entry.NoChiTiet2 == MaSanPham;
            foreach (var detail in entry.ChiTiet)
            {
                if (closing && inPeriod) transfer += detail.SoTien;
                // Nếu hai vế cùng là tài khoản đã chọn, giữ riêng doanh thu và giảm trừ.
                if (credit) Add(false);
                if (debit && !closing) Add(true);

                void Add(bool reduction)
                {
                    var row = new S52HRow
                    {
                        NgayGhiSo = entry.NgayGhiSo, NgayChungTu = entry.NgayChungTu,
                        SoChungTu = entry.SoChungTu, DienGiai = entry.DienGiai,
                        TaiKhoanDoiUng = reduction ? entry.TaiKhoanCo : entry.TaiKhoanNo,
                        SoLuong = reduction ? null : detail.SoLuong, DonGia = reduction ? null : detail.DonGia,
                        ThanhTien = reduction ? 0 : detail.SoTien, GiamTru = reduction ? detail.SoTien : 0
                    };
                    year.Add(row);
                    if (inPeriod) period.Add(row);
                }
            }
        }
        return [.. period, Total("Cộng số phát sinh", period), Total("Cộng lũy kế từ đầu năm", year),
            new() { DienGiai = "Kết chuyển để xác định kết quả hoạt động", GiamTru = transfer, InDam = true }];
    }

    private static S52HRow Total(string label, IEnumerable<S52HRow> source)
    {
        var rows = source.ToList();
        return new() { DienGiai = label, InDam = true, SoLuong = rows.Sum(x => x.SoLuong ?? 0),
            ThanhTien = rows.Sum(x => x.ThanhTien), GiamTru = rows.Sum(x => x.GiamTru) };
    }

    public static List<S52HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S52HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 3, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 15), NgayChungTu = new(2026, 1, 14), SoChungTu = "DT001", DienGiai = "Doanh thu dịch vụ đào tạo tháng 01", TaiKhoanNo = "131", TaiKhoanCo = "5311", CoChiTiet1 = "DV", CoChiTiet2 = "DV01",
                ChiTiet = [new() { SoLuong = 10, DonGia = 1_500_000m, SoTien = 15_000_000m }, new() { SoLuong = 5, DonGia = 2_000_000m, SoTien = 10_000_000m }] },
            new() { NgayGhiSo = new(2026, 2, 20), NgayChungTu = new(2026, 2, 20), SoChungTu = "DT002", DienGiai = "Doanh thu dịch vụ đào tạo tháng 02", TaiKhoanNo = "112", TaiKhoanCo = "5311", CoChiTiet1 = "DV", CoChiTiet2 = "DV01",
                ChiTiet = [new() { SoLuong = 8, DonGia = 1_500_000m, SoTien = 12_000_000m }] },
            new() { NgayGhiSo = new(2026, 3, 5), NgayChungTu = new(2026, 3, 5), SoChungTu = "GG003", DienGiai = "Giảm trừ doanh thu dịch vụ", TaiKhoanNo = "5311", TaiKhoanCo = "131", NoChiTiet1 = "DV", NoChiTiet2 = "DV01",
                ChiTiet = [new() { SoTien = 2_000_000m }] },
            new() { NgayGhiSo = new(2026, 3, 31), NgayChungTu = new(2026, 3, 31), SoChungTu = "KC003", DienGiai = "Kết chuyển doanh thu dịch vụ", TaiKhoanNo = "5311", TaiKhoanCo = "911", NoChiTiet1 = "DV", NoChiTiet2 = "DV01",
                ChiTiet = [new() { SoTien = 35_000_000m }] }
        ]
    };
}

public sealed record S52HPosting
{
    public string MaDonVi { get; init; } = "VP";
    public DateTime NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanNo { get; init; } = "";
    public string TaiKhoanCo { get; init; } = "";
    public string NoChiTiet1 { get; init; } = "";
    public string NoChiTiet2 { get; init; } = "";
    public string CoChiTiet1 { get; init; } = "";
    public string CoChiTiet2 { get; init; } = "";
    public List<S52HDetail> ChiTiet { get; init; } = [];
}

public sealed record S52HDetail
{
    public decimal SoLuong { get; init; }
    public decimal DonGia { get; init; }
    public decimal SoTien { get; init; }
}

public sealed record S52HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanDoiUng { get; init; } = "";
    public decimal? SoLuong { get; init; }
    public decimal? DonGia { get; init; }
    public decimal ThanhTien { get; init; }
    public decimal GiamTru { get; init; }
    public bool InDam { get; init; }
}
