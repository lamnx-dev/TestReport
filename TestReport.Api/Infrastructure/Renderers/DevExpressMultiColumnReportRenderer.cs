using System;
using System.IO;
using TestReport.Api.Application.Interfaces;
using TestReport.Api.Domain.Models;
using TestReport.Api.Domain.Responses;
using TestReport.Api.Infrastructure.Reports;

namespace TestReport.Api.Infrastructure.Renderers
{
    public class DevExpressMultiColumnReportRenderer : IReportRenderer
    {
        public Stream RenderToPdfStream(MultiColumnReportData reportData, MultiColumnPrintSettings settings)
        {
            using var report = new MultiColumnSplitReport(reportData, settings);
            var stream = new MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;
            return stream;
        }

        public ReportBase64Response RenderToBase64(MultiColumnReportData reportData, MultiColumnPrintSettings settings, string fileName = "BaoCao_NhieuCot_TachTrang.pdf")
        {
            using var report = new MultiColumnSplitReport(reportData, settings);
            report.CreateDocument();
            int totalPages = report.Pages.Count;

            using var stream = new MemoryStream();
            report.ExportToPdf(stream);

            byte[] bytes = stream.ToArray();
            string base64String = Convert.ToBase64String(bytes);

            return new ReportBase64Response
            {
                FileName = fileName,
                ContentType = "application/pdf",
                FileSize = bytes.Length,
                TotalRecords = reportData.Rows.Count,
                TotalColumns = reportData.TotalColumns,
                ColumnsPerPage = reportData.ColumnsPerPage,
                TotalPages = totalPages,
                Base64 = base64String
            };
        }
    }
}
