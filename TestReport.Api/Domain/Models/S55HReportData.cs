namespace TestReport.Api.Domain.Models;

public sealed class S55HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public int Nam { get; set; } = 2026;
    // Số thuế còn được miễn giảm, chưa thực hiện tại thời điểm đầu năm.
    public decimal SoDuDauKy { get; set; }
    public decimal DieuChinhDauNam { get; set; }
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Phát sinh đã được phân loại; báo cáo không tự suy diễn bút toán thuế.
    public List<S55HRow> Items { get; set; } = [];

    public List<S55HRow> CreateRows()
    {
        if (Nam is < 1 or > 9999) throw new ArgumentException("Năm báo cáo không hợp lệ.");
        if (Items is null || Items.Any(x => x is null || x.NgayGhiSo is null || x.NgayGhiSo.Value.Year != Nam))
            throw new ArgumentException("Ngày ghi sổ của phát sinh phải thuộc năm báo cáo.");
        var rows = new List<S55HRow>
        {
            new() { DienGiai = "Số dư đầu kỳ", SoDu = SoDuDauKy, InDam = true },
            new() { DienGiai = "Điều chỉnh số dư đầu năm", SoDu = DieuChinhDauNam, InDam = true }
        };
        foreach (var month in Items.OrderBy(x => x.NgayGhiSo).GroupBy(x => x.NgayGhiSo!.Value.Month))
        {
            rows.AddRange(month.Select(x => x with { SoDu = null, InDam = false }));
            rows.Add(new() { DienGiai = $"Cộng phát sinh tháng {month.Key:00}", InDam = true,
                DuocMienGiam = month.Sum(x => x.DuocMienGiam), DaMienGiam = month.Sum(x => x.DaMienGiam) });
        }
        var duoc = Items.Sum(x => x.DuocMienGiam);
        var da = Items.Sum(x => x.DaMienGiam);
        rows.Add(new() { DienGiai = "Cộng luỹ kế từ đầu năm", DuocMienGiam = duoc, DaMienGiam = da, InDam = true });
        rows.Add(new() { DienGiai = "Số dư cuối kỳ", SoDu = SoDuDauKy + DieuChinhDauNam + duoc - da, InDam = true });
        return rows;
    }

    public static List<S55HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S55HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI", Nam = 2026,
        SoDuDauKy = 5_000_000m, DieuChinhDauNam = 500_000m,
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 12, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 10), NgayChungTu = new(2026, 1, 9), SoChungTu = "MG001", DienGiai = "Ghi nhận số thuế GTGT được miễn giảm theo quyết định", DuocMienGiam = 12_000_000m },
            new() { NgayGhiSo = new(2026, 1, 31), NgayChungTu = new(2026, 1, 31), SoChungTu = "MG002", DienGiai = "Thực hiện miễn giảm thuế GTGT tháng 01", DaMienGiam = 8_000_000m },
            new() { NgayGhiSo = new(2026, 2, 10), NgayChungTu = new(2026, 2, 10), SoChungTu = "MG003", DienGiai = "Ghi nhận bổ sung số thuế GTGT được miễn giảm", DuocMienGiam = 6_000_000m },
            new() { NgayGhiSo = new(2026, 2, 28), NgayChungTu = new(2026, 2, 28), SoChungTu = "MG004", DienGiai = "Thực hiện miễn giảm thuế GTGT tháng 02", DaMienGiam = 10_000_000m }
        ]
    };
}

public sealed record S55HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public decimal DuocMienGiam { get; init; }
    public decimal DaMienGiam { get; init; }
    public decimal? SoDu { get; init; }
    public decimal SoTienDuocMienGiam => SoDu ?? DuocMienGiam;
    public bool InDam { get; init; }
}
