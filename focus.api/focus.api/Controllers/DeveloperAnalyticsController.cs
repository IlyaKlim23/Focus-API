using Focus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Focus.Api.Controllers;

[ApiController]
[Route("api/v1/developer/analytics")]
[Authorize(Roles = "Developer")]
public class DeveloperAnalyticsController(
    IDeveloperAnalyticsService service,
    IFeedbackService feedbackService) : ControllerBase
{
    [HttpGet("questionnaires")]
    public async Task<IActionResult> GetQuestionnaireAnalytics([FromQuery] int days = 30, CancellationToken ct = default)
    {
        var data = await service.GetQuestionnaireAnalyticsAsync(days, ct);
        return Ok(data);
    }

    [HttpGet("feedback")]
    public async Task<IActionResult> GetFeedback([FromQuery] int take = 200, CancellationToken ct = default) =>
        Ok(await feedbackService.GetRecentAsync(take, ct));
}
