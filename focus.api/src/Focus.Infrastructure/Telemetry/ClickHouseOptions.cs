namespace Focus.Infrastructure.Telemetry;

public class ClickHouseOptions
{
    public const string Section = "ClickHouse";
    public bool Enabled { get; set; }
    public string BaseUrl { get; set; } = "http://localhost:8123";
    public string Database { get; set; } = "default";
    public string User { get; set; } = "default";
    public string Password { get; set; } = string.Empty;
}
