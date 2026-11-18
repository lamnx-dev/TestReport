using System.Globalization;

namespace TestReport.Api.Domain.Models;

public sealed class S51HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public string TaiKhoan { get; set; } = "511";
    public string TenTaiKhoan { get; set; } = "Doanh thu từ kinh phí ngân sách nhà nước cấp";
    public string MaDonVi { get; set; } = "VP";
    public DateTime TuNgay { get; set; } = new(2026, 1, 1);
    public DateTime DenNgay { get; set; } = new(2026, 3, 31);
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Truyền các dòng định khoản từ đầu năm đến hết kỳ để tính số lũy kế.
    public List<S51HPosting> Items { get; set; } = [];

    public string KyBaoCao
    {
        get
        {
            var month = TuNgay.Month;
            var year = TuNgay.Year;
            if (TuNgay.Day == 1 && DenNgay.Day == DateTime.DaysInMonth(DenNgay.Year, DenNgay.Month) && year == DenNgay.Year)
            {
                if (month == 1 && DenNgay.Month == 12) return $"Năm {year}";
                if (month == DenNgay.Month) return $"Tháng {month:00}/{year}";
                if ((month - 1) % 3 == 0 && DenNgay.Month == month + 2) return $"Quý {(month + 2) / 3}/{year}";
            }
            return FormattableString.Invariant($"Từ ngày {TuNgay:dd'/'MM'/'yyyy} đến ngày {DenNgay:dd'/'MM'/'yyyy}");
        }
    }

    public List<S51HRow> CreateRows()
    {
        if (string.IsNullOrWhiteSpace(TaiKhoan) || !TaiKhoan.StartsWith('5') || !TaiKhoan.All(char.IsAsciiDigit))
            throw new ArgumentException("Chọn tài khoản có mã bắt đầu bằng 5.");
        if (TuNgay.Date > DenNgay.Date || TuNgay.Year != DenNgay.Year)
            throw new ArgumentException("Kỳ báo cáo phải nằm trong cùng năm và ngày bắt đầu không sau ngày kết thúc.");
        if (string.IsNullOrWhiteSpace(MaDonVi) || Items is null || Items.Any(x => x is null ||
            x.NgayGhiSo == default || string.IsNullOrWhiteSpace(x.MaDonVi) ||
            string.IsNullOrWhiteSpace(x.TaiKhoanNo) || string.IsNullOrWhiteSpace(x.TaiKhoanCo)))
            throw new ArgumentException("Thiếu phạm vi đơn vị hoặc dữ liệu định khoản.");

        var period = new List<S51HRow>();
        var year = new List<S51HRow>();
        decimal transfer = 0;
        foreach (var entry in Items.Where(x => x.MaDonVi == MaDonVi && x.NgayGhiSo.Year == TuNgay.Year &&
            x.NgayGhiSo.Date <= DenNgay.Date).OrderBy(x => x.NgayGhiSo))
        {
            var inPeriod = entry.NgayGhiSo.Date >= TuNgay.Date;
            if (entry.TaiKhoanNo == TaiKhoan)
            {
                if (entry.TaiKhoanCo.StartsWith("911", StringComparison.Ordinal))
                {
                    if (inPeriod) transfer += entry.SoTien;
                }
                else Add(true);
            }
            if (entry.TaiKhoanCo == TaiKhoan) Add(false);

            void Add(bool debit)
            {
                var row = new S51HRow
                {
                    NgayGhiSo = entry.NgayGhiSo,
                    NgayChungTu = entry.NgayChungTu,
                    SoChungTu = entry.SoChungTu,
                    DienGiai = entry.DienGiai,
                    TaiKhoanDoiUng = debit ? entry.TaiKhoanCo : entry.TaiKhoanNo,
                    PhatSinhNo = debit ? entry.SoTien : 0,
                    PhatSinhCo = debit ? 0 : entry.SoTien,
                    GhiChu = entry.GhiChu
                };
                year.Add(row);
                if (inPeriod) period.Add(row);
            }
        }

        return
        [
            .. period,
            Total("Cộng số phát sinh", period),
            Total("Lũy kế từ đầu năm", year),
            new() { DienGiai = "Kết chuyển để xác định kết quả hoạt động", PhatSinhNo = transfer, InDam = true }
        ];
    }

    private static S51HRow Total(string label, List<S51HRow> rows) => new()
    {
        DienGiai = label,
        PhatSinhNo = rows.Sum(x => x.PhatSinhNo),
        PhatSinhCo = rows.Sum(x => x.PhatSinhCo),
        InDam = true
    };

    public static List<S51HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S51HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 3, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 12), NgayChungTu = new(2026, 1, 10), SoChungTu = "DT001",
                DienGiai = "Ghi nhận doanh thu từ kinh phí được cấp tháng 01", TaiKhoanNo = "135", TaiKhoanCo = "511", SoTien = 120_000_000m },
            new() { NgayGhiSo = new(2026, 2, 16), NgayChungTu = new(2026, 2, 15), SoChungTu = "DT002",
                DienGiai = "Ghi nhận doanh thu từ kinh phí được cấp tháng 02", TaiKhoanNo = "135", TaiKhoanCo = "511", SoTien = 80_000_000m },
            new() { NgayGhiSo = new(2026, 3, 6), NgayChungTu = new(2026, 3, 6), SoChungTu = "DC003",
                DienGiai = "Điều chỉnh giảm doanh thu", TaiKhoanNo = "511", TaiKhoanCo = "135", SoTien = 5_000_000m, GhiChu = "Theo biên bản" },
            new() { NgayGhiSo = new(2026, 3, 31), NgayChungTu = new(2026, 3, 31), SoChungTu = "KC003",
                DienGiai = "Kết chuyển doanh thu", TaiKhoanNo = "511", TaiKhoanCo = "911", SoTien = 195_000_000m }
        ]
    };
}

public sealed record S51HPosting
{
    public string MaDonVi { get; init; } = "VP";
    public DateTime NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanNo { get; init; } = "";
    public string TaiKhoanCo { get; init; } = "";
    public decimal SoTien { get; init; }
    public string GhiChu { get; init; } = "";
}

public sealed record S51HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanDoiUng { get; init; } = "";
    public decimal PhatSinhNo { get; init; }
    public decimal PhatSinhCo { get; init; }
    public string GhiChu { get; init; } = "";
    public bool InDam { get; init; }
}
