using Microsoft.Extensions.Logging;

namespace Focus.Infrastructure.Telemetry;

public class ClickHouseLoggerProvider(IClickHouseTelemetrySink sink) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new ClickHouseLogger(categoryName, sink);
    public void Dispose() { }

    private sealed class ClickHouseLogger(string categoryName, IClickHouseTelemetrySink sink) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            var message = formatter(state, exception);
            sink.EnqueueLog(logLevel.ToString(), categoryName, message, exception?.ToString());
        }
    }
}
