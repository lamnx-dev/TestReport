namespace TestReport.Api.Domain.Models;

public sealed class S11bHReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public int Thang { get; set; } = 11;
    public int Nam { get; set; } = 2026;
    public decimal SoDuDauNam { get; set; }
    // Phát sinh từ đầu năm đến trước tháng báo cáo, không bao gồm số dư.
    public decimal LuyKeThuTruocThang { get; set; }
    public decimal LuyKeChiTruocThang { get; set; }
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    // Chưa khai báo chức danh thì không in nhãn ký, hướng dẫn và tên người ký.
    public string ChucDanhThuQuy { get; set; } = "";
    public string ChucDanhKeToanTruong { get; set; } = "";
    public string ChucDanhThuTruongDonVi { get; set; } = "";
    public string ThuQuy { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Phát sinh đã lọc theo quỹ tiền mặt, đơn vị và loại tiền.
    public List<S11bHRow> Items { get; set; } = [];

    public List<S11bHRow> CreateRows()
    {
        if (Nam is < 1 or > 9999 || Thang is < 1 or > 12)
            throw new ArgumentException("Tháng, năm báo cáo không hợp lệ.");
        if (Items is null || Items.Any(x => x is null || x.NgayGhiSo is null ||
            x.NgayGhiSo.Value.Year != Nam || x.NgayGhiSo.Value.Month != Thang))
            throw new ArgumentException("Ngày ghi sổ của phát sinh phải thuộc tháng báo cáo.");
        if (Thang == 1 && (LuyKeThuTruocThang != 0 || LuyKeChiTruocThang != 0))
            throw new ArgumentException("Tháng 1 không có phát sinh lũy kế trước tháng.");

        // Mẫu in số dư đầu năm; tồn trước giao dịch đầu tháng gồm cả phát sinh các tháng trước.
        var balance = SoDuDauNam + LuyKeThuTruocThang - LuyKeChiTruocThang;
        var rows = new List<S11bHRow>
        {
            new() { DienGiai = "Số dư đầu năm", Ton = SoDuDauNam, InDam = true },
            new() { DienGiai = "Phát sinh trong tháng", InDam = true }
        };
        foreach (var day in Items.OrderBy(x => x.NgayGhiSo!.Value.Date).GroupBy(x => x.NgayGhiSo!.Value.Date))
        {
            foreach (var item in day)
            {
                balance += item.Thu - item.Chi;
                rows.Add(item with { Ton = balance, InDam = false });
            }
            rows.Add(new() { NgayGhiSo = day.Key, DienGiai = "Cộng số phát sinh ngày", InDam = true,
                Thu = day.Sum(x => x.Thu), Chi = day.Sum(x => x.Chi), Ton = balance });
        }
        var thu = Items.Sum(x => x.Thu);
        var chi = Items.Sum(x => x.Chi);
        rows.Add(new() { DienGiai = "Cộng số lũy kế tháng", Thu = thu, Chi = chi, Ton = balance, InDam = true });
        rows.Add(new() { DienGiai = "Cộng lũy kế từ đầu năm", InDam = true,
            Thu = LuyKeThuTruocThang + thu, Chi = LuyKeChiTruocThang + chi, Ton = balance });
        return rows;
    }

    public static List<S11bHRow> GetSampleRows() => CreateSample().CreateRows();

    public static S11bHReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
        SoDuDauNam = 20_000_000m, LuyKeThuTruocThang = 300_000_000m, LuyKeChiTruocThang = 270_000_000m,
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 11, 30),
        ChucDanhThuQuy = "THỦ QUỸ", ChucDanhKeToanTruong = "KẾ TOÁN TRƯỞNG", ChucDanhThuTruongDonVi = "THỦ TRƯỞNG ĐƠN VỊ",
        ThuQuy = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 11, 2), NgayChungTu = new(2026, 11, 1), SoPhieuThu = "PT001", DienGiai = "Rút tiền gửi ngân hàng nhập quỹ", Thu = 100_000_000m },
            new() { NgayGhiSo = new(2026, 11, 2), NgayChungTu = new(2026, 11, 2), SoPhieuChi = "PC001", DienGiai = "Chi mua văn phòng phẩm", Chi = 2_500_000m },
            new() { NgayGhiSo = new(2026, 11, 5), NgayChungTu = new(2026, 11, 4), SoPhieuChi = "PC002", DienGiai = "Tạm ứng công tác phí", Chi = 5_000_000m, GhiChu = "Nguyễn Thị Lan" },
            new() { NgayGhiSo = new(2026, 11, 10), NgayChungTu = new(2026, 11, 10), SoPhieuThu = "PT002", DienGiai = "Thu hoàn ứng công tác phí", Thu = 1_200_000m }
        ]
    };
}

public sealed record S11bHRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoPhieuThu { get; init; } = "";
    public string SoPhieuChi { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public decimal Thu { get; init; }
    public decimal Chi { get; init; }
    public decimal? Ton { get; init; }
    public string GhiChu { get; init; } = "";
    public bool InDam { get; init; }
}
