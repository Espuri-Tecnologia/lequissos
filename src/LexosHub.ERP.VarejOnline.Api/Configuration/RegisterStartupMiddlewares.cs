using LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Context;
using Microsoft.EntityFrameworkCore;

namespace System
{
    public static class RegisterStartupMiddlewares
    {
        public static WebApplication SetupMiddlewares(this WebApplication app) //TODO - Transform into Nuget
        {
            if (!app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthorization();

            app.MapControllers();
            
            //app.UseHangfire(null, true);

            app.MapHealthChecks("");

            //Execute pending migrations
            using var scope = app.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dataContext.Database.Migrate();

            return app;
        }
    }
}