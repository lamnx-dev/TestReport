namespace TestReport.Api.Domain.Models;

public sealed class S90HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số 153/BHXH-TCKT ngày 14/01/2026 của BHXH Việt Nam)";
    public string KyBaoCao { get; set; } = "Tháng/Quý/Năm";
    public string DonViTinh { get; set; } = "đồng";
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string ChucDanhKeToan { get; set; } = "KẾ TOÁN";
    public string ChucDanhKeToanTruong { get; set; } = "KẾ TOÁN TRƯỞNG";
    public string ChucDanhThuTruong { get; set; } = "THỦ TRƯỞNG ĐƠN VỊ";
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    public List<S90HChiTieu> ChiTieu { get; set; } = [];

    private static readonly S90HRow[] Template =
    [
        new("I", "Kinh phí được giao tự chủ", "", true),
        new("1", "Dự toán được giao trong năm", "01"),
        new("2", "Số thu trong năm", "02", true),
        new("2.1", "Kinh phí do BHXH Việt Nam cấp", "03"),
        new("2.2", "Kinh phí khác", "04"),
        new("II", "Chi hoạt động được giao tự chủ trong năm", "05", true),
        new("III", "Số trích khấu hao tài sản cố định", "06", true),
        new("", "Số trích khấu hao bổ sung Quỹ phát triển hoạt động sự nghiệp", "khau-hao-bo-sung"),
        new("IV", "Số trích lập nguồn cải cách tiền lương", "07", true),
        new("V", "Nộp thuế và các khoản nộp ngân sách nhà nước theo quy định", "08", true),
        new("VI", "Kinh phí chuyển năm sau", "09", true),
        new("VII", "Số chênh lệch thu, chi/tiết kiệm chi cuối năm theo cơ chế tài chính", "10", true),
        new("", "Trích lập các quỹ", "", true),
        new("7.1", "Quỹ phát triển hoạt động sự nghiệp", "10.1"),
        new("7.2", "Quỹ bổ sung thu nhập", "10.2"),
        new("7.3", "Quỹ phúc lợi", "10.3")
    ];

    public List<S90HRow> CreateRows()
    {
        if (ChiTieu is null) throw new ArgumentException("ChiTieu không được null.");
        var values = new Dictionary<string, S90HChiTieu>(StringComparer.Ordinal);
        foreach (var item in ChiTieu)
        {
            if (item is null || string.IsNullOrEmpty(item.MaChiTieu) ||
                !Template.Any(row => row.MaChiTieu == item.MaChiTieu))
                throw new ArgumentException("Mã chỉ tiêu không được hỗ trợ.");
            if (!values.TryAdd(item.MaChiTieu, item))
                throw new ArgumentException($"Mã chỉ tiêu bị trùng: {item.MaChiTieu}.");
        }
        return Template.Select(row => values.TryGetValue(row.MaChiTieu, out var item)
            ? row with { SoTien = item.SoTien, GhiChu = item.GhiChu }
            : row with { }).ToList();
    }

    public static S90HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
        KyBaoCao = "Năm 2026", NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 12, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        ChiTieu =
        [
            new() { MaChiTieu = "01", SoTien = 1_000_000_000m },
            new() { MaChiTieu = "02", SoTien = 950_000_000m },
            new() { MaChiTieu = "03", SoTien = 900_000_000m },
            new() { MaChiTieu = "04", SoTien = 50_000_000m },
            new() { MaChiTieu = "05", SoTien = 700_000_000m },
            new() { MaChiTieu = "06", SoTien = 0m },
            new() { MaChiTieu = "khau-hao-bo-sung", SoTien = 0m },
            new() { MaChiTieu = "07", SoTien = 50_000_000m },
            new() { MaChiTieu = "08", SoTien = 10_000_000m },
            new() { MaChiTieu = "09", SoTien = 90_000_000m },
            new() { MaChiTieu = "10", SoTien = 100_000_000m },
            new() { MaChiTieu = "10.1", SoTien = 40_000_000m },
            new() { MaChiTieu = "10.2", SoTien = 35_000_000m },
            new() { MaChiTieu = "10.3", SoTien = 25_000_000m }
        ]
    };

    public static List<S90HRow> GetSampleRows() => CreateSample().CreateRows();
}

public sealed class S90HChiTieu
{
    public string MaChiTieu { get; set; } = "";
    public decimal? SoTien { get; set; }
    public string GhiChu { get; set; } = "";
}

public sealed record S90HRow(string Stt, string TenChiTieu, string MaChiTieu, bool InDam = false)
{
    public string MaIn => MaChiTieu == "khau-hao-bo-sung" ? "" : MaChiTieu;
    public decimal? SoTien { get; init; }
    public string GhiChu { get; init; } = "";
}
