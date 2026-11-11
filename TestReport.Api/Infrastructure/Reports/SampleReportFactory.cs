using DevExpress.XtraReports.UI;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public sealed class SampleReportFactory
{
    public XtraReport? Create(string id) => id switch
    {
        "s11h" => new S11_H_Report(S11HReportData.CreateSample()),
        "s106h" => new S106HReport(S106HReportData.CreateSample()),
        _ => null
    };
}
