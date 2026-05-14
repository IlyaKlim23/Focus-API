using System.Security.Claims;
using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Focus.Api.Controllers;

[ApiController]
[Route("api/v1/feedback")]
[Authorize]
public class FeedbackController(IFeedbackService service) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId") ?? Guid.Empty.ToString());

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FeedbackDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine([FromQuery] int take = 50, CancellationToken ct = default) =>
        Ok(await service.GetMineAsync(UserId, take, ct));

    [HttpPost]
    [ProducesResponseType(typeof(FeedbackDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateFeedbackRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(UserId, request, ct);
        return CreatedAtAction(nameof(GetMine), new { take = 1 }, result);
    }
}
