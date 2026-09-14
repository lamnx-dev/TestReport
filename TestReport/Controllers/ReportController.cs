using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace TestReport.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Nắng đẹp", "Mát mẻ", "Mưa rào nhẹ", "Nhiều mây", "Ấm áp", "Gió nhẹ", "Có sương mù"
        ];

        private List<WeatherForecast> GenerateSampleData(int count = 150)
        {
            var startDate = DateTime.Now.AddDays(-count);
            return Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(startDate.AddDays(index)),
                TemperatureC = Random.Shared.Next(15, 38),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToList();
        }

        /// <summary>
        /// Xem trước báo cáo PDF trực tiếp trên trình duyệt
        /// </summary>
        [HttpGet("preview")]
        public IActionResult PreviewPdf([FromQuery] int count = 150)
        {
            var data = GenerateSampleData(count);
            using var report = new Report1(data);
            var stream = new MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            Response.Headers.Append("Content-Disposition", "inline; filename=BaoCao_Report1.pdf");
            return File(stream, "application/pdf");
        }

        /// <summary>
        /// Tải xuống báo cáo dưới định dạng PDF
        /// </summary>
        [HttpGet("export-pdf")]
        public IActionResult ExportPdf([FromQuery] int count = 150)
        {
            var data = GenerateSampleData(count);
            using var report = new Report1(data);
            var stream = new MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", "BaoCao_Report1.pdf");
        }

        /// <summary>
        /// Tải xuống báo cáo dưới định dạng Excel (.xlsx)
        /// </summary>
        [HttpGet("export-excel")]
        public IActionResult ExportExcel([FromQuery] int count = 150)
        {
            var data = GenerateSampleData(count);
            using var report = new Report1(data);
            var stream = new MemoryStream();
            report.ExportToXlsx(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCao_Report1.xlsx");
        }

        /// <summary>
        /// Trả về chuỗi Base64 của file PDF để Frontend hiển thị (mặc định 150 dòng ~ 5 trang)
        /// </summary>
        [HttpGet("base64")]
        public IActionResult GetPdfBase64([FromQuery] int count = 150)
        {
            var data = GenerateSampleData(count);
            using var report = new Report1(data);
            report.CreateDocument();
            int totalPages = report.Pages.Count;

            using var stream = new MemoryStream();
            report.ExportToPdf(stream);

            byte[] bytes = stream.ToArray();
            string base64String = Convert.ToBase64String(bytes);

            return Ok(new
            {
                fileName = "BaoCao_Report1.pdf",
                contentType = "application/pdf",
                fileSize = bytes.Length,
                totalRecords = data.Count,
                totalPages = totalPages,
                base64 = base64String,
                dataUrl = $"data:application/pdf;base64,{base64String}"
            });
        }
    }
}
