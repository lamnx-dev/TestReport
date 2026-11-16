namespace TestReport.Api.Domain.Models;

public sealed class S62HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "ĐƠN VỊ SỰ NGHIỆP";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public string MaSanPham { get; set; } = "";
    public string TenSanPham { get; set; } = "";
    public DateTime TuNgay { get; set; } = new(2026, 1, 1);
    public DateTime DenNgay { get; set; } = new(2026, 3, 31);
    public decimal SoDuDauKy { get; set; }
    public decimal DieuChinhDauNam { get; set; }
    // Phát sinh từ đầu năm đến trước TuNgay, không bao gồm số dư.
    public S62HRow LuyKeTruocKy { get; set; } = new();
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Các phát sinh đã phân loại theo sản phẩm, dịch vụ và nhóm chi phí TK 154.
    public List<S62HRow> Items { get; set; } = [];

    public List<S62HRow> CreateRows()
    {
        if (TuNgay.Date > DenNgay.Date || TuNgay.Year != DenNgay.Year)
            throw new ArgumentException("Kỳ báo cáo phải nằm trong cùng năm và ngày bắt đầu không sau ngày kết thúc.");
        if (Items is null || LuyKeTruocKy is null)
            throw new ArgumentException("Dữ liệu phát sinh và lũy kế không được null.");
        if (Items.Any(x => x is null || x.NgayGhiSo is null ||
            x.NgayGhiSo.Value.Date < TuNgay.Date || x.NgayGhiSo.Value.Date > DenNgay.Date))
            throw new ArgumentException("Ngày ghi sổ của phát sinh phải thuộc kỳ báo cáo.");

        var rows = new List<S62HRow>
        {
            new() { DienGiai = "Số dư đầu kỳ", SoDu = SoDuDauKy, InDam = true },
            new() { DienGiai = "Điều chỉnh số dư đầu năm", SoDu = DieuChinhDauNam, InDam = true },
            new() { DienGiai = "Số phát sinh", InDam = true }
        };
        // In cả tháng không phát sinh trong kỳ.
        for (var month = new DateTime(TuNgay.Year, TuNgay.Month, 1);
             month <= DenNgay.Date; month = month.AddMonths(1))
        {
            var items = Items.Where(x => x.NgayGhiSo!.Value.Month == month.Month)
                .OrderBy(x => x.NgayGhiSo).ToList();
            rows.AddRange(items.Select(x => x with { InDam = false, SoDu = null }));
            rows.Add(Total($"Cộng phát sinh tháng {month:MM/yyyy}", items));
        }
        var total = Total("Cộng phát sinh trong kỳ", Items);
        if (TuNgay.Day == 1 && (TuNgay.Month - 1) % 3 == 0 &&
            DenNgay.Date == TuNgay.Date.AddMonths(3).AddDays(-1))
            rows.Add(total);
        rows.Add(Total("Luỹ kế từ đầu năm", [LuyKeTruocKy, total]));
        rows.Add(new() { DienGiai = "Số dư cuối kỳ", InDam = true,
            SoDu = SoDuDauKy + DieuChinhDauNam + total.TongNo - total.GhiCo });
        return rows;
    }

    private static S62HRow Total(string label, IEnumerable<S62HRow> source)
    {
        var items = source.ToList();
        return new() { DienGiai = label, InDam = true,
            NguyenVatLieu = items.Sum(x => x.NguyenVatLieu), NhanCong = items.Sum(x => x.NhanCong),
            SanXuatChung = items.Sum(x => x.SanXuatChung), ChiKhac = items.Sum(x => x.ChiKhac),
            GhiCo = items.Sum(x => x.GhiCo) };
    }

    public static List<S62HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S62HReportData CreateSample() => new()
    {
        DonViBaoCao = "ĐƠN VỊ SỰ NGHIỆP MINH HỌA", MaSanPham = "DV01", TenSanPham = "Dịch vụ đào tạo",
        SoDuDauKy = 20_000_000m, DieuChinhDauNam = 1_000_000m,
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 3, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 5), NgayChungTu = new(2026, 1, 5), SoChungTu = "VT001", DienGiai = "Xuất vật liệu phục vụ dịch vụ", TaiKhoanDoiUng = "152", NguyenVatLieu = 12_000_000m },
            new() { NgayGhiSo = new(2026, 1, 31), NgayChungTu = new(2026, 1, 31), SoChungTu = "TL001", DienGiai = "Chi phí nhân công trực tiếp", TaiKhoanDoiUng = "334", NhanCong = 18_000_000m },
            new() { NgayGhiSo = new(2026, 2, 15), NgayChungTu = new(2026, 2, 15), SoChungTu = "CP002", DienGiai = "Chi phí sản xuất chung", TaiKhoanDoiUng = "331", SanXuatChung = 5_000_000m, ChiKhac = 2_000_000m },
            new() { NgayGhiSo = new(2026, 3, 31), NgayChungTu = new(2026, 3, 31), SoChungTu = "KC003", DienGiai = "Kết chuyển giá vốn dịch vụ hoàn thành", TaiKhoanDoiUng = "632", GhiCo = 30_000_000m }
        ]
    };
}

public sealed record S62HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanDoiUng { get; init; } = "";
    public decimal NguyenVatLieu { get; init; }
    public decimal NhanCong { get; init; }
    public decimal SanXuatChung { get; init; }
    public decimal ChiKhac { get; init; }
    public decimal GhiCo { get; init; }
    public decimal TongNo => NguyenVatLieu + NhanCong + SanXuatChung + ChiKhac;
    public decimal? SoDu { get; init; }
    public decimal SoTienNo => SoDu ?? TongNo;
    public bool InDam { get; init; }
}
