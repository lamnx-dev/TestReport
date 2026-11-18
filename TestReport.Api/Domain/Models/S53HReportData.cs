using System.Globalization;

namespace TestReport.Api.Domain.Models;

public sealed class S53HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public string TaiKhoan { get; set; } = "3331";
    public string TenTaiKhoan { get; set; } = "Thuế giá trị gia tăng phải nộp";
    public DateTime TuNgay { get; set; } = new(2026, 1, 1);
    public DateTime DenNgay { get; set; } = new(2026, 3, 31);
    // Số dư của tài khoản đã chọn tại đầu tháng bắt đầu kỳ báo cáo, trước điều chỉnh trong kỳ.
    // Không cộng lại phát sinh trước kỳ vào số dư này.
    public decimal NoDauKy { get; set; }
    public decimal CoDauKy { get; set; }
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Các phát sinh tài khoản từ đầu năm để tính lũy kế; mã loại 16 là điều chỉnh đầu năm.
    public List<S53HPosting> Items { get; set; } = [];

    private bool IsYear => TuNgay.Month == 1 && DenNgay.Month == 12;
    private bool IsQuarter => (TuNgay.Month - 1) % 3 == 0 && DenNgay.Month == TuNgay.Month + 2;
    public string KyBaoCao => IsYear ? $"Năm: {TuNgay.Year}"
        : IsQuarter ? $"Quý: {(TuNgay.Month - 1) / 3 + 1} - Năm: {TuNgay.Year}"
        : "Tháng: " + TuNgay.ToString("MM'/'yyyy", CultureInfo.InvariantCulture);

    public List<S53HRow> CreateRows()
    {
        if (string.IsNullOrWhiteSpace(TaiKhoan) || !TaiKhoan.StartsWith("3331", StringComparison.Ordinal) || !TaiKhoan.All(char.IsAsciiDigit))
            throw new ArgumentException("Chọn tài khoản có mã bắt đầu bằng 3331.");
        if (TuNgay.Year != DenNgay.Year || TuNgay.Date > DenNgay.Date || TuNgay.Day != 1 ||
            DenNgay.Day != DateTime.DaysInMonth(DenNgay.Year, DenNgay.Month) ||
            !(IsYear || IsQuarter || TuNgay.Month == DenNgay.Month))
            throw new ArgumentException("Chọn một tháng, quý hoặc năm đầy đủ trong cùng năm.");
        if (Items is null || Items.Any(x => x is null || x.NgayGhiSo == default || string.IsNullOrWhiteSpace(x.TaiKhoan)))
            throw new ArgumentException("Phát sinh phải có ngày ghi sổ và tài khoản.");

        var entries = Items.Where(x => x.TaiKhoan.StartsWith(TaiKhoan, StringComparison.Ordinal) &&
            x.NgayGhiSo.Year == TuNgay.Year && x.NgayGhiSo.Date <= DenNgay.Date)
            .OrderBy(x => x.NgayGhiSo).ToList();
        var period = entries.Where(x => x.NgayGhiSo.Date >= TuNgay.Date).ToList();
        var normal = period.Where(x => x.MaLoai != 16).ToList();
        var rows = new List<S53HRow>
        {
            Balance("Số dư đầu kỳ", NoDauKy - CoDauKy),
            Total("Điều chỉnh số dư đầu năm", period.Where(x => x.MaLoai == 16)),
            new() { DienGiai = "Số phát sinh", InDam = true }
        };
        // Giữ cả tháng không phát sinh; các dòng tổng không tham gia tính tổng tiếp theo.
        for (var month = TuNgay.Month; month <= DenNgay.Month; month++)
        {
            var monthly = normal.Where(x => x.NgayGhiSo.Month == month).ToList();
            rows.AddRange(monthly.Select(x => new S53HRow
            {
                NgayGhiSo = x.NgayGhiSo, NgayChungTu = x.NgayChungTu, SoChungTu = x.SoChungTu,
                DienGiai = x.DienGiai, DaNop = x.PhatSinhNo, PhaiNop = x.PhatSinhCo
            }));
            rows.Add(Total($"Cộng phát sinh tháng {month:00}", monthly));
        }
        if (IsQuarter) rows.Add(Total("Cộng phát sinh trong kỳ", normal));
        rows.Add(Total("Cộng luỹ kế từ đầu năm", entries.Where(x => x.MaLoai != 16)));
        rows.Add(Balance("Số dư cuối kỳ", NoDauKy - CoDauKy + period.Sum(x => x.PhatSinhNo - x.PhatSinhCo)));
        return rows;
    }

    private static S53HRow Balance(string label, decimal balance) => new()
    {
        DienGiai = label, InDam = true, DaNop = Math.Max(balance, 0), PhaiNop = Math.Max(-balance, 0)
    };

    private static S53HRow Total(string label, IEnumerable<S53HPosting> source)
    {
        var entries = source.ToList();
        return new() { DienGiai = label, InDam = true, DaNop = entries.Sum(x => x.PhatSinhNo), PhaiNop = entries.Sum(x => x.PhatSinhCo) };
    }

    public static List<S53HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S53HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI", CoDauKy = 5_000_000m,
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 3, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 1), MaLoai = 16, PhatSinhCo = 500_000m },
            new() { NgayGhiSo = new(2026, 1, 10), NgayChungTu = new(2026, 1, 9), SoChungTu = "GT001", DienGiai = "Ghi nhận thuế GTGT phải nộp tháng 01", PhatSinhCo = 12_000_000m },
            new() { NgayGhiSo = new(2026, 1, 31), NgayChungTu = new(2026, 1, 31), SoChungTu = "GT002", DienGiai = "Nộp thuế GTGT tháng 01", PhatSinhNo = 8_000_000m },
            new() { NgayGhiSo = new(2026, 2, 10), NgayChungTu = new(2026, 2, 10), SoChungTu = "GT003", DienGiai = "Ghi nhận thuế GTGT phải nộp tháng 02", PhatSinhCo = 6_000_000m },
            new() { NgayGhiSo = new(2026, 3, 31), NgayChungTu = new(2026, 3, 31), SoChungTu = "GT004", DienGiai = "Nộp thuế GTGT vào ngân sách nhà nước", PhatSinhNo = 10_000_000m }
        ]
    };
}

public sealed record S53HPosting
{
    public DateTime NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string TaiKhoan { get; init; } = "33311";
    public int MaLoai { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public decimal PhatSinhNo { get; init; }
    public decimal PhatSinhCo { get; init; }
}

public sealed record S53HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public decimal DaNop { get; init; }
    public decimal PhaiNop { get; init; }
    public bool InDam { get; init; }
}
