using System.IO;
using TestReport.Api.Domain.Models;
using TestReport.Api.Domain.Responses;

namespace TestReport.Api.Application.Interfaces
{
    public interface IReportRenderer
    {
        Stream RenderToPdfStream(MultiColumnReportData reportData);
        ReportBase64Response RenderToBase64(MultiColumnReportData reportData, string fileName = "BaoCao_NhieuCot_TachTrang.pdf");
    }
}

