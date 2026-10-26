using Microsoft.AspNetCore.Mvc;
using TestReport.Api.Application.Interfaces;
using TestReport.Api.Domain.Models;
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

        /// <summary>
        /// Xem trước PDF Mẫu số 07/BCNV (Báo cáo tổng hợp lãi thu hồi chi sai) trực tiếp trên trình duyệt
        /// </summary>
        [HttpGet("07-bcnv-preview")]
        public IActionResult Preview07BcnvPdf([FromQuery] int? quy = null, [FromQuery] int? nam = null)
        {
            var reportData = BaoCao07BcnvReportData.CreateSample();
            if (quy.HasValue) reportData.Quy = quy.Value;
            if (nam.HasValue) reportData.Nam = nam.Value;

            using var report = new Infrastructure.Reports.BaoCao07BcnvReport(reportData);

            var stream = new System.IO.MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            int q = quy ?? 1;
            int n = nam ?? 2026;
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"BaoCao_07_BCNV_Quy{q}_{n}.pdf\"");
            return File(stream, "application/pdf");
        }

        /// <summary>
        /// Xuất file PDF Mẫu số 07/BCNV từ dữ liệu Object truyền vào qua JSON Body
        /// Nếu không truyền dữ liệu (null) -> Tự động nạp dữ liệu mẫu preview
        /// </summary>
        [HttpPost("07-bcnv-export-pdf")]
        public IActionResult Export07BcnvPdfFromBody([FromBody] Domain.Models.BaoCao07BcnvReportData? data)
        {
            using var report = new Infrastructure.Reports.BaoCao07BcnvReport(data);

            var stream = new System.IO.MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            var q = data?.Quy ?? 1;
            var n = data?.Nam ?? 2026;
            string fileName = $"BaoCao_07_BCNV_Quy{q}_{n}.pdf";
            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return File(stream, "application/pdf", fileName);
        }

        /// <summary>
        /// Tải xuống file PDF Mẫu số 07/BCNV (GET)
        /// </summary>
        [HttpGet("07-bcnv-export-pdf")]
        public IActionResult Export07BcnvPdf([FromQuery] int quy = 1, [FromQuery] int nam = 2026)
        {
            var reportData = Domain.Models.BaoCao07BcnvReportData.CreateSample();
            reportData.Quy = quy;
            reportData.Nam = nam;
            using var report = new Infrastructure.Reports.BaoCao07BcnvReport(reportData);

            var stream = new System.IO.MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            string fileName = $"BaoCao_07_BCNV_Quy{quy}_{nam}.pdf";
            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return File(stream, "application/pdf", fileName);
        }

        /// <summary>
        /// Xem trước PDF Mẫu số 12-CBH (Danh sách báo giảm hưởng, tạm dừng hưởng các chế độ BHXH hàng tháng, TCTN)
        /// </summary>
        [HttpGet("12-cbh-preview")]
        public IActionResult Preview12CbhPdf([FromQuery] int? thang = null, [FromQuery] int? nam = null)
        {
            var reportData = Domain.Models.BaoCao12CbhReportData.CreateSample();
            if (thang.HasValue) reportData.Thang = thang.Value;
            if (nam.HasValue) reportData.Nam = nam.Value;

            using var report = new Infrastructure.Reports.BaoCao12CbhReport(reportData);

            var stream = new System.IO.MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            int m = thang ?? 10;
            int y = nam ?? 2026;
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"BaoCao_12_CBH_Thang{m}_{y}.pdf\"");
            return File(stream, "application/pdf");
        }

        /// <summary>
        /// Xuất file PDF Mẫu số 12-CBH từ dữ liệu JSON Body truyền vào
        /// </summary>
        [HttpPost("12-cbh-export-pdf")]
        public IActionResult Export12CbhPdfFromBody([FromBody] Domain.Models.BaoCao12CbhReportData? data)
        {
            using var report = new Infrastructure.Reports.BaoCao12CbhReport(data);

            var stream = new System.IO.MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            var m = data?.Thang ?? 10;
            var y = data?.Nam ?? 2026;
            string fileName = $"BaoCao_12_CBH_Thang{m}_{y}.pdf";
            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return File(stream, "application/pdf", fileName);
        }

        /// <summary>
        /// Tải xuống file PDF Mẫu số 12-CBH (GET)
        /// </summary>
        [HttpGet("12-cbh-export-pdf")]
        public IActionResult Export12CbhPdf([FromQuery] int thang = 10, [FromQuery] int nam = 2026)
        {
            var reportData = Domain.Models.BaoCao12CbhReportData.CreateSample();
            reportData.Thang = thang;
            reportData.Nam = nam;
            using var report = new Infrastructure.Reports.BaoCao12CbhReport(reportData);

            var stream = new System.IO.MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            string fileName = $"BaoCao_12_CBH_Thang{thang}_{nam}.pdf";
            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return File(stream, "application/pdf", fileName);
        }
    }
}

