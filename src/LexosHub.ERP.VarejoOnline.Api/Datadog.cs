using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace LexosHub.ERP.VarejoOnline.Api;

public static class Datadog
{
    public static LoggerConfiguration Setup(HostBuilderContext context, LoggerConfiguration configuration)
    {
        if (context.HostingEnvironment.IsProduction())
        {
            configuration.MinimumLevel.Debug();
            configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Error);
        }

        configuration.Enrich.FromLogContext();
        configuration.Enrich.WithMachineName();
        configuration.Enrich.WithThreadId();
        configuration.Enrich.WithThreadName();
        configuration.Enrich.WithEnvironmentName();

        if (context.HostingEnvironment.IsProduction())
        {
            configuration.WriteTo.Console(new CompactJsonFormatter());
        }

        if (!context.HostingEnvironment.IsProduction())
        {
            configuration.WriteTo.Console();
        }

        return configuration;
    }
}