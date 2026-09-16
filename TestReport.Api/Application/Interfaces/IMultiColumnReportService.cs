using TestReport.Api.Domain.Models;
using TestReport.Api.Domain.Requests;
using TestReport.Api.Domain.Responses;

namespace TestReport.Api.Application.Interfaces
{
    public interface IMultiColumnReportService
    {
        MultiColumnReportData GetReportData(MultiColumnReportRequest request);
        ReportFileResponse ExportPdf(MultiColumnReportRequest request);
        ReportBase64Response GetBase64(MultiColumnReportRequest request);
    }
}

