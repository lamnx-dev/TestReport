namespace TestReport.Api.Domain.Models;

public sealed class S38HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public string TaiKhoan { get; set; } = "331";
    public string TenTaiKhoan { get; set; } = "Phải trả cho người bán";
    public string MaDonVi { get; set; } = "VP";
    public DateTime TuNgay { get; set; } = new(2026, 4, 1);
    public DateTime DenNgay { get; set; } = new(2026, 6, 30);
    public decimal NoDauNam { get; set; }
    public decimal CoDauNam { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Số dư trước điều chỉnh và các định khoản phải cùng phạm vi đơn vị, tài khoản.
    // Truyền đủ định khoản từ đầu năm đến hết kỳ.
    public List<S38HPosting> Items { get; set; } = [];

    public string KyBaoCao
    {
        get
        {
            if (TuNgay.Day == 1 && DenNgay.Day == DateTime.DaysInMonth(DenNgay.Year, DenNgay.Month) && TuNgay.Year == DenNgay.Year)
            {
                if (TuNgay.Month == DenNgay.Month) return FormattableString.Invariant($"Tháng {TuNgay:MM'/'yyyy}");
                if (TuNgay.Month == 1 && DenNgay.Month == 12) return $"Năm {TuNgay.Year}";
                if ((TuNgay.Month - 1) % 3 == 0 && DenNgay.Month == TuNgay.Month + 2)
                    return $"Quý {(TuNgay.Month + 2) / 3}/{TuNgay.Year}";
            }
            return FormattableString.Invariant($"Từ ngày {TuNgay:dd'/'MM'/'yyyy} đến ngày {DenNgay:dd'/'MM'/'yyyy}");
        }
    }

    public List<S38HRow> CreateRows()
    {
        if (string.IsNullOrWhiteSpace(TaiKhoan) || !TaiKhoan.All(char.IsAsciiDigit))
            throw new ArgumentException("Chọn mã tài khoản hợp lệ.");
        if (TuNgay.Date > DenNgay.Date || TuNgay.Year != DenNgay.Year)
            throw new ArgumentException("Kỳ báo cáo phải nằm trong cùng năm và ngày bắt đầu không sau ngày kết thúc.");
        if (string.IsNullOrWhiteSpace(MaDonVi) || Items is null || Items.Any(x => x is null ||
            x.NgayGhiSo == default || string.IsNullOrWhiteSpace(x.MaDonVi) ||
            string.IsNullOrWhiteSpace(x.TaiKhoanNo) || string.IsNullOrWhiteSpace(x.TaiKhoanCo)))
            throw new ArgumentException("Thiếu phạm vi đơn vị hoặc dữ liệu định khoản.");

        var entries = Items.Where(x => x.MaDonVi == MaDonVi && x.NgayGhiSo.Year == TuNgay.Year && x.NgayGhiSo.Date <= DenNgay.Date)
            .OrderBy(x => x.NgayGhiSo)
            .Select(x => (Posting: x, No: x.TaiKhoanNo.StartsWith(TaiKhoan, StringComparison.Ordinal),
                Co: x.TaiKhoanCo.StartsWith(TaiKhoan, StringComparison.Ordinal)))
            .Where(x => x.No || x.Co)
            .ToList();

        var opening = NoDauNam - CoDauNam;
        decimal adjustmentDebit = 0, adjustmentCredit = 0, priorDebit = 0, priorCredit = 0;
        foreach (var (posting, debit, credit) in entries)
        {
            var debitAmount = debit ? posting.SoTien : 0;
            var creditAmount = credit ? posting.SoTien : 0;
            if (posting.NgayGhiSo.Date < TuNgay.Date)
            {
                opening += debitAmount - creditAmount;
                if (posting.MaLoai != 16)
                {
                    priorDebit += debitAmount;
                    priorCredit += creditAmount;
                }
            }
            else if (posting.MaLoai == 16)
            {
                adjustmentDebit += debitAmount;
                adjustmentCredit += creditAmount;
            }
        }

        var rows = new List<S38HRow>
        {
            WithBalance(new() { DienGiai = "Số dư đầu kỳ", InDam = true }, opening),
            Total("Điều chỉnh số dư đầu năm", adjustmentDebit, adjustmentCredit),
            new() { DienGiai = "Số phát sinh", InDam = true }
        };

        var balance = opening + adjustmentDebit - adjustmentCredit;
        decimal periodDebit = 0, periodCredit = 0;
        foreach (var (posting, debit, credit) in entries.Where(x => x.Posting.NgayGhiSo.Date >= TuNgay.Date && x.Posting.MaLoai != 16))
        {
            var debitAmount = debit ? posting.SoTien : 0;
            var creditAmount = credit ? posting.SoTien : 0;
            periodDebit += debitAmount;
            periodCredit += creditAmount;
            balance += debitAmount - creditAmount;

            var detail = new S38HRow
            {
                NgayGhiSo = posting.NgayGhiSo,
                NgayChungTu = posting.NgayChungTu,
                SoChungTu = posting.SoChungTu,
                DienGiai = posting.DienGiai,
                GhiChu = posting.GhiChu
            };
            if (debit)
            {
                var debitRow = detail with { TaiKhoanDoiUng = posting.TaiKhoanCo, PhatSinhNo = debitAmount };
                // Khi cả hai vế cùng thuộc tài khoản, ghi số dư sau khi đã phản ánh cả định khoản.
                rows.Add(credit ? debitRow : WithBalance(debitRow, balance));
            }
            if (credit)
                rows.Add(WithBalance(detail with { TaiKhoanDoiUng = posting.TaiKhoanNo, PhatSinhCo = creditAmount }, balance));
        }

        rows.Add(Total("Cộng số phát sinh", periodDebit, periodCredit));
        rows.Add(Total("Cộng lũy kế từ đầu năm", priorDebit + periodDebit, priorCredit + periodCredit));
        rows.Add(WithBalance(new() { DienGiai = "Số dư cuối kỳ", InDam = true }, balance));
        return rows;
    }

    private static S38HRow WithBalance(S38HRow row, decimal balance) => row with
    {
        DuNo = balance >= 0 ? balance : null,
        DuCo = balance < 0 ? -balance : null
    };

    private static S38HRow Total(string label, decimal debit, decimal credit) => new()
    {
        DienGiai = label, PhatSinhNo = debit, PhatSinhCo = credit, InDam = true
    };

    public static List<S38HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S38HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
        CoDauNam = 50_000_000m,
        NgayKy = new(2026, 6, 30),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 1), MaLoai = 16, TaiKhoanNo = "331", TaiKhoanCo = "421", SoTien = 2_000_000m },
            new() { NgayGhiSo = new(2026, 3, 5), SoChungTu = "PC001",
                TaiKhoanNo = "331", TaiKhoanCo = "112", SoTien = 15_000_000m },
            new() { NgayGhiSo = new(2026, 4, 8), NgayChungTu = new(2026, 4, 7), SoChungTu = "MH042",
                DienGiai = "Mua vật tư chưa thanh toán", TaiKhoanNo = "152", TaiKhoanCo = "331", SoTien = 25_000_000m, GhiChu = "Hợp đồng 04" },
            new() { NgayGhiSo = new(2026, 5, 12), NgayChungTu = new(2026, 5, 12), SoChungTu = "UNC051",
                DienGiai = "Thanh toán tiền mua vật tư", TaiKhoanNo = "331", TaiKhoanCo = "112", SoTien = 30_000_000m },
            new() { NgayGhiSo = new(2026, 6, 18), NgayChungTu = new(2026, 6, 17), SoChungTu = "MH061",
                DienGiai = "Ghi nhận công nợ dịch vụ", TaiKhoanNo = "611", TaiKhoanCo = "331", SoTien = 8_000_000m }
        ]
    };
}

public sealed record S38HPosting
{
    public string MaDonVi { get; init; } = "VP";
    public DateTime NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanNo { get; init; } = "";
    public string TaiKhoanCo { get; init; } = "";
    public decimal SoTien { get; init; }
    public int MaLoai { get; init; }
    public string GhiChu { get; init; } = "";
}

public sealed record S38HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanDoiUng { get; init; } = "";
    public decimal PhatSinhNo { get; init; }
    public decimal PhatSinhCo { get; init; }
    public decimal? DuNo { get; init; }
    public decimal? DuCo { get; init; }
    public string GhiChu { get; init; } = "";
    public bool InDam { get; init; }
}
