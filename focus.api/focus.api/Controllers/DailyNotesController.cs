using System.Security.Claims;
using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Focus.Api.Controllers;

[ApiController]
[Route("api/v1/daily-notes")]
[Authorize]
public class DailyNotesController(IDailyNoteService dailyNoteService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId") ?? Guid.Empty.ToString());

    /// <summary>
    /// Получить заметку за дату
    /// </summary>
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(DailyNoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(string date, CancellationToken ct)
    {
        if (!DateOnly.TryParse(date, out var d))
            return BadRequest("Формат даты: yyyy-MM-dd");

        var note = await dailyNoteService.GetByDateAsync(UserId, d, ct);
        if (note == null) return NotFound();
        return Ok(note);
    }

    /// <summary>
    /// Создать или обновить заметку за день
    /// </summary>
    [HttpPost("{date}")]
    [ProducesResponseType(typeof(DailyNoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrUpdate(string date, [FromBody] CreateDailyNoteRequest request, CancellationToken ct)
    {
        if (!DateOnly.TryParse(date, out var d))
            return BadRequest("Формат даты: yyyy-MM-dd");

        var note = await dailyNoteService.CreateOrUpdateAsync(UserId, d, request, ct);
        return Ok(note);
    }
}
