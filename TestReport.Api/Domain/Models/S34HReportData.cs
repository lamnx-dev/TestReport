namespace TestReport.Api.Domain.Models;

public sealed class S34HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public string TaiKhoan { get; set; } = "131";
    public string TenTaiKhoan { get; set; } = "Phải thu của khách hàng";
    // Đối tượng thuộc nhóm danh mục chi tiết 1 của tài khoản được chọn.
    public string MaDoiTuong { get; set; } = "A";
    public string TenDoiTuong { get; set; } = "Đơn vị A";
    public string MaDonVi { get; set; } = "VP";
    public DateTime TuNgay { get; set; } = new(2026, 1, 1);
    public DateTime DenNgay { get; set; } = new(2026, 3, 31);
    public decimal NoDauNam { get; set; }
    public decimal CoDauNam { get; set; }
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Số dư tháng 01 trước điều chỉnh phải cùng phạm vi đơn vị, tài khoản và đối tượng.
    // Truyền đủ định khoản từ đầu năm đến hết kỳ.
    public List<S34HPosting> Items { get; set; } = [];

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

    public List<S34HRow> CreateRows()
    {
        if (string.IsNullOrWhiteSpace(TaiKhoan) || !TaiKhoan.All(char.IsAsciiDigit) ||
            !(TaiKhoan.StartsWith("131", StringComparison.Ordinal) || TaiKhoan.StartsWith("331", StringComparison.Ordinal)))
            throw new ArgumentException("Chọn tài khoản thuộc nhóm 131 hoặc 331.");
        if (TuNgay.Date > DenNgay.Date || TuNgay.Year != DenNgay.Year)
            throw new ArgumentException("Kỳ báo cáo phải nằm trong cùng năm và ngày bắt đầu không sau ngày kết thúc.");
        if (string.IsNullOrWhiteSpace(MaDonVi) || string.IsNullOrWhiteSpace(MaDoiTuong) || Items is null || Items.Any(x => x is null ||
            x.NgayGhiSo == default || string.IsNullOrWhiteSpace(x.MaDonVi) ||
            string.IsNullOrWhiteSpace(x.TaiKhoanNo) || string.IsNullOrWhiteSpace(x.TaiKhoanCo)))
            throw new ArgumentException("Thiếu phạm vi đơn vị, đối tượng hoặc dữ liệu định khoản.");

        var entries = Items.Where(x => x.MaDonVi == MaDonVi && x.NgayGhiSo.Year == TuNgay.Year && x.NgayGhiSo.Date <= DenNgay.Date)
            .OrderBy(x => x.NgayGhiSo)
            .Select(x => (Posting: x, No: x.TaiKhoanNo.StartsWith(TaiKhoan, StringComparison.Ordinal) && x.NoDoiTuong == MaDoiTuong,
                Co: x.TaiKhoanCo.StartsWith(TaiKhoan, StringComparison.Ordinal) && x.CoDoiTuong == MaDoiTuong))
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

        var rows = new List<S34HRow>
        {
            WithBalance(new() { DienGiai = "Số dư đầu kỳ", InDam = true }, opening),
            Total("Điều chỉnh số dư đầu năm", adjustmentDebit, adjustmentCredit)
        };

        var balance = opening;
        var details = new List<(S34HRow Row, int Month, bool Adjustment)>();
        var period = entries.Where(x => x.Posting.NgayGhiSo.Date >= TuNgay.Date).ToList();
        // Tính số dư theo ngày ghi sổ trước khi tách nhóm in để điều chỉnh muộn không ảnh hưởng tháng trước.
        foreach (var entry in period)
            AppendPosting(entry.Posting, entry.No, entry.Co);
        rows.AddRange(details.Where(x => x.Adjustment).Select(x => x.Row));

        var normal = period.Where(x => x.Posting.MaLoai != 16).ToList();
        for (var month = TuNgay.Month; month <= DenNgay.Month; month++)
        {
            var monthly = normal.Where(x => x.Posting.NgayGhiSo.Month == month).ToList();
            rows.AddRange(details.Where(x => !x.Adjustment && x.Month == month).Select(x => x.Row));
            var monthBalance = opening + period.Where(x => x.Posting.NgayGhiSo.Month <= month)
                .Sum(x => (x.No ? x.Posting.SoTien : 0) - (x.Co ? x.Posting.SoTien : 0));
            rows.Add(WithBalance(Total($"Cộng phát sinh tháng {month:00}",
                monthly.Sum(x => x.No ? x.Posting.SoTien : 0), monthly.Sum(x => x.Co ? x.Posting.SoTien : 0)), monthBalance));
        }
        var periodDebit = normal.Sum(x => x.No ? x.Posting.SoTien : 0);
        var periodCredit = normal.Sum(x => x.Co ? x.Posting.SoTien : 0);
        rows.Add(Total("Cộng lũy kế từ đầu năm", priorDebit + periodDebit, priorCredit + periodCredit));
        rows.Add(WithBalance(new() { DienGiai = "Số dư cuối kỳ", InDam = true }, balance));
        return rows;

        void AppendPosting(S34HPosting posting, bool debit, bool credit)
        {
            var debitAmount = debit ? posting.SoTien : 0;
            var creditAmount = credit ? posting.SoTien : 0;
            balance += debitAmount - creditAmount;

            var detail = new S34HRow
            {
                NgayGhiSo = posting.NgayGhiSo,
                NgayChungTu = posting.NgayChungTu,
                SoChungTu = posting.SoChungTu,
                DienGiai = posting.DienGiai
            };
            if (debit)
            {
                var debitRow = detail with { TaiKhoanDoiUng = posting.TaiKhoanCo, PhatSinhNo = debitAmount };
                // Khi cả hai vế cùng thuộc tài khoản, ghi số dư sau khi đã phản ánh cả định khoản.
                Add(credit ? debitRow : WithBalance(debitRow, balance));
            }
            if (credit)
                Add(WithBalance(detail with { TaiKhoanDoiUng = posting.TaiKhoanNo, PhatSinhCo = creditAmount }, balance));

            void Add(S34HRow row) => details.Add((row, posting.NgayGhiSo.Month, posting.MaLoai == 16));
        }
    }

    private static S34HRow WithBalance(S34HRow row, decimal balance) => row with
    {
        DuNo = balance >= 0 ? balance : null,
        DuCo = balance < 0 ? -balance : null
    };

    private static S34HRow Total(string label, decimal debit, decimal credit) => new()
    {
        DienGiai = label, PhatSinhNo = debit, PhatSinhCo = credit, InDam = true
    };

    public static List<S34HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S34HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 3, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 10), NgayChungTu = new(2026, 1, 9), SoChungTu = "PT001", DienGiai = "Phải thu của đơn vị A", TaiKhoanNo = "131", NoDoiTuong = "A", TaiKhoanCo = "531", SoTien = 50_000_000m },
            new() { NgayGhiSo = new(2026, 1, 20), NgayChungTu = new(2026, 1, 20), SoChungTu = "BC001", DienGiai = "Đơn vị A đã thanh toán", TaiKhoanNo = "112", TaiKhoanCo = "131", CoDoiTuong = "A", SoTien = 20_000_000m },
            new() { NgayGhiSo = new(2026, 2, 10), NgayChungTu = new(2026, 2, 10), SoChungTu = "PT002", DienGiai = "Phải thu của đơn vị A lần 2", TaiKhoanNo = "131", NoDoiTuong = "A", TaiKhoanCo = "531", SoTien = 30_000_000m }
        ]
    };
}

public sealed record S34HPosting
{
    public string MaDonVi { get; init; } = "VP";
    public DateTime NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanNo { get; init; } = "";
    public string TaiKhoanCo { get; init; } = "";
    public string NoDoiTuong { get; init; } = "";
    public string CoDoiTuong { get; init; } = "";
    public decimal SoTien { get; init; }
    public int MaLoai { get; init; }
}

public sealed record S34HRow
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
    public bool InDam { get; init; }
}
