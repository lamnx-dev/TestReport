using Microsoft.AspNetCore.Mvc;
using TestReport.Api.Application.Interfaces;
using TestReport.Api.Domain.Requests;

namespace TestReport.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IMultiColumnReportService _reportService;

        public ReportController(IMultiColumnReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Xem trước PDF báo cáo nhiều cột trực tiếp trên trình duyệt
        /// </summary>
        [HttpGet("multi-column-preview")]
        public IActionResult PreviewMultiColumnPdf([FromQuery] MultiColumnReportRequest request)
        {
            var result = _reportService.ExportPdf(request);
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{result.FileName}\"");
            return File(result.Stream, result.ContentType);
        }

        /// <summary>
        /// Tải xuống file PDF báo cáo nhiều cột
        /// </summary>
        [HttpGet("multi-column-export-pdf")]
        public IActionResult ExportMultiColumnPdf([FromQuery] MultiColumnReportRequest request)
        {
            var result = _reportService.ExportPdf(request);
            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{result.FileName}\"");
            return File(result.Stream, result.ContentType, result.FileName);
        }

        /// <summary>
        /// Trả về chuỗi Base64 của PDF báo cáo nhiều cột cho Frontend Viewer
        /// </summary>
        [HttpGet("multi-column-base64")]
        public IActionResult GetMultiColumnBase64([FromQuery] MultiColumnReportRequest request)
        {
            var result = _reportService.GetBase64(request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy dữ liệu dạng JSON thô của báo cáo nhiều cột
        /// </summary>
        [HttpGet("multi-column-data")]
        public IActionResult GetMultiColumnData([FromQuery] MultiColumnReportRequest request)
        {
            var data = _reportService.GetReportData(request);
            return Ok(data);
        }
    }
}

