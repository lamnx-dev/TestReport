using DevExpress.XtraReports.UI;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports;

public sealed class SampleReportFactory
{
    public XtraReport? Create(string id) => id switch
    {
        "s11h" => new S11HReport(S11HReportData.CreateSample()),
        "s11bh" => new S11bHReport(S11bHReportData.CreateSample()),
        "s12h" => new S12HReport(S12HReportData.CreateSample()),
        "s31h" => new S31HReport(S31HReportData.CreateSample()),
        "s32h" => new S32HReport(S32HReportData.CreateSample()),
        "s33h" => new S33HReport(S33HReportData.CreateSample()),
        "s34h" => new S34HReport(S34HReportData.CreateSample()),
        "s36h" => new S36HReport(S36HReportData.CreateSample()),
        "s37h" => new S37HReport(S37HReportData.CreateSample()),
        "s38h" => new S38HReport(S38HReportData.CreateSample()),
        "s51h" => new S51HReport(S51HReportData.CreateSample()),
        "s52h" => new S52HReport(S52HReportData.CreateSample()),
        "s53h" => new S53HReport(S53HReportData.CreateSample()),
        "s54h" => new S54HReport(S54HReportData.CreateSample()),
        "s55h" => new S55HReport(S55HReportData.CreateSample()),
        "s106h" => new S106HReport(S106HReportData.CreateSample()),
        "s101h" => new S101HReport(S101HReportData.CreateSample()),
        "s90h" => new S90HReport(S90HReportData.CreateSample()),
        "s61h" => new S61HReport(S61HReportData.CreateSample()),
        "s62h" => new S62HReport(S62HReportData.CreateSample()),
        _ => null
    };
}
