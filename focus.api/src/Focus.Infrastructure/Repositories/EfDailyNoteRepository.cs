using Focus.Database;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Focus.Infrastructure.Repositories;

public class EfDailyNoteRepository(FocusDbContext db) : IDailyNoteRepository
{
    public async Task<DailyNote?> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken ct = default) =>
        await db.DailyNotes.FirstOrDefaultAsync(x => x.UserId == userId && x.Date == date, ct);

    public async Task<IReadOnlyList<DailyNote>> GetByUserAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default) =>
        await db.DailyNotes
            .Where(x => x.UserId == userId && x.Date >= from && x.Date <= to)
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

    public async Task<DailyNote> AddAsync(DailyNote note, CancellationToken ct = default)
    {
        db.DailyNotes.Add(note);
        await db.SaveChangesAsync(ct);
        return note;
    }

    public async Task UpdateAsync(DailyNote note, CancellationToken ct = default)
    {
        db.DailyNotes.Update(note);
        await db.SaveChangesAsync(ct);
    }
}
