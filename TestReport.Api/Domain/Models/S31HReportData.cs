namespace TestReport.Api.Domain.Models;

public sealed class S31HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    // Nguồn được chọn từ danh mục tài khoản 1353% tại cấp theo dõi của đơn vị.
    public string TaiKhoan { get; set; } = "13531";
    public string TenTaiKhoan { get; set; } = "Nguồn kinh phí hoạt động";
    public string MaDonVi { get; set; } = "VP";
    public DateTime TuNgay { get; set; } = new(2026, 1, 1);
    public DateTime DenNgay { get; set; } = new(2026, 1, 31);
    public decimal NoDauNam { get; set; }
    public decimal CoDauNam { get; set; }
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Số dư tháng 01 trước điều chỉnh phải cùng phạm vi đơn vị và tài khoản nguồn đã chọn.
    // Truyền đủ định khoản từ đầu năm đến hết kỳ.
    public List<S31HPosting> Items { get; set; } = [];

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

    public List<S31HRow> CreateRows()
    {
        if (string.IsNullOrWhiteSpace(TaiKhoan) || !TaiKhoan.All(char.IsAsciiDigit) ||
            !TaiKhoan.StartsWith("1353", StringComparison.Ordinal))
            throw new ArgumentException("Chọn nguồn thuộc nhóm tài khoản 1353.");
        if (TuNgay.Date > DenNgay.Date || TuNgay.Year != DenNgay.Year)
            throw new ArgumentException("Kỳ báo cáo phải nằm trong cùng năm và ngày bắt đầu không sau ngày kết thúc.");
        if (string.IsNullOrWhiteSpace(MaDonVi) || Items is null || Items.Any(x => x is null ||
            x.NgayGhiSo == default || string.IsNullOrWhiteSpace(x.MaDonVi) ||
            string.IsNullOrWhiteSpace(x.TaiKhoanNo) || string.IsNullOrWhiteSpace(x.TaiKhoanCo)))
            throw new ArgumentException("Thiếu đơn vị hoặc dữ liệu định khoản.");

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

        var rows = new List<S31HRow>
        {
            WithBalance(new() { DienGiai = "Số dư đầu kỳ", InDam = true }, opening),
            Total("Điều chỉnh số dư đầu năm", adjustmentDebit, adjustmentCredit)
        };

        var balance = opening;
        var details = new List<(S31HRow Row, int Month, bool Adjustment)>();
        var period = entries.Where(x => x.Posting.NgayGhiSo.Date >= TuNgay.Date).ToList();
        // Tính số dư theo ngày ghi sổ trước khi tách nhóm in để điều chỉnh muộn không ảnh hưởng tháng trước.
        foreach (var entry in period)
            AppendPosting(entry.Posting, entry.No, entry.Co);
        rows.AddRange(details.Where(x => x.Adjustment).Select(x => x.Row));
        rows.Add(new() { DienGiai = "Số phát sinh", InDam = true });

        var normal = period.Where(x => x.Posting.MaLoai != 16).ToList();
        for (var month = TuNgay.Month; month <= DenNgay.Month; month++)
        {
            var monthly = normal.Where(x => x.Posting.NgayGhiSo.Month == month).ToList();
            rows.AddRange(details.Where(x => !x.Adjustment && x.Month == month).Select(x => x.Row));
            var monthBalance = opening + period.Where(x => x.Posting.NgayGhiSo.Month <= month)
                .Sum(x => (x.No ? x.Posting.SoTien : 0) - (x.Co ? x.Posting.SoTien : 0));
            rows.Add(Total($"Cộng phát sinh tháng {month:00}",
                monthly.Sum(x => x.No ? x.Posting.SoTien : 0), monthly.Sum(x => x.Co ? x.Posting.SoTien : 0)));
            rows.Add(WithBalance(new() { DienGiai = $"Số dư cuối tháng {month:00}", InDam = true }, monthBalance));
        }
        var periodDebit = normal.Sum(x => x.No ? x.Posting.SoTien : 0);
        var periodCredit = normal.Sum(x => x.Co ? x.Posting.SoTien : 0);
        rows.Add(Total("Cộng lũy kế từ đầu năm", priorDebit + periodDebit, priorCredit + periodCredit));
        rows.Add(WithBalance(new() { DienGiai = "Số dư cuối kỳ", InDam = true }, balance));
        return rows;

        void AppendPosting(S31HPosting posting, bool debit, bool credit)
        {
            var debitAmount = debit ? posting.SoTien : 0;
            var creditAmount = credit ? posting.SoTien : 0;
            balance += debitAmount - creditAmount;

            var detail = new S31HRow
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
                Add(debitRow);
            }
            if (credit)
                Add(detail with { TaiKhoanDoiUng = posting.TaiKhoanNo, PhatSinhCo = creditAmount });

            void Add(S31HRow row) => details.Add((row, posting.NgayGhiSo.Month, posting.MaLoai == 16));
        }
    }

    private static S31HRow WithBalance(S31HRow row, decimal balance) => row with
    {
        DuNo = balance >= 0 ? balance : null,
        DuCo = balance < 0 ? -balance : null
    };

    private static S31HRow Total(string label, decimal debit, decimal credit) => new()
    {
        DienGiai = label, PhatSinhNo = debit, PhatSinhCo = credit, InDam = true
    };

    public static List<S31HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S31HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
        NoDauNam = 50_000_000m,
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 1, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 1), NgayChungTu = new(2026, 1, 1), SoChungTu = "DC001", DienGiai = "Điều chỉnh phải thu kinh phí đầu năm", MaLoai = 16, TaiKhoanNo = "13531", TaiKhoanCo = "421", SoTien = 2_000_000m },
            new() { NgayGhiSo = new(2026, 1, 10), NgayChungTu = new(2026, 1, 9), SoChungTu = "KP001", DienGiai = "Ghi nhận kinh phí được cấp phải thu", TaiKhoanNo = "13531", TaiKhoanCo = "511", SoTien = 100_000_000m, GhiChu = "Quyết định cấp kinh phí 01" },
            new() { NgayGhiSo = new(2026, 1, 20), NgayChungTu = new(2026, 1, 20), SoChungTu = "BC001", DienGiai = "Nhận kinh phí được cấp", TaiKhoanNo = "112", TaiKhoanCo = "13531", SoTien = 70_000_000m },
            new() { NgayGhiSo = new(2026, 1, 25), NgayChungTu = new(2026, 1, 25), SoChungTu = "KP002", DienGiai = "Ghi nhận bổ sung kinh phí phải thu", TaiKhoanNo = "135311", TaiKhoanCo = "511", SoTien = 30_000_000m }
        ]
    };
}

public sealed record S31HPosting
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

public sealed record S31HRow
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
