namespace TestReport.Api.Domain.Models;

public sealed class ReportRenderRequest
{
    public ReportAppearance? Appearance { get; set; }
}

public sealed class ReportAppearance
{
    public ReportElementStyle? Title { get; set; }
    public ReportElementStyle? TableHeader { get; set; }
    public ReportElementStyle? TableBody { get; set; }
}

public sealed record ReportElementStyle
{
    public string? FontFamily { get; init; }
    public float? FontSize { get; init; }
    public bool? Bold { get; init; }
    public bool? Italic { get; init; }
    public string? Color { get; init; }
}
