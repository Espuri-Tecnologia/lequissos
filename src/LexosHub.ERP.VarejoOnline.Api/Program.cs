using Amazon.SQS;
using FluentValidation;
using FluentValidation.AspNetCore;
using Lexos.SQS;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Persistence;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Domain.Services;
using LexosHub.ERP.VarejoOnline.Domain.Validators;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Context;
using LexosHub.ERP.VarejoOnline.Infra.Data.Repositories.Integration;
using LexosHub.ERP.VarejoOnline.Infra.Data.Repositories.Persistence;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Services;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;

try
{
    Log.Information("Starting web application.");

    var builder = WebApplication.CreateBuilder(args);
    //builder.Host.UseSerilog((context, configuration) => LexosHub.ERP.VarejoOnline.Api.Datadog.Setup(context, configuration));

    Log.Information($"Environment: {builder.Environment.EnvironmentName}");
    Log.Information($"ConnectionString ErpDBConn: {builder.Configuration.GetConnectionString("ErpDBConn")}");

    builder.RegisterServices();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ErpDBConn"))
    );

    builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
    builder.Services.AddScoped<IValidator<HubIntegracaoDto>, HubIntegracaoDtoValidator>();

    builder.Services.AddOptions<SyncOutConfig>().Bind(builder.Configuration.GetSection(nameof(SyncOutConfig)));
    builder.Services.AddOptions<SyncInConfig>().Bind(builder.Configuration.GetSection(nameof(SyncInConfig)));

    builder.Services.AddTransient<IIntegrationService, IntegrationService>();
    builder.Services.AddTransient<IAuthService, AuthService>();
    builder.Services.AddTransient<IVarejoOnlineApiService, VarejoOnlineApiService>();
    builder.Services.AddTransient<ISqsRepository, SqsRepository>();

    builder.Services.AddScoped<IIntegrationRepository, IntegrationRepository>();

    builder.Services.AddScoped<IApplicationWriteDbConnection, ApplicationWriteDbConnection>();
    builder.Services.AddScoped<IApplicationReadDbConnection, ApplicationReadDbConnection>();

    builder.Services.AddFluentValidationAutoValidation(conf => { conf.DisableDataAnnotationsValidation = true; });

    builder.Services.AddOptions<VarejoOnlineApiSettings>().Bind(builder.Configuration.GetSection(nameof(VarejoOnlineApiSettings)));

    builder.Services.AddSingleton<IAmazonSQS>(sp =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        var sqsConfig = new AmazonSQSConfig
        {
            ServiceURL = config["AWS:ServiceURL"]
        };

        return new AmazonSQSClient("test", "test", sqsConfig);
    });
    builder.Services.AddSingleton<EventDispatcher>();
    builder.Services.AddSingleton<IEventDispatcher, SqsEventDispatcher>();
    builder.Services.AddTransient<IEventHandler<IntegrationCreated>, IntegrationCreatedEventHandler>();
    builder.Services.AddTransient<IEventHandler<InitialSync>, InitialSyncEventHandler>();
    builder.Services.AddTransient<IEventHandler<ProductsRequested>, ProductsRequestedEventHandler>();
    builder.Services.AddTransient<IEventHandler<ProductsPageProcessed>, ProductsPageProcessedEventHandler>();
    builder.Services.AddTransient<IEventHandler<PriceTablesPageProcessed>, PriceTablesPageProcessedEventHandler>();
    builder.Services.AddTransient<IEventHandler<CompaniesRequested>, CompaniesRequestedEventHandler>();
    builder.Services.AddHostedService<SqsListenerService>();

    var app = builder.Build().SetupMiddlewares();

    if (app.Environment.IsDevelopment() || Debugger.IsAttached)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (Debugger.IsAttached)
    {

    }
    else
    {
        //RecurringJob.AddOrUpdate<SyncProcessJobService>(SyncProcessJobService.AddSyncProcessToNewIntegrationJobId, x => x.AddSyncProcessToNewIntegrationJob(null), "*/3 * * * *");
        //RecurringJob.AddOrUpdate<SyncProcessJobService>(SyncProcessJobService.RunScheduleSyncProcessRecurringJobId, x => x.RunScheduleSyncProcessRecurringJob(null), "*/3 * * * *");
        //RecurringJob.AddOrUpdate<SyncProcessJobService>(SyncProcessJobService.AddSyncProcessToNewIntegrationJobId, x => x.AddSyncProcessToNewIntegrationJob(null), "*/3 * * * *");
        //RecurringJob.AddOrUpdate<SyncProcessJobService>(SyncProcessJobService.RunCheckStoppedIntegrationsJobId, x => x.RunCheckStoppedIntegrationsJob(null), "*/10 * * * *");
        //RecurringJob.AddOrUpdate<SyncProcessJobService>(SyncProcessJobService.RunAuditInitialSyncActionId, x => x.RunAuditInitialSyncAction(null), "0 3 * * *");
        //RecurringJob.AddOrUpdate<SyncProcessJobService>(SyncProcessJobService.RunGetNewOrdersJobId, x => x.RunGetNewOrdersJob(null), "*/2 * * * *");
        //RecurringJob.AddOrUpdate<SyncProcessJobService>(SyncProcessJobService.RunGetOrdersUpdatesJobId, x => x.RunGetOrdersUpdatesJob(null), "*/3 * * * *");
        //RecurringJob.AddOrUpdate<SyncProcessJobService>(SyncProcessJobService.RunDeleteOldItemsFromDbJobId, x => x.RunDeleteOldItemsFromDbJob(null), "0 4 * * *");
        //RecurringJob.AddOrUpdate<SyncProcessJobService>(SyncProcessJobService.ExecuteRetriesJobId, x => x.ExecuteRetriesJob(null), "*/5 * * * *");
    }

    //GlobalJobFilters.Filters.Add(new CustomExpirationTimeAttribute(0, 0, 30));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}