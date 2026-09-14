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

        #region Báo cáo nhiều cột phân tách trang in (Multi-Column Split)

        private MultiColumnReportData GenerateMultiColumnData(int totalColumns = 18, int columnsPerPage = 6, int rowCount = 100)
        {
            totalColumns = Math.Clamp(totalColumns, 2, 50);
            columnsPerPage = Math.Clamp(columnsPerPage, 2, 12);
            rowCount = Math.Clamp(rowCount, 1, 1000);

            var reportData = new MultiColumnReportData
            {
                TotalColumns = totalColumns,
                ColumnsPerPage = columnsPerPage
            };

            for (int i = 1; i <= totalColumns; i++)
            {
                reportData.ColumnNames.Add($"Cột {i}");
            }

            for (int r = 1; r <= rowCount; r++)
            {
                var row = new MultiColumnRow { Stt = r };
                for (int c = 1; c <= totalColumns; c++)
                {
                    // Sinh dữ liệu mẫu mô phỏng số liệu kinh doanh / thống kê
                    int val = (r * 10) + c * 3 + Random.Shared.Next(10, 99);
                    row.Values[$"Cột {c}"] = val.ToString("#,##0");
                }
                reportData.Rows.Add(row);
            }

            return reportData;
        }

        /// <summary>
        /// Xem trước PDF báo cáo nhiều cột trực tiếp trên trình duyệt
        /// </summary>
        [HttpGet("multi-column-preview")]
        public IActionResult PreviewMultiColumnPdf(
            [FromQuery] int totalColumns = 18,
            [FromQuery] int columnsPerPage = 6,
            [FromQuery] int rowCount = 100)
        {
            var data = GenerateMultiColumnData(totalColumns, columnsPerPage, rowCount);
            using var report = new MultiColumnSplitReport(data);
            var stream = new MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            Response.Headers.Append("Content-Disposition", "inline; filename=\"BaoCao_NhieuCot_TachTrang.pdf\"");
            return File(stream, "application/pdf");
        }

        /// <summary>
        /// Tải xuống file PDF báo cáo nhiều cột
        /// </summary>
        [HttpGet("multi-column-export-pdf")]
        public IActionResult ExportMultiColumnPdf(
            [FromQuery] int totalColumns = 18,
            [FromQuery] int columnsPerPage = 6,
            [FromQuery] int rowCount = 100)
        {
            var data = GenerateMultiColumnData(totalColumns, columnsPerPage, rowCount);
            using var report = new MultiColumnSplitReport(data);
            var stream = new MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            Response.Headers.Append("Content-Disposition", "attachment; filename=\"BaoCao_NhieuCot_TachTrang.pdf\"");
            return File(stream, "application/pdf", "BaoCao_NhieuCot_TachTrang.pdf");
        }


        /// <summary>
        /// Trả về chuỗi Base64 của PDF báo cáo nhiều cột cho Frontend Viewer
        /// </summary>
        [HttpGet("multi-column-base64")]
        public IActionResult GetMultiColumnBase64(
            [FromQuery] int totalColumns = 18,
            [FromQuery] int columnsPerPage = 6,
            [FromQuery] int rowCount = 100)
        {
            var data = GenerateMultiColumnData(totalColumns, columnsPerPage, rowCount);
            using var report = new MultiColumnSplitReport(data);
            report.CreateDocument();
            int totalPages = report.Pages.Count;

            using var stream = new MemoryStream();
            report.ExportToPdf(stream);

            byte[] bytes = stream.ToArray();
            string base64String = Convert.ToBase64String(bytes);

            return Ok(new
            {
                fileName = "BaoCao_NhieuCot_TachTrang.pdf",
                contentType = "application/pdf",
                fileSize = bytes.Length,
                totalRecords = data.Rows.Count,
                totalColumns = data.TotalColumns,
                columnsPerPage = data.ColumnsPerPage,
                totalPages = totalPages,
                base64 = base64String,
                dataUrl = $"data:application/pdf;base64,{base64String}"
            });
        }

        /// <summary>
        /// Lấy dữ liệu dạng JSON thô để preview hoặc xuất dữ liệu
        /// </summary>
        [HttpGet("multi-column-data")]
        public IActionResult GetMultiColumnData(
            [FromQuery] int totalColumns = 18,
            [FromQuery] int columnsPerPage = 6,
            [FromQuery] int rowCount = 100)

        {
            var data = GenerateMultiColumnData(totalColumns, columnsPerPage, rowCount);
            return Ok(data);
        }

        #endregion
    }
}

