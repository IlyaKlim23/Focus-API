using Focus.Domain.Entities;
using Focus.Domain.Interfaces;

namespace Focus.Infrastructure.Repositories;

/// <summary>
/// In-memory реализация для старта и тестирования. Замените на EF Core при подключении БД.
/// </summary>
public class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _tasks = [];
    private readonly object _lock = new();

    public Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var t = _tasks.FirstOrDefault(x => x.Id == id && x.UserId == userId);
            return Task.FromResult(t);
        }
    }

    public Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var query = _tasks.Where(x => x.UserId == userId).AsEnumerable();
            if (from.HasValue) query = query.Where(x => x.CreatedAt >= from.Value);
            if (to.HasValue) query = query.Where(x => x.CreatedAt <= to.Value);
            return Task.FromResult<IReadOnlyList<TaskItem>>(query.ToList());
        }
    }

    public Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct = default)
    {
        lock (_lock)
        {
            _tasks.Add(task);
            return Task.FromResult(task);
        }
    }

    public Task UpdateAsync(TaskItem task, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var idx = _tasks.FindIndex(x => x.Id == task.Id && x.UserId == task.UserId);
            if (idx >= 0) _tasks[idx] = task;
            return Task.CompletedTask;
        }
    }

    public Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        lock (_lock)
        {
            _tasks.RemoveAll(x => x.Id == id && x.UserId == userId);
            return Task.CompletedTask;
        }
    }
}
