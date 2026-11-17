using Microsoft.AspNetCore.Mvc;
using TestReport.Api.Domain.Models;
using TestReport.Api.Infrastructure.Reports;
using TestReport.Api.Infrastructure.Renderers;

namespace TestReport.Api.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController(
    SampleReportFactory factory,
    ReportPdfRenderer renderer,
    ILogger<ReportsController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Catalog() => Ok(new
    {
        reports = new[]
        {
            new { id = "s11h", name = "S11-H — Sổ kế toán chi tiết tiền mặt" },
            new { id = "s12h", name = "S12-H — Sổ tiền gửi ngân hàng, kho bạc" },
            new { id = "s55h", name = "S55-H — Sổ theo dõi thuế giá trị gia tăng được miễn giảm" },
            new { id = "s106h", name = "S106-H — Sổ theo dõi kinh phí hoạt động nghiệp vụ" },
            new { id = "s101h", name = "S101-H — Sổ theo dõi dự toán" },
            new { id = "s90h", name = "S90-H — Bảng xác định chênh lệch thu, chi" },
            new { id = "s62h", name = "S62-H — Sổ chi tiết chi phí sản xuất kinh doanh dịch vụ dở dang" }
        },
        fonts = ReportAppearanceApplier.Fonts,
        minFontSize = ReportAppearanceApplier.MinFontSize,
        maxFontSize = ReportAppearanceApplier.MaxFontSize
    });

    [HttpPost("{reportId}/render")]
    public IActionResult Render(string reportId, [FromBody] ReportRenderRequest request)
    {
        if (reportId is not ("s11h" or "s12h" or "s55h" or "s106h" or "s90h" or "s62h" or "s101h"))
            return NotFound(new { message = "Không tìm thấy báo cáo." });
        try { ReportAppearanceApplier.Validate(request.Appearance); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }

        try
        {
            using var report = factory.Create(reportId)!;
            ReportAppearanceApplier.Apply(report, request.Appearance);
            return File(renderer.Render(report), "application/pdf", $"{reportId.ToUpperInvariant()}.pdf");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to render report {ReportId}", reportId);
            return Problem(statusCode: 500, title: "Không thể tạo báo cáo. Vui lòng thử lại.");
        }
    }
}
