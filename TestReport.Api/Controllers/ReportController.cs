using Microsoft.AspNetCore.Mvc;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        [HttpGet("s90-h-preview")]
        public IActionResult PreviewS90HPdf()
        {
            using var report = new Infrastructure.Reports.S90HReport(S90HReportData.CreateSample());
            var stream = new MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;
            Response.Headers.Append("Content-Disposition", "inline; filename=\"S90-H.pdf\"");
            return File(stream, "application/pdf");
        }

        [HttpPost("s90-h-export-pdf")]
        public IActionResult ExportS90HPdf([FromBody] S90HReportData data)
        {
            if (data is null) return BadRequest(new { message = "Cần cung cấp dữ liệu báo cáo S90-H." });
            try
            {
                using var report = new Infrastructure.Reports.S90HReport(data);
                var stream = new MemoryStream();
                report.ExportToPdf(stream);
                stream.Position = 0;
                return File(stream, "application/pdf", "S90-H.pdf");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
