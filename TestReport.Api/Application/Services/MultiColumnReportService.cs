using TestReport.Api.Application.Interfaces;
using TestReport.Api.Domain.Models;
using TestReport.Api.Domain.Requests;
using TestReport.Api.Domain.Responses;

namespace TestReport.Api.Application.Services
{
    public class MultiColumnReportService : IMultiColumnReportService
    {
        private const string DefaultPdfFileName = "BaoCao_NhieuCot_TachTrang.pdf";
        private readonly IMultiColumnReportRepository _reportRepository;
        private readonly IReportRenderer _reportRenderer;
        private readonly IMultiColumnPrintSettingsStore _printSettings;

        public MultiColumnReportService(
            IMultiColumnReportRepository reportRepository,
            IReportRenderer reportRenderer,
            IMultiColumnPrintSettingsStore printSettings)
        {
            _reportRepository = reportRepository;
            _reportRenderer = reportRenderer;
            _printSettings = printSettings;
        }

        public MultiColumnReportData GetReportData(MultiColumnReportRequest request)
        {
            return _reportRepository.GetReportData(request);
        }

        public ReportFileResponse ExportPdf(MultiColumnReportRequest request)
        {
            var data = _reportRepository.GetReportData(request);
            var stream = _reportRenderer.RenderToPdfStream(data, _printSettings.Get());
            return new ReportFileResponse(stream, "application/pdf", DefaultPdfFileName);
        }

        public ReportBase64Response GetBase64(MultiColumnReportRequest request)
        {
            var data = _reportRepository.GetReportData(request);
            return _reportRenderer.RenderToBase64(data, _printSettings.Get(), DefaultPdfFileName);
        }
    }
}
