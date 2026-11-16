namespace TestReport.Api.Domain.Models;

public sealed class S12HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string CanCu { get; set; } = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
    public int Thang { get; set; } = 11;
    public int Nam { get; set; } = 2026;
    public string NoiMoTaiKhoan { get; set; } = "";
    public string SoTaiKhoan { get; set; } = "";
    public string LoaiTienGui { get; set; } = "VNĐ";
    public decimal SoDuDauThang { get; set; }
    // Phát sinh từ đầu năm đến trước tháng báo cáo; không bao gồm số dư.
    public decimal LuyKeGuiVaoTruocThang { get; set; }
    public decimal LuyKeRutRaTruocThang { get; set; }
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    // Không khai báo chức danh thì không in nhãn ký, hướng dẫn ký và tên người ký.
    public string ChucDanhKeToan { get; set; } = "";
    public string ChucDanhKeToanTruong { get; set; } = "";
    public string ChucDanhThuTruong { get; set; } = "";
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Phát sinh đã lọc theo tài khoản ngân hàng/kho bạc và loại tiền gửi.
    public List<S12HRow> Items { get; set; } = [];

    public List<S12HRow> CreateRows()
    {
        if (Nam is < 1 or > 9999 || Thang is < 1 or > 12)
            throw new ArgumentException("Tháng, năm báo cáo không hợp lệ.");
        if (Items is null || Items.Any(x => x is null || x.NgayGhiSo is null ||
            x.NgayGhiSo.Value.Year != Nam || x.NgayGhiSo.Value.Month != Thang))
            throw new ArgumentException("Ngày ghi sổ của phát sinh phải thuộc tháng báo cáo.");
        var balance = SoDuDauThang;
        var rows = new List<S12HRow>
        {
            new() { DienGiai = "Số dư đầu tháng", ConLai = balance, InDam = true },
            new() { DienGiai = "Số phát sinh", InDam = true }
        };
        // Cùng ngày giữ thứ tự đầu vào, không dùng ngày chứng từ để tính số dư.
        foreach (var day in Items.OrderBy(x => x.NgayGhiSo!.Value.Date).GroupBy(x => x.NgayGhiSo!.Value.Date))
        {
            foreach (var item in day)
            {
                balance += item.GuiVao - item.RutRa;
                rows.Add(item with { ConLai = balance, InDam = false });
            }
            rows.Add(new() { NgayGhiSo = day.Key, DienGiai = "Cộng cuối ngày", InDam = true,
                GuiVao = day.Sum(x => x.GuiVao), RutRa = day.Sum(x => x.RutRa), ConLai = balance });
        }
        var gui = Items.Sum(x => x.GuiVao);
        var rut = Items.Sum(x => x.RutRa);
        rows.Add(new() { DienGiai = "Cộng phát sinh tháng", GuiVao = gui, RutRa = rut, InDam = true });
        rows.Add(new() { DienGiai = "Cộng lũy kế từ đầu năm", InDam = true,
            GuiVao = LuyKeGuiVaoTruocThang + gui, RutRa = LuyKeRutRaTruocThang + rut });
        rows.Add(new() { DienGiai = "Số dư cuối kỳ", ConLai = balance, InDam = true });
        return rows;
    }

    public static List<S12HRow> GetSampleRows() => CreateSample().CreateRows();

    public static S12HReportData CreateSample() => new()
    {
        DonViBaoCao = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
        NoiMoTaiKhoan = "Kho bạc Nhà nước Hà Nội", SoTaiKhoan = "3713.0.1234567", LoaiTienGui = "VNĐ",
        SoDuDauThang = 50_000_000m, LuyKeGuiVaoTruocThang = 300_000_000m, LuyKeRutRaTruocThang = 250_000_000m,
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 11, 30),
        ChucDanhKeToan = "KẾ TOÁN", ChucDanhKeToanTruong = "KẾ TOÁN TRƯỞNG", ChucDanhThuTruong = "THỦ TRƯỞNG ĐƠN VỊ",
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 11, 2), NgayChungTu = new(2026, 11, 1), SoChungTu = "BC001", DienGiai = "Nhận kinh phí chuyển vào tài khoản", TaiKhoanDoiUng = "135", GuiVao = 100_000_000m },
            new() { NgayGhiSo = new(2026, 11, 2), NgayChungTu = new(2026, 11, 2), SoChungTu = "BN001", DienGiai = "Chuyển khoản thanh toán tiền vật tư", TaiKhoanDoiUng = "331", RutRa = 12_000_000m },
            new() { NgayGhiSo = new(2026, 11, 5), NgayChungTu = new(2026, 11, 4), SoChungTu = "BN002", DienGiai = "Rút tiền gửi nhập quỹ tiền mặt", TaiKhoanDoiUng = "111", RutRa = 20_000_000m },
            new() { NgayGhiSo = new(2026, 11, 10), NgayChungTu = new(2026, 11, 10), SoChungTu = "BC002", DienGiai = "Thu hoàn ứng qua tài khoản", TaiKhoanDoiUng = "141", GuiVao = 5_000_000m }
        ]
    };
}

public sealed record S12HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanDoiUng { get; init; } = "";
    public decimal GuiVao { get; init; }
    public decimal RutRa { get; init; }
    public decimal? ConLai { get; init; }
    public bool InDam { get; init; }
}
