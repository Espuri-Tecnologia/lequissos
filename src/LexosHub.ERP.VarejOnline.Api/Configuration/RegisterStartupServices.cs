using Microsoft.OpenApi.Models;

namespace System
{
    public static class RegisterStartupServices
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder) //TODO - Transform into Nuget
        {
            var configuration = builder.Configuration;

            //var sectionHangfire = builder.Configuration.GetSection("RedisCache");
            //var conn = sectionHangfire.GetValue<string>("CacheConn");
            //var instanceName = sectionHangfire.GetValue<string>("InstanceName");

            //builder.Services.CriarCacheDistribuidoRedis(instanceName, conn);

            builder.Services.AddHealthChecks(); //TODO - HealthCheck Continue this implementation - https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", configuration.GetSection("OpenApi").Get<OpenApiInfo>());
            });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return builder;
        }
    }
}