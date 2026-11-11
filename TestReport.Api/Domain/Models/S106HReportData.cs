namespace TestReport.Api.Domain.Models;

/// <summary>Giá trị kế toán do bên gọi cung cấp; mẫu không tính tổng hoặc suy diễn số dư.</summary>
public class S106HReportData
{
    public string DonViCapTren { get; set; } = "BẢO HIỂM XÃ HỘI VIỆT NAM";
    public string DonViBaoCao { get; set; } = "BẢO HIỂM XÃ HỘI …";
    public string KyBaoCao { get; set; } = "Tháng/Quý/Năm …";
    public DateTime? NgayMoSo { get; set; }
    public DateTime? NgayKy { get; set; }
    public string KeToan { get; set; } = "";
    public string KeToanTruong { get; set; } = "";
    public string ThuTruongDonVi { get; set; } = "";
    public S106HReceivedSection NhanVaNopTra { get; set; } = new();
    public List<S106HSubItem> TieuMuc { get; set; } = new();
    public S106HAmounts? TongDaSuDungTrongNam { get; set; }
    public S106HAmounts? ChuaSuDung { get; set; }
    public string GhiChuChuaSuDung { get; set; } = "";

    // Projection chỉ thêm dòng trình bày; giữ nguyên thứ tự và giá trị đầu vào.
    public List<S106HReceivedRow> PartIRows
    {
        get
        {
            var section = NhanVaNopTra;
            var rows = new List<S106HReceivedRow>
            {
                ReceivedTotal("Kinh phí chưa sử dụng kỳ trước chuyển sang", section.DauKy),
                ReceivedTotal("Điều chỉnh kinh phí năm trước", section.DieuChinh),
                new() { DienGiai = "Số phát sinh trong kỳ", Dam = true }
            };
            foreach (var month in section.Thang)
            {
                rows.AddRange(month.Items);
                rows.Add(ReceivedTotal($"Cộng phát sinh tháng {month.Thang}/{month.Nam}", month.CongThang));
            }
            rows.Add(ReceivedTotal("Cộng lũy kế từ đầu năm", section.LuyKe));
            rows.Add(ReceivedTotal("Số dư chuyển kỳ sau", section.CuoiKy));
            return rows;
        }
    }

    public List<S106HUsedRow> PartIIRows
    {
        get
        {
            var rows = new List<S106HUsedRow>();
            for (var i = 0; i < TieuMuc.Count; i++)
            {
                var item = TieuMuc[i];
                var label = $"Mã tiểu mục chi: {item.MaTieuMuc}";
                rows.Add(new() { NhomTieuMuc = i, TenTieuMuc = label, DienGiai = "Số phát sinh", Dam = true });
                foreach (var month in item.Thang)
                {
                    rows.AddRange(month.Items.Select(x => x with { NhomTieuMuc = i, TenTieuMuc = label }));
                    rows.Add(new() { NhomTieuMuc = i, TenTieuMuc = label,
                        DienGiai = $"Cộng phát sinh tháng {month.Thang}/{month.Nam}", Tien = month.CongThang, Dam = true });
                }
                rows.Add(new() { NhomTieuMuc = i, TenTieuMuc = label,
                    DienGiai = "Cộng lũy kế từ đầu năm", Tien = item.LuyKe, Dam = true });
            }
            rows.Add(new() { NhomTieuMuc = int.MaxValue, DienGiai = "Tổng cộng số đã sử dụng trong năm", Tien = TongDaSuDungTrongNam, Dam = true });
            return rows;
        }
    }

    public List<S106HUnusedRow> UnusedRows => new() { new() { Tien = ChuaSuDung, GhiChu = GhiChuChuaSuDung } };

    private static S106HReceivedRow ReceivedTotal(string label, S106HReceivedTotals? value) =>
        new() { DienGiai = label, Nhan = value?.Nhan, NopTra = value?.NopTra, Dam = true };

    public static S106HReportData CreateSample()
    {
        static S106HAmounts Money(decimal total) => new() { TongSo = total, ToChucHoatDong = total * .6m,
            CongNgheThongTin = total * .2m, XayDungCoBan = total * .1m, Khac = total * .1m };
        static S106HReceivedTotals Received(decimal amount) => new() { Nhan = Money(amount), NopTra = Money(0) };
        var data = new S106HReportData
        {
            DonViBaoCao = "BẢO HIỂM XÃ HỘI TỈNH MINH HỌA", KyBaoCao = "Quý I năm 2026",
            NgayMoSo = new(2026, 1, 1), NgayKy = new(2026, 3, 31),
            NhanVaNopTra = new() { DauKy = Received(50_000_000), DieuChinh = Received(-1_000_000),
                LuyKe = Received(90_000_000), CuoiKy = Received(97_000_000) },
            TongDaSuDungTrongNam = Money(42_000_000), ChuaSuDung = Money(97_000_000),
            GhiChuChuaSuDung = "Dữ liệu minh họa bố cục"
        };
        for (var month = 1; month <= 3; month++)
            data.NhanVaNopTra.Thang.Add(new() { Thang = month, Nam = 2026,
                Items = new() { new() { NgayGhiSo = new(2026, month, 5), NgayChungTu = new(2026, month, 4),
                    SoChungTu = $"KP{month:00}", DienGiai = "Nhận kinh phí hoạt động nghiệp vụ theo thông báo cấp kinh phí", Nhan = Money(30_000_000), NopTra = Money(0) } },
                CongThang = Received(30_000_000) });
        foreach (var code in new[] { "6001", "6003" })
        {
            var item = new S106HSubItem { MaTieuMuc = code, LuyKe = Money(21_000_000) };
            for (var month = 1; month <= 3; month++)
                item.Thang.Add(new() { Thang = month, Nam = 2026, CongThang = Money(7_000_000),
                    Items = new() { new() { NgayGhiSo = new(2026, month, 12), NgayChungTu = new(2026, month, 11),
                        SoChungTu = $"PC{code}-{month}", DienGiai = "Chi hoạt động nghiệp vụ theo chứng từ được phê duyệt", Tien = Money(7_000_000) } } });
            data.TieuMuc.Add(item);
        }
        return data;
    }
}

public class S106HAmounts
{
    public decimal TongSo { get; set; }
    public decimal ToChucHoatDong { get; set; }
    public decimal CongNgheThongTin { get; set; }
    public decimal XayDungCoBan { get; set; }
    public decimal Khac { get; set; }
}

public class S106HReceivedTotals
{
    public S106HAmounts? Nhan { get; set; }
    public S106HAmounts? NopTra { get; set; }
}

public class S106HReceivedSection
{
    public S106HReceivedTotals? DauKy { get; set; }
    public S106HReceivedTotals? DieuChinh { get; set; }
    public List<S106HReceivedMonth> Thang { get; set; } = new();
    public S106HReceivedTotals? LuyKe { get; set; }
    public S106HReceivedTotals? CuoiKy { get; set; }
}

public class S106HReceivedMonth
{
    public int Thang { get; set; }
    public int Nam { get; set; }
    public List<S106HReceivedRow> Items { get; set; } = new();
    public S106HReceivedTotals? CongThang { get; set; }
}

public class S106HSubItem
{
    public string MaTieuMuc { get; set; } = "";
    public List<S106HUsedMonth> Thang { get; set; } = new();
    public S106HAmounts? LuyKe { get; set; }
}

public class S106HUsedMonth
{
    public int Thang { get; set; }
    public int Nam { get; set; }
    public List<S106HUsedRow> Items { get; set; } = new();
    public S106HAmounts? CongThang { get; set; }
}

public record S106HReceivedRow
{
    public DateTime? NgayGhiSo { get; init; }
    public string SoChungTu { get; init; } = "";
    public DateTime? NgayChungTu { get; init; }
    public string DienGiai { get; init; } = "";
    public S106HAmounts? Nhan { get; init; }
    public S106HAmounts? NopTra { get; init; }
    public bool Dam { get; init; }
}

public record S106HUsedRow
{
    public DateTime? NgayGhiSo { get; init; }
    public string SoChungTu { get; init; } = "";
    public DateTime? NgayChungTu { get; init; }
    public string DienGiai { get; init; } = "";
    public S106HAmounts? Tien { get; init; }
    public bool Dam { get; init; }
    public int NhomTieuMuc { get; init; }
    public string TenTieuMuc { get; init; } = "";
}

public class S106HUnusedRow
{
    public S106HAmounts? Tien { get; set; }
    public string GhiChu { get; set; } = "";
}

/// <summary>Snapshot có các collection ổn định để ObjectDataSource duyệt được toàn bộ dòng.</summary>
public class S106HReportDocument
{
    public S106HReportDocument(S106HReportData data)
    {
        Header = data;
        PartIRows = data.PartIRows;
        PartIIRows = data.PartIIRows;
        UnusedRows = data.UnusedRows;
    }

    public S106HReportData Header { get; }
    public List<S106HReceivedRow> PartIRows { get; }
    public List<S106HUsedRow> PartIIRows { get; }
    public List<S106HUnusedRow> UnusedRows { get; }
    public static List<S106HReportDocument> GetSampleData() => new() { new(S106HReportData.CreateSample()) };
}
