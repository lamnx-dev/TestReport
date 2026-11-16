using DevExpress.XtraReports.UI;

namespace TestReport.Api.Infrastructure.Reports;

/// <summary>Fills labels and table cells whose Tag matches a supplied text key.</summary>
public static class ReportTextApplier
{
    public static void Apply(XtraReport report, IReadOnlyDictionary<string, string> texts)
    {
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(texts);
        Visit(report, texts);
    }

    private static void Visit(XRControl control, IReadOnlyDictionary<string, string> texts)
    {
        if (control is XRLabel label && label.Tag is string tag && texts.TryGetValue(tag, out var text))
            label.Text = text;

        switch (control)
        {
            case XtraReportBase report:
                foreach (Band band in report.Bands) Visit(band, texts);
                break;
            case XRTable table:
                foreach (XRTableRow row in table.Rows) Visit(row, texts);
                break;
            case XRTableRow row:
                foreach (XRTableCell cell in row.Cells) Visit(cell, texts);
                break;
            default:
                foreach (XRControl child in control.Controls) Visit(child, texts);
                break;
        }
    }
}
