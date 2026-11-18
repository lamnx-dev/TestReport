namespace TestReport.Api.Domain.Models;

public sealed class S32HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public string TaiKhoan => "242";
    // Danh mục nhóm 123, chi tiết 1 của tài khoản 242, theo mẫu Excel.
    public static IReadOnlyDictionary<string, string> CacLoaiPhanBo { get; } =
        new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            ["01"] = "Công cụ, dụng cụ", ["02"] = "Chi phí sửa chữa TSCĐ", ["03"] = "Trả trước tiền thuê TSCĐ",
            ["04"] = "Chi phí mua các loại bảo hiểm, phí, lệ phí", ["05"] = "Chi phí trả trước khác"
        });
    public string MaLoaiPhanBo { get; set; } = "01";
    public string TenLoaiPhanBo => MaLoaiPhanBo is not null && CacLoaiPhanBo.TryGetValue(MaLoaiPhanBo, out var name) ? name : "";
    public string MaDonVi { get; set; } = "VP";
    public DateTime TuNgay { get; set; } = new(2026, 1, 1);
    public DateTime DenNgay { get; set; } = new(2026, 3, 31);
    public decimal SoDuDauNam { get; set; }
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Số dư tháng 01 trước điều chỉnh phải cùng phạm vi đơn vị, tài khoản 242 và loại phân bổ.
    // Truyền đủ định khoản từ đầu năm đến hết kỳ.
    public List<S32HPosting> Items { get; set; } = [];

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

    public List<S32HRow> CreateRows()
    {
        if (string.IsNullOrWhiteSpace(MaLoaiPhanBo) || !CacLoaiPhanBo.ContainsKey(MaLoaiPhanBo))
            throw new ArgumentException("Chọn loại phân bổ từ 01 đến 05 thuộc danh mục nhóm 123.");
        if (TuNgay.Date > DenNgay.Date || TuNgay.Year != DenNgay.Year)
            throw new ArgumentException("Kỳ báo cáo phải nằm trong cùng năm và ngày bắt đầu không sau ngày kết thúc.");
        if (string.IsNullOrWhiteSpace(MaDonVi) || string.IsNullOrWhiteSpace(MaLoaiPhanBo) || Items is null || Items.Any(x => x is null ||
            x.NgayGhiSo == default || string.IsNullOrWhiteSpace(x.MaDonVi) ||
            string.IsNullOrWhiteSpace(x.TaiKhoanNo) || string.IsNullOrWhiteSpace(x.TaiKhoanCo)))
            throw new ArgumentException("Thiếu đơn vị, loại phân bổ hoặc dữ liệu định khoản.");

        var entries = Items.Where(x => x.MaDonVi == MaDonVi && x.NgayGhiSo.Year == TuNgay.Year && x.NgayGhiSo.Date <= DenNgay.Date)
            .OrderBy(x => x.NgayGhiSo)
            .Select(x => (Posting: x, No: x.TaiKhoanNo.StartsWith(TaiKhoan, StringComparison.Ordinal) && x.NoLoaiPhanBo == MaLoaiPhanBo,
                Co: x.TaiKhoanCo.StartsWith(TaiKhoan, StringComparison.Ordinal) && x.CoLoaiPhanBo == MaLoaiPhanBo))
            .Where(x => x.No || x.Co)
            .ToList();

        var opening = SoDuDauNam;
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

        var rows = new List<S32HRow>
        {
            WithBalance(new() { DienGiai = "Số dư đầu kỳ", InDam = true }, opening),
            Total("Điều chỉnh số dư đầu năm", adjustmentDebit, adjustmentCredit)
        };

        var balance = opening;
        var details = new List<(S32HRow Row, int Month, bool Adjustment)>();
        var period = entries.Where(x => x.Posting.NgayGhiSo.Date >= TuNgay.Date).ToList();
        // Tính số dư theo ngày ghi sổ trước khi tách nhóm in để điều chỉnh muộn không ảnh hưởng tháng trước.
        foreach (var entry in period)
            AppendPosting(entry.Posting, entry.No, entry.Co);
        rows.AddRange(details.Where(x => x.Adjustment).Select(x => x.Row));
        rows.Add(new() { DienGiai = "Số phát sinh trong kỳ", InDam = true });

        var normal = period.Where(x => x.Posting.MaLoai != 16).ToList();
        for (var month = TuNgay.Month; month <= DenNgay.Month; month++)
        {
            var monthly = normal.Where(x => x.Posting.NgayGhiSo.Month == month).ToList();
            rows.AddRange(details.Where(x => !x.Adjustment && x.Month == month).Select(x => x.Row));
            rows.Add(Total($"Cộng phát sinh tháng {month:00}",
                monthly.Sum(x => x.No ? x.Posting.SoTien : 0), monthly.Sum(x => x.Co ? x.Posting.SoTien : 0)));
        }
        var periodDebit = normal.Sum(x => x.No ? x.Posting.SoTien : 0);
        var periodCredit = normal.Sum(x => x.Co ? x.Posting.SoTien : 0);
        rows.Add(Total("Cộng lũy kế từ đầu năm", priorDebit + periodDebit, priorCredit + periodCredit));
        rows.Add(WithBalance(new() { DienGiai = "Số dư cuối kỳ", InDam = true }, balance));
        return rows;

        void AppendPosting(S32HPosting posting, bool debit, bool credit)
        {
            var debitAmount = debit ? posting.SoTien : 0;
            var creditAmount = credit ? posting.SoTien : 0;
            balance += debitAmount - creditAmount;

            var detail = new S32HRow
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
                Add(credit ? debitRow : WithBalance(debitRow, balance));
            }
            if (credit)
                Add(WithBalance(detail with { TaiKhoanDoiUng = posting.TaiKhoanNo, PhatSinhCo = creditAmount }, balance));

            void Add(S32HRow row) => details.Add((row, posting.NgayGhiSo.Month, posting.MaLoai == 16));
        }
    }

    private static S32HRow WithBalance(S32HRow row, decimal balance) => row with
    {
        SoDu = balance
    };

    private static S32HRow Total(string label, decimal debit, decimal credit) => new()
    {
        DienGiai = label, PhatSinhNo = debit, PhatSinhCo = credit, InDam = true
    };

    public static List<S32HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S32HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
        SoDuDauNam = 24_000_000m,
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 3, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 1), NgayChungTu = new(2026, 1, 1), SoChungTu = "DC001", DienGiai = "Điều chỉnh chi phí trả trước đầu năm", MaLoai = 16, TaiKhoanNo = "2421", NoLoaiPhanBo = "01", TaiKhoanCo = "421", SoTien = 1_000_000m },
            new() { NgayGhiSo = new(2026, 1, 10), NgayChungTu = new(2026, 1, 9), SoChungTu = "CP001", DienGiai = "Ghi nhận công cụ, dụng cụ chờ phân bổ", TaiKhoanNo = "2421", NoLoaiPhanBo = "01", TaiKhoanCo = "153", SoTien = 12_000_000m, GhiChu = "Công cụ CCDC01" },
            new() { NgayGhiSo = new(2026, 2, 28), NgayChungTu = new(2026, 2, 28), SoChungTu = "PB002", DienGiai = "Phân bổ chi phí công cụ, dụng cụ", TaiKhoanNo = "611", TaiKhoanCo = "2421", CoLoaiPhanBo = "01", SoTien = 3_000_000m },
            new() { NgayGhiSo = new(2026, 3, 31), NgayChungTu = new(2026, 3, 31), SoChungTu = "PB003", DienGiai = "Phân bổ chi phí công cụ, dụng cụ", TaiKhoanNo = "642", TaiKhoanCo = "2421", CoLoaiPhanBo = "01", SoTien = 3_000_000m }
        ]
    };
}

public sealed record S32HPosting
{
    public string MaDonVi { get; init; } = "VP";
    public DateTime NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanNo { get; init; } = "";
    public string TaiKhoanCo { get; init; } = "";
    public string NoLoaiPhanBo { get; init; } = "";
    public string CoLoaiPhanBo { get; init; } = "";
    public decimal SoTien { get; init; }
    public int MaLoai { get; init; }
    public string GhiChu { get; init; } = "";
}

public sealed record S32HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanDoiUng { get; init; } = "";
    public decimal PhatSinhNo { get; init; }
    public decimal PhatSinhCo { get; init; }
    public decimal? SoDu { get; init; }
    public string GhiChu { get; init; } = "";
    public bool InDam { get; init; }
}
