namespace LexosHub.ERP.VarejoOnline.Api.Configuration;

public static class BaseConfiguration
{
    public static IConfiguration Config()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration;
    }
}