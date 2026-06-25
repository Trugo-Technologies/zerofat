using Serilog.Events;

namespace ZeroFat.Infrastructure.Logging;

public class LoggingOptions
{
    public const string SectionName = "Logging";
    public string AppName { get; set; } = "InnovatePro.ERP";

    public Dictionary<string, string> LogLevel { get; set; } = new();
    public FileOptions? File { get; set; }
    public ElasticsearchOptions? Elasticsearch { get; set; }
    public EventLogOptions? EventLog { get; set; }
    public OpenTelemetryOptions? OpenTelemetry { get; set; }
}

public class ElasticsearchOptions
{
    public bool IsEnabled { get; set; }

    public string? Host { get; set; }

    public string? IndexFormat { get; set; }

    public LogEventLevel MinimumLogEventLevel { get; set; }
}

public class FileOptions
{
    public LogEventLevel MinimumLogEventLevel { get; set; }
}

public class EventLogOptions
{
    public bool IsEnabled { get; set; }

    public string? LogName { get; set; }

    public string? SourceName { get; set; }
}

public class OpenTelemetryOptions
{
    public bool IsEnabled { get; set; }

    public string? ServiceName { get; set; }

    public OtlpOptions? Otlp { get; set; }
}

public class OtlpOptions
{
    public string? Endpoint { get; set; }
}

