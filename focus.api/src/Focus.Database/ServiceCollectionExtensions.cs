using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Focus.Database;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавляет FocusDbContext и настраивает подключение к PostgreSQL
    /// </summary>
    public static IServiceCollection AddFocusDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<FocusDbContext>(options =>
            options.UseNpgsql(connectionString));
        return services;
    }
}
