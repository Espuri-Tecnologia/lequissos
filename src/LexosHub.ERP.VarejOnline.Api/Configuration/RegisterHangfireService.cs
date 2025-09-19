using Lexos.Hangfire;
using System.Diagnostics;

namespace System
{
    public static class RegisterHangfireService
    {
        public static WebApplicationBuilder RegisterHangfire(this WebApplicationBuilder builder)
        {
            if (Debugger.IsAttached)
                builder.Services.CriarServidorHangfireMemoria();
            else
            {
                var sectionHangfire = builder.Configuration.GetSection("Hangfire");
                var banco = sectionHangfire.GetValue<string>("RedisHangfireConn");
                var prefixo = sectionHangfire.GetValue<string>("Prefix");
                builder.Services.CriarServidorHangfireRedis(banco, prefixo, 20, null, false);
            }

            return builder;
        }
    }

}