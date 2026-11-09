using System.IO;
using TestReport.Api.Domain.Models;
using TestReport.Api.Domain.Responses;

namespace TestReport.Api.Application.Interfaces
{
    public interface IReportRenderer
    {
        Stream RenderToPdfStream(MultiColumnReportData reportData, MultiColumnPrintSettings settings);
        ReportBase64Response RenderToBase64(MultiColumnReportData reportData, MultiColumnPrintSettings settings, string fileName = "BaoCao_NhieuCot_TachTrang.pdf");
    }
}
