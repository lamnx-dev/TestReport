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
            new { id = "s11bh", name = "S11b-H — Sổ quỹ tiền mặt" },
            new { id = "s12h", name = "S12-H — Sổ tiền gửi ngân hàng, kho bạc" },
            new { id = "s31h", name = "S31-H — Sổ chi tiết phải thu kinh phí được cấp" },
            new { id = "s32h", name = "S32-H — Sổ theo dõi chi phí trả trước" },
            new { id = "s33h", name = "S33-H — Sổ chi tiết các khoản phải thu, phải trả nội bộ" },
            new { id = "s34h", name = "S34-H — Sổ chi tiết thanh toán với khách hàng" },
            new { id = "s36h", name = "S36-H — Sổ chi tiết tài sản thừa chờ xử lý" },
            new { id = "s37h", name = "S37-H — Sổ chi tiết tài sản thiếu chờ xử lý" },
            new { id = "s38h", name = "S38-H — Sổ chi tiết các tài khoản" },
            new { id = "s51h", name = "S51-H — Sổ chi tiết doanh thu" },
            new { id = "s52h", name = "S52-H — Sổ chi tiết doanh thu sản xuất, kinh doanh, dịch vụ" },
            new { id = "s53h", name = "S53-H — Sổ theo dõi thuế giá trị gia tăng" },
            new { id = "s54h", name = "S54-H — Sổ theo dõi thuế giá trị gia tăng được hoàn lại" },
            new { id = "s55h", name = "S55-H — Sổ theo dõi thuế giá trị gia tăng được miễn giảm" },
            new { id = "s106h", name = "S106-H — Sổ theo dõi kinh phí hoạt động nghiệp vụ" },
            new { id = "s101h", name = "S101-H — Sổ theo dõi dự toán" },
            new { id = "s90h", name = "S90-H — Bảng xác định chênh lệch thu, chi" },
            new { id = "s61h", name = "S61-H — Sổ chi tiết chi phí" },
            new { id = "s62h", name = "S62-H — Sổ chi tiết chi phí sản xuất kinh doanh dịch vụ dở dang" }
        },
        fonts = ReportAppearanceApplier.Fonts,
        minFontSize = ReportAppearanceApplier.MinFontSize,
        maxFontSize = ReportAppearanceApplier.MaxFontSize
    });

    [HttpPost("{reportId}/render")]
    public IActionResult Render(string reportId, [FromBody] ReportRenderRequest request)
    {
        if (reportId is not ("s11h" or "s11bh" or "s12h" or "s31h" or "s32h" or "s33h" or "s34h" or "s36h" or "s37h" or "s38h" or "s51h" or "s52h" or "s53h" or "s54h" or "s55h" or "s106h" or "s90h" or "s61h" or "s62h" or "s101h"))
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
