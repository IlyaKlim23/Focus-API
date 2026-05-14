using Focus.Domain.Interfaces;
using Focus.Infrastructure.Nlp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Focus.Infrastructure.ML;

public static class MlServiceCollectionExtensions
{
    public static IServiceCollection AddFocusMlClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MlServiceOptions>(configuration.GetSection(MlServiceOptions.Section));
        services.AddTransient<MlApiKeyHandler>();

        services
            .AddHttpClient("MlService", (sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<MlServiceOptions>>().Value;
                if (!string.IsNullOrWhiteSpace(opts.BaseUrl))
                    client.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromSeconds(60);
            })
            .AddHttpMessageHandler<MlApiKeyHandler>();

        services.AddScoped<IProductivityPredictor, HttpProductivityPredictor>();
        services.AddScoped<INlpAnalyzer, HttpNlpAnalyzer>();

        return services;
    }
}
