using Microsoft.Extensions.Options;

namespace Focus.Infrastructure.ML;

/// <summary>Добавляет X-Api-Key к каждому запросу, если задан MlService:ApiKey.</summary>
public sealed class MlApiKeyHandler(IOptionsMonitor<MlServiceOptions> options) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var key = options.CurrentValue.ApiKey;
        if (!string.IsNullOrWhiteSpace(key))
        {
            request.Headers.Remove("X-Api-Key");
            request.Headers.TryAddWithoutValidation("X-Api-Key", key);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
