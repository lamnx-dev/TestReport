using DevExpress.XtraReports.UI;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public sealed class SampleReportFactory
{
    public XtraReport? Create(string id) => id switch
    {
        "s11h" => new S11_H_Report(S11HReportData.CreateSample()),
        "s106h" => new S106HReport(S106HReportData.CreateSample()),
        "s90h" => new S90HReport(S90HReportData.CreateSample()),
        "s62h" => new S62HReport(S62HReportData.CreateSample()),
        _ => null
    };
}
