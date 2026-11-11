using System.Drawing;
using System.Text.RegularExpressions;
using DevExpress.Drawing;
using DevExpress.XtraReports.UI;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public static class ReportAppearanceApplier
{
    public static readonly string[] Fonts = ["Arial", "Times New Roman", "Tahoma"];
    public const float MinFontSize = 6;
    public const float MaxFontSize = 24;

    public static void Validate(ReportAppearance? appearance)
    {
        foreach (var style in new[] { appearance?.Title, appearance?.TableHeader, appearance?.TableBody })
        {
            if (style is null) continue;
            if (style.FontFamily is not null && !Fonts.Contains(style.FontFamily))
                throw new ArgumentException("Font không được hỗ trợ.");
            if (style.FontSize is float size && (!float.IsFinite(size) || size < MinFontSize || size > MaxFontSize))
                throw new ArgumentException("Cỡ chữ phải từ 6 đến 24 pt.");
            if (style.Color is not null && !Regex.IsMatch(style.Color, @"\A#[0-9a-fA-F]{6}\z"))
                throw new ArgumentException("Màu phải có dạng #RRGGBB.");
        }
    }

    public static void Apply(XtraReport report, ReportAppearance? appearance)
    {
        Validate(appearance);
        if (appearance is null) return;
        var styles = new Dictionary<string, ReportElementStyle?>(StringComparer.OrdinalIgnoreCase)
        {
            ["ReportTitle"] = appearance.Title,
            ["TableHeader"] = appearance.TableHeader,
            ["TableBody"] = appearance.TableBody
        };
        VisitBands(report, styles);
    }

    private static void VisitBands(XtraReportBase report, Dictionary<string, ReportElementStyle?> styles)
    {
        foreach (Band band in report.Bands)
        {
            if (band is DetailReportBand nested) VisitBands(nested, styles);
            foreach (XRControl control in band.Controls) Visit(control, styles, null);
        }
    }

    private static void Visit(XRControl control, Dictionary<string, ReportElementStyle?> styles, ReportElementStyle? inherited)
    {
        var style = inherited;
        if (styles.TryGetValue(control.Tag?.ToString()?.Trim() ?? "", out var own) && own is not null)
        {
            style = new ReportElementStyle
            {
                FontFamily = own.FontFamily ?? inherited?.FontFamily,
                FontSize = own.FontSize ?? inherited?.FontSize,
                Bold = own.Bold ?? inherited?.Bold,
                Italic = own.Italic ?? inherited?.Italic,
                Color = own.Color ?? inherited?.Color
            };
        }

        if (control is XRTable table)
        {
            foreach (XRTableRow row in table.Rows) Visit(row, styles, style);
        }
        else if (control is XRTableRow row)
        {
            foreach (XRTableCell cell in row.Cells) Visit(cell, styles, style);
        }
        else
        {
            if (style is not null && control is XRLabel or XRTableCell) ApplyControl(control, style);
            foreach (XRControl child in control.Controls) Visit(child, styles, style);
        }
    }

    private static void ApplyControl(XRControl control, ReportElementStyle style)
    {
        if (style.FontFamily is not null || style.FontSize.HasValue || style.Bold.HasValue || style.Italic.HasValue)
        {
            var font = control.GetEffectiveFont();
            var flags = font.Style;
            if (style.Bold is bool bold) flags = bold ? flags | DXFontStyle.Bold : flags & ~DXFontStyle.Bold;
            if (style.Italic is bool italic) flags = italic ? flags | DXFontStyle.Italic : flags & ~DXFontStyle.Italic;
            control.Font = new DXFont(style.FontFamily ?? font.Name, style.FontSize ?? font.SizeInPoints, flags, DXGraphicsUnit.Point);
            control.StylePriority.UseFont = false;
        }
        if (style.Color is not null)
        {
            control.ForeColor = ColorTranslator.FromHtml(style.Color);
            control.StylePriority.UseForeColor = false;
        }
        // ExpressionBindings remain intact and are evaluated during document creation.
    }
}
