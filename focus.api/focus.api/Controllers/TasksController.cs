using System.Security.Claims;
using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Focus.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TasksController(ITaskService taskService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId") ?? Guid.Empty.ToString());

    /// <summary>
    /// Список задач пользователя
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var tasks = await taskService.GetByUserAsync(UserId, from, to, ct);
        return Ok(tasks);
    }

    /// <summary>
    /// Получить задачу по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var task = await taskService.GetByIdAsync(id, UserId, ct);
        if (task == null) return NotFound();
        return Ok(task);
    }

    /// <summary>
    /// Создать задачу
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var task = await taskService.CreateAsync(UserId, request, ct);
        return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
    }

    /// <summary>
    /// Обновить задачу
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request, CancellationToken ct)
    {
        var task = await taskService.UpdateAsync(id, UserId, request, ct);
        if (task == null) return NotFound();
        return Ok(task);
    }

    /// <summary>
    /// Удалить задачу
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await taskService.DeleteAsync(id, UserId, ct);
        return NoContent();
    }
}
