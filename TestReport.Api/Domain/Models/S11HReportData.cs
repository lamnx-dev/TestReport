namespace TestReport.Api.Domain.Models;

public class S11HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI";
    public int Thang { get; set; } = 11;
    public int Nam { get; set; } = 2026;
    public decimal SoDuDauKy { get; set; }
    public List<S11HItemDto> Items { get; set; } = new();

    // Giữ thứ tự ghi sổ do bên gọi cung cấp; không thay đổi dữ liệu đầu vào.
    public List<S11HItemDto> CreateRows()
    {
        decimal balance = SoDuDauKy;
        var rows = new List<S11HItemDto>
        {
            new() { DienGiai = "Số dư đầu kỳ", Ton = balance }
        };
        foreach (var item in Items)
        {
            balance += item.Thu - item.Chi;
            rows.Add(item with { Ton = balance });
        }
        return rows;
    }

    public static S11HReportData CreateSample() => new()
    {
        SoDuDauKy = 50_000_000m,
        Items = new()
        {
            new() { NgayGhiSo = new(2026, 11, 2), NgayChungTu = new(2026, 11, 2), SoPhieuThu = "PT001", DienGiai = "Rút tiền gửi ngân hàng nhập quỹ", TaiKhoanDoiUng = "1121", Thu = 100_000_000m },
            new() { NgayGhiSo = new(2026, 11, 3), NgayChungTu = new(2026, 11, 3), SoPhieuChi = "PC001", DienGiai = "Chi mua văn phòng phẩm", TaiKhoanDoiUng = "611", Chi = 2_500_000m },
            new() { NgayGhiSo = new(2026, 11, 4), NgayChungTu = new(2026, 11, 4), SoPhieuChi = "PC002", DienGiai = "Tạm ứng công tác phí", TaiKhoanDoiUng = "141", Chi = 5_000_000m, GhiChu = "Nguyễn Văn An" },
            new() { NgayGhiSo = new(2026, 11, 5), NgayChungTu = new(2026, 11, 5), SoPhieuThu = "PT002", DienGiai = "Thu hoàn ứng công tác phí", TaiKhoanDoiUng = "141", Thu = 1_200_000m, GhiChu = "Trần Thị Bình" },
            new() { NgayGhiSo = new(2026, 11, 6), NgayChungTu = new(2026, 11, 6), SoPhieuChi = "PC003", DienGiai = "Thanh toán chi phí vận chuyển hồ sơ", TaiKhoanDoiUng = "611", Chi = 1_800_000m },
            new() { NgayGhiSo = new(2026, 11, 9), NgayChungTu = new(2026, 11, 9), SoPhieuChi = "PC004", DienGiai = "Chi sửa chữa thiết bị văn phòng", TaiKhoanDoiUng = "611", Chi = 3_500_000m },
            new() { NgayGhiSo = new(2026, 11, 10), NgayChungTu = new(2026, 11, 10), SoPhieuThu = "PT003", DienGiai = "Thu hồi khoản chi thừa", TaiKhoanDoiUng = "1388", Thu = 800_000m },
            new() { NgayGhiSo = new(2026, 11, 11), NgayChungTu = new(2026, 11, 11), SoPhieuChi = "PC005", DienGiai = "Chi công tác phí theo bảng kê", TaiKhoanDoiUng = "611", Chi = 4_200_000m },
            new() { NgayGhiSo = new(2026, 11, 12), NgayChungTu = new(2026, 11, 12), SoPhieuChi = "PC006", DienGiai = "Thanh toán tiền mua vật tư", TaiKhoanDoiUng = "331", Chi = 6_000_000m },
            new() { NgayGhiSo = new(2026, 11, 13), NgayChungTu = new(2026, 11, 13), SoPhieuThu = "PT004", DienGiai = "Rút tiền gửi ngân hàng nhập quỹ", TaiKhoanDoiUng = "1121", Thu = 20_000_000m }
        }
    };

    public static List<S11HItemDto> GetSampleRows()
    {
        return CreateSample().CreateRows();
    }
}

public record S11HItemDto
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoPhieuThu { get; init; } = "";
    public string SoPhieuChi { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanDoiUng { get; init; } = "";
    public decimal Thu { get; init; }
    public decimal Chi { get; init; }
    public decimal Ton { get; init; }
    public string GhiChu { get; init; } = "";
}
