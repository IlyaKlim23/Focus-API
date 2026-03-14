using Focus.Domain.Entities;
using Focus.Domain.Interfaces;

namespace Focus.Infrastructure.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = [];
    private readonly object _lock = new();

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        lock (_lock)
            return Task.FromResult(_users.FirstOrDefault(x => x.Id == id));
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        lock (_lock)
            return Task.FromResult(_users.FirstOrDefault(x => string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        lock (_lock)
        {
            _users.Add(user);
            return Task.FromResult(user);
        }
    }
}
