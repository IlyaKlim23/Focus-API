using System.Security.Claims;
using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Focus.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ScheduleController(IScheduleService scheduleService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId") ?? Guid.Empty.ToString());

    /// <summary>
    /// Сгенерировать расписание на указанную дату
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ScheduleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Generate([FromBody] ScheduleRequest request, CancellationToken ct)
    {
        var req = request with { UserId = UserId };
        var schedule = await scheduleService.GenerateScheduleAsync(req, ct);
        return Ok(schedule);
    }
}
