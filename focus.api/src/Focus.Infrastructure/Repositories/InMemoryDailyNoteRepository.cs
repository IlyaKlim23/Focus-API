using Focus.Domain.Entities;
using Focus.Domain.Interfaces;

namespace Focus.Infrastructure.Repositories;

public class InMemoryDailyNoteRepository : IDailyNoteRepository
{
    private readonly List<DailyNote> _notes = [];
    private readonly object _lock = new();

    public Task<DailyNote?> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken ct = default)
    {
        lock (_lock)
            return Task.FromResult(_notes.FirstOrDefault(x => x.UserId == userId && x.Date == date));
    }

    public Task<IReadOnlyList<DailyNote>> GetByUserAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var list = _notes.Where(x => x.UserId == userId && x.Date >= from && x.Date <= to).ToList();
            return Task.FromResult<IReadOnlyList<DailyNote>>(list);
        }
    }

    public Task<DailyNote> AddAsync(DailyNote note, CancellationToken ct = default)
    {
        lock (_lock)
        {
            _notes.Add(note);
            return Task.FromResult(note);
        }
    }

    public Task UpdateAsync(DailyNote note, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var idx = _notes.FindIndex(x => x.Id == note.Id && x.UserId == note.UserId);
            if (idx >= 0) _notes[idx] = note;
            return Task.CompletedTask;
        }
    }
}
