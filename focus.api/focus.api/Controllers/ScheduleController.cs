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

    [HttpGet("{date}")]
    [ProducesResponseType(typeof(ScheduleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDate(string date, CancellationToken ct)
    {
        if (!DateOnly.TryParse(date, out var d))
            return BadRequest("Формат даты: yyyy-MM-dd");
        var dt = d.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        return Ok(await scheduleService.GetScheduleAsync(UserId, dt, ct));
    }

    [HttpPost("slots")]
    [ProducesResponseType(typeof(ScheduleSlotDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddSlot([FromBody] CreateScheduleSlotRequest request, CancellationToken ct)
    {
        var slot = await scheduleService.AddManualSlotAsync(UserId, request, ct);
        return CreatedAtAction(nameof(GetByDate), new { date = DateOnly.FromDateTime(slot.SlotStart).ToString("yyyy-MM-dd") }, slot);
    }

    [HttpPut("slots/{id:guid}")]
    [ProducesResponseType(typeof(ScheduleSlotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSlot(Guid id, [FromBody] UpdateScheduleSlotRequest request, CancellationToken ct)
    {
        var slot = await scheduleService.UpdateManualSlotAsync(UserId, id, request, ct);
        return slot == null ? NotFound() : Ok(slot);
    }

    [HttpDelete("slots/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSlot(Guid id, CancellationToken ct)
    {
        await scheduleService.DeleteManualSlotAsync(UserId, id, ct);
        return NoContent();
    }
}
