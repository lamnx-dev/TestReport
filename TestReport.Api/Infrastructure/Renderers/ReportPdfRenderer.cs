using DevExpress.XtraReports.UI;

namespace TestReport.Api.Infrastructure.Renderers;

public sealed class ReportPdfRenderer
{
    public byte[] Render(XtraReport report)
    {
        using var stream = new MemoryStream();
        report.ExportToPdf(stream);
        return stream.ToArray();
    }
}
