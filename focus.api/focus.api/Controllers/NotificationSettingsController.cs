using System.Security.Claims;
using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Focus.Api.Controllers;

[ApiController]
[Route("api/v1/notification-settings")]
[Authorize]
public class NotificationSettingsController(INotificationPreferenceService service) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId") ?? Guid.Empty.ToString());

    [HttpGet]
    [ProducesResponseType(typeof(NotificationPreferenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await service.GetAsync(UserId, ct);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(NotificationPreferenceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Upsert([FromBody] UpsertNotificationPreferenceRequest request, CancellationToken ct)
    {
        var result = await service.UpsertAsync(UserId, request, ct);
        return Ok(result);
    }
}
