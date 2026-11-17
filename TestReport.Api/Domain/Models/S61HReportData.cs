namespace TestReport.Api.Domain.Models;

public sealed class S61HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI MINH HỌA";
    public string CanCu { get; set; } = "Ban hành kèm theo Công văn số 153/BHXH-TCKT ngày 14/01/2026 của Bảo hiểm xã hội Việt Nam";
    public string TaiKhoan { get; set; } = "611";
    public string TenTaiKhoan { get; set; } = "Chi phí hoạt động";
    public string MaDonVi { get; set; } = "VP";
    // Danh sách đơn vị thuộc tỉnh, gồm cả văn phòng tỉnh.
    public List<string> MaDonViToanTinh { get; set; } = ["VP", "HUYEN"];
    public bool ToanTinh { get; set; }
    public DateTime TuNgay { get; set; } = new(2026, 1, 1);
    public DateTime DenNgay { get; set; } = new(2026, 3, 31);
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    // Từng dòng định khoản từ đầu năm đến hết kỳ, không gộp theo chứng từ.
    public List<S61HPosting> Items { get; set; } = [];

    public List<S61HRow> CreateRows()
    {
        if (TaiKhoan is not ("611" or "612" or "642"))
            throw new ArgumentException("Chọn tài khoản 611, 612 hoặc 642.");
        if (TuNgay.Date > DenNgay.Date || TuNgay.Year != DenNgay.Year)
            throw new ArgumentException("Kỳ báo cáo phải nằm trong cùng năm và ngày bắt đầu không sau ngày kết thúc.");
        if (Items is null || string.IsNullOrWhiteSpace(MaDonVi) ||
            (ToanTinh && (MaDonViToanTinh is null || MaDonViToanTinh.Count == 0)))
            throw new ArgumentException("Thiếu dữ liệu định khoản hoặc phạm vi đơn vị.");
        if (Items.Any(x => x is null || string.IsNullOrWhiteSpace(x.TaiKhoanNo) ||
            string.IsNullOrWhiteSpace(x.TaiKhoanCo) || string.IsNullOrWhiteSpace(x.MaDonVi)))
            throw new ArgumentException("Định khoản phải có tài khoản Nợ, Có và mã đơn vị.");

        var entries = Items.Where(x => x.NgayGhiSo.Year == TuNgay.Year && x.NgayGhiSo.Date <= DenNgay.Date &&
            (ToanTinh ? MaDonViToanTinh.Contains(x.MaDonVi) : x.MaDonVi == MaDonVi))
            .OrderBy(x => x.NgayGhiSo).ToList();
        var normal = new List<S61HRow>();
        var transfers = new List<S61HRow>();
        var prior = new List<S61HRow>();
        foreach (var entry in entries)
        {
            var inPeriod = entry.NgayGhiSo.Date >= TuNgay.Date;
            // Hai vế được giữ riêng khi cả hai cùng thuộc tài khoản được chọn.
            foreach (var debit in new[] { true, false })
            {
                var account = debit ? entry.TaiKhoanNo : entry.TaiKhoanCo;
                var counterpart = debit ? entry.TaiKhoanCo : entry.TaiKhoanNo;
                if (!account.StartsWith(TaiKhoan, StringComparison.Ordinal)) continue;
                var group = account.Length > 3 ? account[3] : '\0';
                if (group is not ('1' or '2' or '3' or '8'))
                    throw new ArgumentException($"Tài khoản {account} chưa có nhóm chi phí 1, 2, 3 hoặc 8.");
                if (debit && !ToanTinh && (counterpart.StartsWith("136") || counterpart.StartsWith("515"))) continue;
                var transfer = !debit && counterpart.StartsWith("911");
                var amount = entry.SoTien;
                var row = new S61HRow
                {
                    NgayGhiSo = entry.NgayGhiSo, NgayChungTu = entry.NgayChungTu,
                    SoChungTu = entry.SoChungTu, DienGiai = entry.DienGiai, TaiKhoanDoiUng = counterpart,
                    NoLuong = debit && group == '1' ? amount : 0,
                    NoVatTu = debit && group == '2' ? amount : 0,
                    NoKhauHao = debit && group == '3' ? amount : 0,
                    NoKhac = debit && group == '8' ? amount : 0,
                    CoLuong = !debit && group == '1' ? amount : 0,
                    CoVatTu = !debit && group == '2' ? amount : 0,
                    CoKhauHao = !debit && group == '3' ? amount : 0,
                    CoKhac = !debit && group == '8' ? amount : 0
                };
                if (transfer) { if (inPeriod) transfers.Add(row); }
                else if (inPeriod) normal.Add(row);
                else prior.Add(row);
            }
        }
        return [.. normal, Total("Cộng số phát sinh", normal),
            Total("Cộng lũy kế từ đầu năm", prior.Concat(normal)),
            new() { DienGiai = "Kết chuyển để xác định kết quả hoạt động", InDam = true }, .. transfers];
    }

    private static S61HRow Total(string label, IEnumerable<S61HRow> source)
    {
        var rows = source.ToList();
        return new() { DienGiai = label, InDam = true,
            NoLuong = rows.Sum(x => x.NoLuong), NoVatTu = rows.Sum(x => x.NoVatTu),
            NoKhauHao = rows.Sum(x => x.NoKhauHao), NoKhac = rows.Sum(x => x.NoKhac),
            CoLuong = rows.Sum(x => x.CoLuong), CoVatTu = rows.Sum(x => x.CoVatTu),
            CoKhauHao = rows.Sum(x => x.CoKhauHao), CoKhac = rows.Sum(x => x.CoKhac) };
    }

    public static List<S61HRow> GetSampleRows() => CreateSample().CreateRows();
    public static S61HReportData CreateSample() => new()
    {
        NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 3, 31),
        KeToan = "Nguyễn Văn An", KeToanTruong = "Trần Thị Bình", ThuTruongDonVi = "Lê Văn Minh",
        Items =
        [
            new() { NgayGhiSo = new(2026, 1, 31), NgayChungTu = new(2026, 1, 31), SoChungTu = "TL001", DienGiai = "Chi tiền lương nhân viên", TaiKhoanNo = "6111", TaiKhoanCo = "334", SoTien = 50_000_000m },
            new() { NgayGhiSo = new(2026, 2, 10), NgayChungTu = new(2026, 2, 10), SoChungTu = "VT002", DienGiai = "Chi vật tư, công cụ và dịch vụ", TaiKhoanNo = "6112", TaiKhoanCo = "331", SoTien = 12_000_000m },
            new() { NgayGhiSo = new(2026, 2, 28), NgayChungTu = new(2026, 2, 28), SoChungTu = "KH002", DienGiai = "Chi hao mòn tài sản cố định", TaiKhoanNo = "6113", TaiKhoanCo = "214", SoTien = 8_000_000m },
            new() { NgayGhiSo = new(2026, 3, 15), NgayChungTu = new(2026, 3, 15), SoChungTu = "CP003", DienGiai = "Chi phí khác", TaiKhoanNo = "6118", TaiKhoanCo = "112", SoTien = 3_000_000m },
            new() { NgayGhiSo = new(2026, 3, 20), NgayChungTu = new(2026, 3, 20), SoChungTu = "GC003", DienGiai = "Ghi giảm chi phí vật tư", TaiKhoanNo = "152", TaiKhoanCo = "6112", SoTien = 1_000_000m },
            new() { NgayGhiSo = new(2026, 3, 31), NgayChungTu = new(2026, 3, 31), SoChungTu = "KC003", DienGiai = "Kết chuyển chi phí tiền lương", TaiKhoanNo = "911", TaiKhoanCo = "6111", SoTien = 50_000_000m }
        ]
    };
}

public sealed record S61HPosting
{
    public string MaDonVi { get; init; } = "VP";
    public DateTime NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanNo { get; init; } = "";
    public string TaiKhoanCo { get; init; } = "";
    public decimal SoTien { get; init; }
}

public sealed record S61HRow
{
    public DateTime? NgayGhiSo { get; init; }
    public DateTime? NgayChungTu { get; init; }
    public string SoChungTu { get; init; } = "";
    public string DienGiai { get; init; } = "";
    public string TaiKhoanDoiUng { get; init; } = "";
    public decimal NoLuong { get; init; }
    public decimal NoVatTu { get; init; }
    public decimal NoKhauHao { get; init; }
    public decimal NoKhac { get; init; }
    public decimal CoLuong { get; init; }
    public decimal CoVatTu { get; init; }
    public decimal CoKhauHao { get; init; }
    public decimal CoKhac { get; init; }
    public decimal TongNo => NoLuong + NoVatTu + NoKhauHao + NoKhac;
    public decimal TongCo => CoLuong + CoVatTu + CoKhauHao + CoKhac;
    public bool InDam { get; init; }
}
