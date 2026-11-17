namespace TestReport.Api.Domain.Models;

/// <summary>Values supplied by the accounting system; parent columns include their “Trong đó” columns.</summary>
public sealed class S101HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số ……/BHXH-TCKT ngày …/…/2024 của Bảo hiểm xã hội Việt Nam)";
    public string KyBaoCao { get; set; } = "Năm …";
    public string DonViTinh { get; set; } = "đồng";
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    public List<S101HChiTieu> ChiTieu { get; set; } = [];
    public List<S101HQuyetDinh> QuyetDinh { get; set; } = [];

    private static readonly S101HRow[] Template =
    [
        new("1", "1. Dự toán năm trước sang", true),
        new("1.1", "- Số liệu năm trước"),
        new("1.2", "- Điều chỉnh số năm trước chuyển sang"),
        new("2", "2. Dự toán giao trong năm", true),
        new("3", "3. Dự toán bị hủy (hết nhiệm vụ chi)", true),
        new("4", "4. Số đề nghị quyết toán", true),
        new("5", "5. Số dư dự toán chuyển năm sau", true)
    ];

    public List<S101HRow> CreateRows()
    {
        if (ChiTieu is null || QuyetDinh is null) throw new ArgumentException("Danh sách số liệu không được null.");
        var values = new Dictionary<string, S101HChiTieu>(StringComparer.Ordinal);
        foreach (var item in ChiTieu)
        {
            if (item is null || !Template.Any(r => r.MaChiTieu == item.MaChiTieu))
                throw new ArgumentException("Mã chỉ tiêu không được hỗ trợ.");
            if (!values.TryAdd(item.MaChiTieu, item)) throw new ArgumentException("Mã chỉ tiêu bị trùng: " + item.MaChiTieu);
        }
        var result = new List<S101HRow>();
        foreach (var row in Template)
        {
            result.Add(values.TryGetValue(row.MaChiTieu, out var item)
                ? row with { NgayGhiSo = item.NgayGhiSo, SoChungTu = item.SoChungTu, NgayChungTu = item.NgayChungTu, SoTien = item.SoTien }
                : row with { });
            if (row.MaChiTieu != "2") continue;
            if (QuyetDinh.Count == 0)
            {
                result.Add(new("", "- Quyết định số: ……"));
                result.Add(new("", "- Quyết định số: ……"));
            }
            foreach (var decision in QuyetDinh)
            {
                if (decision is null) throw new ArgumentException("Quyết định không được null.");
                result.Add(new("", "- Quyết định số: " + decision.SoQuyetDinh)
                { NgayGhiSo = decision.NgayGhiSo, SoChungTu = decision.SoChungTu, NgayChungTu = decision.NgayChungTu, SoTien = decision.SoTien });
            }
        }
        return result;
    }

    public static S101HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI", KyBaoCao = "Năm 2026",
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 12, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        ChiTieu =
        [
            new() { MaChiTieu = "1", SoTien = SampleAmounts(100_000_000m) },
            new() { MaChiTieu = "1.1", SoTien = SampleAmounts(110_000_000m) },
            new() { MaChiTieu = "1.2", SoTien = SampleAmounts(-10_000_000m) },
            new() { MaChiTieu = "2", SoTien = SampleAmounts(1_000_000_000m) },
            new() { MaChiTieu = "3", SoTien = SampleAmounts(-50_000_000m) },
            new() { MaChiTieu = "4", SoTien = SampleAmounts(900_000_000m) },
            new() { MaChiTieu = "5", SoTien = SampleAmounts(150_000_000m) }
        ],
        QuyetDinh =
        [
            new() { SoQuyetDinh = "01/QĐ-BHXH", NgayGhiSo = new(2026, 1, 5), SoChungTu = "DT001", NgayChungTu = new(2026, 1, 3), SoTien = SampleAmounts(800_000_000m) },
            new() { SoQuyetDinh = "02/QĐ-BHXH", NgayGhiSo = new(2026, 6, 5), SoChungTu = "DT002", NgayChungTu = new(2026, 6, 3), SoTien = SampleAmounts(200_000_000m) }
        ]
    };

    private static S101HAmounts SampleAmounts(decimal total) => new()
    {
        TongSoTien = total, TuyenTruyen = total * .1m, TrongDoTuyenTruyen = total * .05m,
        CaiCachHanhChinh = total * .2m, HoTroCapXa = total * .1m, TaiSan = total * .3m,
        DauTuPhatTrien = total * .1m, MuaSamSuaChua = total * .15m, CongNgheThongTin = total * .05m,
        HoatDongBoMay = total * .4m, TrongDoHoatDongBoMay = total * .1m
    };
    public static List<S101HRow> GetSampleRows() => CreateSample().CreateRows();
}

public sealed class S101HAmounts
{
    public decimal? TongSoTien { get; set; }
    public decimal? TuyenTruyen { get; set; }
    public decimal? TrongDoTuyenTruyen { get; set; }
    public decimal? CaiCachHanhChinh { get; set; }
    public decimal? HoTroCapXa { get; set; }
    public decimal? TaiSan { get; set; }
    public decimal? DauTuPhatTrien { get; set; }
    public decimal? MuaSamSuaChua { get; set; }
    public decimal? CongNgheThongTin { get; set; }
    public decimal? HoatDongBoMay { get; set; }
    public decimal? TrongDoHoatDongBoMay { get; set; }
}

public sealed class S101HChiTieu
{
    public string MaChiTieu { get; set; } = "";
    public DateTime? NgayGhiSo { get; set; }
    public string SoChungTu { get; set; } = "";
    public DateTime? NgayChungTu { get; set; }
    public S101HAmounts? SoTien { get; set; }
}

public sealed class S101HQuyetDinh
{
    public string SoQuyetDinh { get; set; } = "";
    public DateTime? NgayGhiSo { get; set; }
    public string SoChungTu { get; set; } = "";
    public DateTime? NgayChungTu { get; set; }
    public S101HAmounts? SoTien { get; set; }
}

public sealed record S101HRow(string MaChiTieu, string TenChiTieu, bool InDam = false)
{
    public DateTime? NgayGhiSo { get; init; }
    public string SoChungTu { get; init; } = "";
    public DateTime? NgayChungTu { get; init; }
    public S101HAmounts? SoTien { get; init; }
}
