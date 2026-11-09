namespace TestReport.Api.Domain.Models;

public sealed class PrintFontSettings
{
    public string Family { get; set; } = "Arial";
    public float Size { get; set; } = 9;
    public bool Bold { get; set; }
    public bool Italic { get; set; }
}

public sealed class MultiColumnPrintSettings
{
    public string PaperOrientation { get; set; } = "landscape";
    public bool RepeatTitleOnEveryPage { get; set; }
    public bool ShowSigner { get; set; }
    public string SignerTitle { get; set; } = "Người lập báo cáo";
    public string SignerName { get; set; } = "";
    public PrintFontSettings TitleFont { get; set; } = new() { Size = 14, Bold = true };
    public PrintFontSettings SubtitleFont { get; set; } = new() { Size = 9.5f, Italic = true };
    public PrintFontSettings HeaderFont { get; set; } = new() { Size = 9, Bold = true };
    public PrintFontSettings DetailFont { get; set; } = new() { Size = 9 };

    public static MultiColumnPrintSettings Default() => new();

    public static string? Validate(MultiColumnPrintSettings? value)
    {
        if (value is null) return "Thiếu cấu hình in.";
        if (value.PaperOrientation is not ("portrait" or "landscape"))
            return "Khổ giấy phải là A4 dọc hoặc A4 ngang.";
        if (value.SignerTitle is null || value.SignerTitle.Length > 100 ||
            value.SignerName is null || value.SignerName.Length > 100)
            return "Thông tin người ký không được vượt quá 100 ký tự.";
        foreach (var font in new[] { value.TitleFont, value.SubtitleFont, value.HeaderFont, value.DetailFont })
        {
            if (font is null || font.Family is not ("Arial" or "Times New Roman" or "Tahoma") ||
                !float.IsFinite(font.Size) || font.Size < 6 || font.Size > 20)
                return "Font phải thuộc danh sách hỗ trợ và có cỡ từ 6 đến 20.";
        }
        return null;
    }
}
