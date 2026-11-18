namespace TestReport.Api.Domain.Models;

public sealed class S54HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public string TaiKhoan { get; set; } = "133";
    public string TenTaiKhoan { get; set; } = "Thuế GTGT được khấu trừ";
    public DateTime TuNgay { get; set; } = new(2026, 1, 1);
    public DateTime DenNgay { get; set; } = new(2026, 12, 31);
    // Số dư tài khoản tháng 1 theo mẫu Excel. Items phải có đủ phát sinh từ đầu năm.
    public decimal NoDauNam { get; set; }
    public decimal CoDauNam { get; set; }
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    public List<S54HPosting> Items { get; set; } = [];

    public string KyBaoCao => TuNgay.Date == new DateTime(TuNgay.Year, 1, 1) && DenNgay.Date == new DateTime(TuNgay.Year, 12, 31)
        ? $"Năm: {TuNgay.Year}"
        : $"Từ ngày {TuNgay:dd/MM/yyyy} đến ngày {DenNgay:dd/MM/yyyy}";

    public List<S54HRow> CreateRows()
    {
        if (string.IsNullOrWhiteSpace(TaiKhoan) || !TaiKhoan.StartsWith("133", StringComparison.Ordinal) || !TaiKhoan.All(char.IsAsciiDigit))
            throw new ArgumentException("Chọn tài khoản có mã bắt đầu bằng 133.");
        if (TuNgay.Date > DenNgay.Date || TuNgay.Year != DenNgay.Year)
            throw new ArgumentException("Kỳ báo cáo phải nằm trong cùng năm và ngày bắt đầu không sau ngày kết thúc.");
        if (Items is null || Items.Any(x => x is null || string.IsNullOrWhiteSpace(x.TaiKhoan)))
            throw new ArgumentException("Phát sinh phải có tài khoản.");

        var entries = Items.Where(x => x.TaiKhoan.StartsWith(TaiKhoan, StringComparison.Ordinal) &&
            x.NgayGhiSo.Year == TuNgay.Year && x.NgayGhiSo.Date <= DenNgay.Date).OrderBy(x => x.NgayGhiSo).ToList();
        var adjustments = entries.Where(x => x.MaLoai == 16).ToList();
        var normal = entries.Where(x => x.MaLoai != 16).ToList();
        var period = normal.Where(x => x.NgayGhiSo.Date >= TuNgay.Date).ToList();
        var opening = NoDauNam - CoDauNam;
        return
        [
            Balance("Số dư đầu kỳ", opening),
            Total("Điều chỉnh số dư đầu năm", adjustments),
            new() { DienGiai = "Số phát sinh", InDam = true },
            .. period.Select(x => new S54HRow { NgayGhiSo = x.NgayGhiSo, NgayChungTu = x.NgayChungTu,
                SoChungTu = x.SoChungTu, DienGiai = x.DienGiai, DuocHoanLai = x.PhatSinhNo, DaHoanLai = x.PhatSinhCo }),
            Total("Cộng phát sinh trong kỳ", period),
            Total("Cộng luỹ kế từ đầu năm", normal),
            Balance("Số dư cuối kỳ", opening + entries.Sum(x => x.PhatSinhNo - x.PhatSinhCo))
        ];
    }

    private static S54HRow Balance(string label, decimal balance) => new()
    {
        DienGiai = label, InDam = true, DuocHoanLai = Math.Max(balance, 0), DaHoanLai = Math.Max(-balance, 0)
    };

    private static S54HRow Total(string label, IEnumerable<S54HPosting> source) => new()
    {
        DienGiai = label, InDam = true, DuocHoanLai = source.Sum(x => x.PhatSinhNo), DaHoanLai = source.Sum(x => x.PhatSinhCo)
    };

    public static List<S54HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S54HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI", NoDauNam = 5_000_000m,
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 12, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 1), MaLoai = 16, PhatSinhNo = 500_000m },
            new() { NgayGhiSo = new(2026, 1, 10), NgayChungTu = new(2026, 1, 9), SoChungTu = "HT001", DienGiai = "Ghi nhận số thuế GTGT được hoàn lại", PhatSinhNo = 12_000_000m },
            new() { NgayGhiSo = new(2026, 1, 31), NgayChungTu = new(2026, 1, 31), SoChungTu = "HT002", DienGiai = "Nhận tiền hoàn thuế GTGT tháng 01", PhatSinhCo = 8_000_000m },
            new() { NgayGhiSo = new(2026, 2, 10), NgayChungTu = new(2026, 2, 10), SoChungTu = "HT003", DienGiai = "Ghi nhận bổ sung thuế GTGT được hoàn lại", PhatSinhNo = 6_000_000m },
            new() { NgayGhiSo = new(2026, 2, 28), NgayChungTu = new(2026, 2, 28), SoChungTu = "HT004", DienGiai = "Nhận tiền hoàn thuế GTGT tháng 02", PhatSinhCo = 10_000_000m }
        ]
    };
}

public sealed record S54HPosting
{
    public DateTime NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string TaiKhoan { get; init; } = "1331";
    public int MaLoai { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public decimal PhatSinhNo { get; init; }
    public decimal PhatSinhCo { get; init; }
}

public sealed record S54HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public decimal DuocHoanLai { get; init; }
    public decimal DaHoanLai { get; init; }
    public bool InDam { get; init; }
}
