using System.Security.Claims;
using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Focus.Api.Controllers;

[ApiController]
[Route("api/v1/psychological-questionnaires")]
[Authorize]
public class PsychologicalQuestionnairesController(IPsychologicalQuestionnaireService service) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId") ?? Guid.Empty.ToString());

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<QuestionnaireDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await service.GetActiveAsync(ct));

    [HttpGet("{id:guid}/questions")]
    [ProducesResponseType(typeof(IReadOnlyList<QuestionnaireQuestionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuestions(Guid id, CancellationToken ct) =>
        Ok(await service.GetQuestionsAsync(id, ct));

    [HttpGet("schedules")]
    [ProducesResponseType(typeof(IReadOnlyList<UserQuestionnaireScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchedules(CancellationToken ct) =>
        Ok(await service.GetUserSchedulesAsync(UserId, ct));

    [HttpPut("schedules")]
    [ProducesResponseType(typeof(UserQuestionnaireScheduleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpsertSchedule([FromBody] UpsertQuestionnaireScheduleRequest request, CancellationToken ct) =>
        Ok(await service.UpsertScheduleAsync(UserId, request, ct));

    [HttpPost("responses")]
    [ProducesResponseType(typeof(QuestionnaireResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Submit([FromBody] SubmitQuestionnaireRequest request, CancellationToken ct) =>
        Ok(await service.SubmitAsync(UserId, request, ct));

    [HttpGet("{id:guid}/responses")]
    [ProducesResponseType(typeof(IReadOnlyList<QuestionnaireResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent(Guid id, [FromQuery] int take = 20, CancellationToken ct = default) =>
        Ok(await service.GetRecentResponsesAsync(UserId, id, take, ct));
}
