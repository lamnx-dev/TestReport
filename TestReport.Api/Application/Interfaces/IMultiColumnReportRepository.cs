using System.Threading.Tasks;
using TestReport.Api.Domain.Models;
using TestReport.Api.Domain.Requests;

namespace TestReport.Api.Application.Interfaces
{
    public interface IMultiColumnReportRepository
    {
        MultiColumnReportData GetReportData(MultiColumnReportRequest request);
        Task<MultiColumnReportData> GetReportDataAsync(MultiColumnReportRequest request);
    }
}

