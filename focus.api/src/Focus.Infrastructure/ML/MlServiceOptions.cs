namespace Focus.Infrastructure.ML;

public class MlServiceOptions
{
    public const string Section = "MlService";

    /// <summary>Базовый URL ML-сервиса, например http://localhost:8080</summary>
    public string BaseUrl { get; set; } = "";

    /// <summary>Секрет для X-Api-Key (совпадает с ML_API_KEY на Python). Пусто — без заголовка (dev).</summary>
    public string? ApiKey { get; set; }
}
