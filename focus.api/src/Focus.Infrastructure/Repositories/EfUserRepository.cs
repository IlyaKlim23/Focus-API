using Focus.Database;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Focus.Infrastructure.Repositories;

public class EfUserRepository(FocusDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Users.FindAsync([id], ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await db.Users.FirstOrDefaultAsync(x => x.Email == email, ct);

    public async Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }
}
