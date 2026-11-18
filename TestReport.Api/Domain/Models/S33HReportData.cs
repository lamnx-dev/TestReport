namespace TestReport.Api.Domain.Models;

public sealed class S33HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public string MaDonVi { get; set; } = "VP";
    // Đơn vị nội bộ thuộc danh mục chi tiết 1 của tài khoản 136/336.
    public string MaDonViNoiBo { get; set; } = "001";
    public string TenDonViNoiBo { get; set; } = "Đơn vị nội bộ 001";
    public DateTime TuNgay { get; set; } = new(2026, 1, 1);
    public DateTime DenNgay { get; set; } = new(2026, 3, 31);
    // Số dư tháng 01 trước điều chỉnh của đúng đơn vị báo cáo và đơn vị nội bộ đã chọn.
    public decimal No136DauNam { get; set; }
    public decimal Co136DauNam { get; set; }
    public decimal No336DauNam { get; set; }
    public decimal Co336DauNam { get; set; }
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Truyền đầy đủ từng dòng định khoản từ đầu năm để tính số dư đầu kỳ và lũy kế.
    public List<S33HPosting> Items { get; set; } = [];

    public string KyBaoCao
    {
        get
        {
            if (TuNgay.Year == DenNgay.Year && TuNgay.Day == 1 && DenNgay.Day == DateTime.DaysInMonth(DenNgay.Year, DenNgay.Month))
            {
                if (TuNgay.Month == DenNgay.Month) return FormattableString.Invariant($"Tháng {TuNgay:MM'/'yyyy}");
                if (TuNgay.Month == 1 && DenNgay.Month == 12) return $"Năm {TuNgay.Year}";
                if ((TuNgay.Month - 1) % 3 == 0 && DenNgay.Month == TuNgay.Month + 2) return $"Quý {(TuNgay.Month + 2) / 3}/{TuNgay.Year}";
            }
            return FormattableString.Invariant($"Từ ngày {TuNgay:dd'/'MM'/'yyyy} đến ngày {DenNgay:dd'/'MM'/'yyyy}");
        }
    }

    public List<S33HRow> CreateRows()
    {
        if (TuNgay.Date > DenNgay.Date || TuNgay.Year != DenNgay.Year)
            throw new ArgumentException("Kỳ báo cáo phải nằm trong cùng năm và ngày bắt đầu không sau ngày kết thúc.");
        if (string.IsNullOrWhiteSpace(MaDonVi) || string.IsNullOrWhiteSpace(MaDonViNoiBo) || Items is null ||
            Items.Any(x => x is null || x.NgayGhiSo == default || string.IsNullOrWhiteSpace(x.MaDonVi) ||
                string.IsNullOrWhiteSpace(x.TaiKhoanNo) || string.IsNullOrWhiteSpace(x.TaiKhoanCo)))
            throw new ArgumentException("Thiếu đơn vị báo cáo, đơn vị nội bộ hoặc dữ liệu định khoản hợp lệ.");

        var entries = new List<(S33HRow Row, DateTime Date, bool Adjustment)>();
        foreach (var posting in Items.Where(x => x.MaDonVi == MaDonVi && x.NgayGhiSo.Year == TuNgay.Year && x.NgayGhiSo.Date <= DenNgay.Date)
            .OrderBy(x => x.NgayGhiSo))
        {
            if (posting.NoDonViNoiBo == MaDonViNoiBo) Add(posting.TaiKhoanNo, true);
            if (posting.CoDonViNoiBo == MaDonViNoiBo) Add(posting.TaiKhoanCo, false);

            void Add(string account, bool debit)
            {
                var receivable = account.StartsWith("136", StringComparison.Ordinal);
                var payable = account.StartsWith("336", StringComparison.Ordinal);
                if (!receivable && !payable) return;
                entries.Add((new()
                {
                    NgayGhiSo = posting.NgayGhiSo, NgayChungTu = posting.NgayChungTu,
                    SoChungTu = posting.SoChungTu, DienGiai = posting.DienGiai, GhiChu = posting.GhiChu,
                    TaiKhoanDoiUng = debit ? posting.TaiKhoanCo : posting.TaiKhoanNo,
                    No136 = receivable && debit ? posting.SoTien : 0, Co136 = receivable && !debit ? posting.SoTien : 0,
                    No336 = payable && debit ? posting.SoTien : 0, Co336 = payable && !debit ? posting.SoTien : 0
                }, posting.NgayGhiSo.Date, posting.MaLoai == 16));
            }
        }
        var prior = entries.Where(x => x.Date < TuNgay.Date).Select(x => x.Row).ToList();
        var opening136 = No136DauNam - Co136DauNam + prior.Sum(x => x.No136 - x.Co136);
        var opening336 = No336DauNam - Co336DauNam + prior.Sum(x => x.No336 - x.Co336);
        var period = entries.Where(x => x.Date >= TuNgay.Date).ToList();
        var adjustments = period.Where(x => x.Adjustment).Select(x => x.Row).ToList();
        var normal = period.Where(x => !x.Adjustment).ToList();
        List<S33HRow> rows =
        [
            Balance("Số dư đầu kỳ", opening136, opening336),
            Total("Điều chỉnh số dư đầu năm", adjustments),
            .. adjustments
        ];
        for (var month = TuNgay.Month; month <= DenNgay.Month; month++)
        {
            var monthly = normal.Where(x => x.Date.Month == month).Select(x => x.Row).ToList();
            rows.AddRange(monthly);
            rows.Add(Total($"Cộng phát sinh tháng {month:00}", monthly));
        }
        rows.Add(Total("Cộng lũy kế từ đầu năm", entries.Where(x => !x.Adjustment).Select(x => x.Row)));
        rows.Add(Balance("Số dư cuối kỳ", opening136 + period.Sum(x => x.Row.No136 - x.Row.Co136),
            opening336 + period.Sum(x => x.Row.No336 - x.Row.Co336)));
        return rows;
    }

    private static S33HRow Balance(string label, decimal receivable, decimal payable) => new()
    {
        DienGiai = label, InDam = true,
        No136 = Math.Max(receivable, 0), Co136 = Math.Max(-receivable, 0),
        No336 = Math.Max(payable, 0), Co336 = Math.Max(-payable, 0)
    };

    private static S33HRow Total(string label, IEnumerable<S33HRow> source)
    {
        var rows = source.ToList();
        return new() { DienGiai = label, InDam = true, No136 = rows.Sum(x => x.No136), Co136 = rows.Sum(x => x.Co136),
            No336 = rows.Sum(x => x.No336), Co336 = rows.Sum(x => x.Co336) };
    }

    public static List<S33HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S33HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI", No136DauNam = 20_000_000m, Co336DauNam = 12_000_000m,
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 3, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 1), NgayChungTu = new(2026, 1, 1), SoChungTu = "DC001", DienGiai = "Điều chỉnh số phải thu nội bộ đầu năm", MaLoai = 16, TaiKhoanNo = "1361", NoDonViNoiBo = "001", TaiKhoanCo = "421", SoTien = 2_000_000m },
            new() { NgayGhiSo = new(2026, 1, 10), NgayChungTu = new(2026, 1, 9), SoChungTu = "NB001", DienGiai = "Chi hộ đơn vị nội bộ 001", TaiKhoanNo = "1361", NoDonViNoiBo = "001", TaiKhoanCo = "112", SoTien = 30_000_000m, GhiChu = "Chi hộ theo đề nghị 01" },
            new() { NgayGhiSo = new(2026, 1, 20), NgayChungTu = new(2026, 1, 20), SoChungTu = "NB002", DienGiai = "Đơn vị nội bộ hoàn trả tiền chi hộ", TaiKhoanNo = "112", TaiKhoanCo = "1361", CoDonViNoiBo = "001", SoTien = 10_000_000m },
            new() { NgayGhiSo = new(2026, 2, 10), NgayChungTu = new(2026, 2, 10), SoChungTu = "NB003", DienGiai = "Ghi nhận khoản đơn vị nội bộ chi hộ", TaiKhoanNo = "611", TaiKhoanCo = "3361", CoDonViNoiBo = "001", SoTien = 15_000_000m }
        ]
    };
}

public sealed record S33HPosting
{
    public string MaDonVi { get; init; } = "VP";
    public DateTime NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanNo { get; init; } = "";
    public string TaiKhoanCo { get; init; } = "";
    public string NoDonViNoiBo { get; init; } = "";
    public string CoDonViNoiBo { get; init; } = "";
    public decimal SoTien { get; init; }
    public int MaLoai { get; init; }
    public string GhiChu { get; init; } = "";
}

public sealed record S33HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanDoiUng { get; init; } = "";
    public decimal No136 { get; init; }
    public decimal Co136 { get; init; }
    public decimal No336 { get; init; }
    public decimal Co336 { get; init; }
    public string GhiChu { get; init; } = "";
    public bool InDam { get; init; }
}
