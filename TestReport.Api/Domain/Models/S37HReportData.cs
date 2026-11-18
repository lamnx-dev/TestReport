namespace TestReport.Api.Domain.Models;

public sealed class S37HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public string TaiKhoan => "1381";
    public string MaDonVi { get; set; } = "VP";
    public int Nam { get; set; } = 2026;
    // Mã và tên được chọn từ danh mục tài sản (chi tiết 1 của tài khoản 1381).
    public string MaTaiSan { get; set; } = "TS001";
    public string TenTaiSan { get; set; } = "Máy tính để bàn";
    public S37HBalance SoDuDauKy { get; set; } = new();
    // Nếu có số dư cuối kỳ từ bảng số dư, dùng trực tiếp thay cho giá trị đối chiếu tính từ phát sinh.
    public S37HBalance? SoDuCuoiKy { get; set; }
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Mỗi dòng là một chi tiết định khoản đã ghép chứng từ; không gộp các chi tiết cùng chứng từ.
    public List<S37HPosting> Items { get; set; } = [];

    public List<S37HRow> CreateRows()
    {
        if (Nam is < 1 or > 9999 || string.IsNullOrWhiteSpace(MaDonVi) || string.IsNullOrWhiteSpace(MaTaiSan) ||
            SoDuDauKy is null || Items is null || Items.Any(x => x is null || x.NgayGhiSo == default ||
                string.IsNullOrWhiteSpace(x.MaDonVi) || string.IsNullOrWhiteSpace(x.TaiKhoanNo) || string.IsNullOrWhiteSpace(x.TaiKhoanCo)))
            throw new ArgumentException("Thiếu năm, đơn vị, mã tài sản, số dư hoặc dữ liệu định khoản hợp lệ.");

        var movements = new List<(S37HRow Row, int Month, bool Adjustment)>();
        foreach (var posting in Items.Where(x => x.MaDonVi == MaDonVi && x.NgayGhiSo.Year == Nam).OrderBy(x => x.NgayGhiSo))
        {
            if (posting.TaiKhoanNo == TaiKhoan && posting.NoMaTaiSan == MaTaiSan) Add(true);
            if (posting.TaiKhoanCo == TaiKhoan && posting.CoMaTaiSan == MaTaiSan) Add(false);

            void Add(bool debit) => movements.Add((new()
            {
                NgayGhiSo = posting.NgayGhiSo, NgayChungTu = posting.NgayChungTu,
                SoChungTu = posting.SoChungTu, DienGiai = posting.DienGiai, GhiChu = posting.GhiChu,
                TaiKhoanDoiUng = debit ? posting.TaiKhoanCo : posting.TaiKhoanNo,
                SoLuongPhatSinh = debit ? posting.SoLuong : 0, GiaTriPhatSinh = debit ? posting.SoTien : 0,
                SoLuongXuLy = debit ? 0 : posting.SoLuong, GiaTriXuLy = debit ? 0 : posting.SoTien
            }, posting.NgayGhiSo.Month, posting.MaLoai == 16));
        }
        var normal = movements.Where(x => !x.Adjustment).ToList();
        var rows = new List<S37HRow>
        {
            Balance("Số dư đầu kỳ", SoDuDauKy),
            Total("Điều chỉnh số dư đầu năm", movements.Where(x => x.Adjustment).Select(x => x.Row)),
            new() { DienGiai = "Số phát sinh", InDam = true }
        };
        foreach (var month in normal.GroupBy(x => x.Month))
        {
            rows.AddRange(month.Select(x => x.Row));
            rows.Add(Total($"Cộng phát sinh tháng {month.Key:00}", month.Select(x => x.Row)));
        }
        rows.Add(Total("Cộng luỹ kế từ đầu năm", normal.Select(x => x.Row)));
        var closing = SoDuCuoiKy ?? new S37HBalance
        {
            SoLuong = SoDuDauKy.SoLuong + movements.Sum(x => x.Row.SoLuongPhatSinh - x.Row.SoLuongXuLy),
            No = SoDuDauKy.No + movements.Sum(x => x.Row.GiaTriPhatSinh),
            Co = SoDuDauKy.Co + movements.Sum(x => x.Row.GiaTriXuLy)
        };
        rows.Add(Balance("Số dư cuối kỳ", closing));
        return rows;
    }

    private static S37HRow Balance(string label, S37HBalance balance) => new()
    {
        DienGiai = label, InDam = true, SoLuongPhatSinh = balance.SoLuong, GiaTriPhatSinh = balance.No - balance.Co
    };

    private static S37HRow Total(string label, IEnumerable<S37HRow> source)
    {
        var rows = source.ToList();
        return new() { DienGiai = label, InDam = true, SoLuongPhatSinh = rows.Sum(x => x.SoLuongPhatSinh),
            GiaTriPhatSinh = rows.Sum(x => x.GiaTriPhatSinh), SoLuongXuLy = rows.Sum(x => x.SoLuongXuLy), GiaTriXuLy = rows.Sum(x => x.GiaTriXuLy) };
    }

    public static List<S37HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S37HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
        SoDuDauKy = new() { SoLuong = 2, No = 20_000_000m },
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 12, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 1), MaLoai = 16, TaiKhoanNo = "1381", TaiKhoanCo = "421", NoMaTaiSan = "TS001", SoTien = 1_000_000m },
            new() { NgayGhiSo = new(2026, 1, 10), NgayChungTu = new(2026, 1, 9), SoChungTu = "KK001", DienGiai = "Ghi nhận máy tính thiếu qua kiểm kê", TaiKhoanNo = "1381", TaiKhoanCo = "211", NoMaTaiSan = "TS001", SoLuong = 3, SoTien = 30_000_000m, GhiChu = "Biên bản kiểm kê 01" },
            new() { NgayGhiSo = new(2026, 1, 25), NgayChungTu = new(2026, 1, 24), SoChungTu = "XL001", DienGiai = "Xử lý tài sản thiếu, cá nhân bồi thường", TaiKhoanNo = "1388", TaiKhoanCo = "1381", CoMaTaiSan = "TS001", SoLuong = 1, SoTien = 10_000_000m },
            new() { NgayGhiSo = new(2026, 2, 10), NgayChungTu = new(2026, 2, 10), SoChungTu = "KK002", DienGiai = "Ghi nhận bổ sung tài sản thiếu", TaiKhoanNo = "1381", TaiKhoanCo = "211", NoMaTaiSan = "TS001", SoLuong = 1, SoTien = 12_000_000m },
            new() { NgayGhiSo = new(2026, 2, 28), NgayChungTu = new(2026, 2, 28), SoChungTu = "XL002", DienGiai = "Xử lý tài sản thiếu theo quyết định", TaiKhoanNo = "811", TaiKhoanCo = "1381", CoMaTaiSan = "TS001", SoLuong = 2, SoTien = 20_000_000m }
        ]
    };
}

public sealed record S37HBalance
{
    public decimal SoLuong { get; init; }
    public decimal No { get; init; }
    public decimal Co { get; init; }
}

public sealed record S37HPosting
{
    public string MaDonVi { get; init; } = "VP";
    public DateTime NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanNo { get; init; } = "";
    public string TaiKhoanCo { get; init; } = "";
    public string NoMaTaiSan { get; init; } = "";
    public string CoMaTaiSan { get; init; } = "";
    public decimal SoLuong { get; init; }
    public decimal SoTien { get; init; }
    public int MaLoai { get; init; }
    public string GhiChu { get; init; } = "";
}

public sealed record S37HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanDoiUng { get; init; } = "";
    public decimal SoLuongPhatSinh { get; init; }
    public decimal GiaTriPhatSinh { get; init; }
    public decimal SoLuongXuLy { get; init; }
    public decimal GiaTriXuLy { get; init; }
    public string GhiChu { get; init; } = "";
    public bool InDam { get; init; }
}
