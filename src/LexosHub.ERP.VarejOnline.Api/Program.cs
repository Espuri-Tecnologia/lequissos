using Amazon.SQS;
using FluentValidation;
using FluentValidation.AspNetCore;
using Lexos.SQS;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Persistence;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Webhook;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Domain.Services;
using LexosHub.ERP.VarejOnline.Domain.Validators;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Context;
using LexosHub.ERP.VarejOnline.Infra.Data.Repositories.Integration;
using LexosHub.ERP.VarejOnline.Infra.Data.Repositories.Persistence;
using LexosHub.ERP.VarejOnline.Infra.Data.Repositories.Webhook;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Services;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;

try
{
    Log.Information("Starting web application.");

    var builder = WebApplication.CreateBuilder(args);
    //builder.Host.UseSerilog((context, configuration) => LexosHub.ERP.VarejOnline.Api.Datadog.Setup(context, configuration));

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
    builder.Services.AddTransient<IVarejOnlineApiService, VarejOnlineApiService>();
    builder.Services.AddTransient<ISqsRepository, SqsRepository>();
    builder.Services.AddTransient<IWebhookService, WebhookService>();
    builder.Services.AddTransient<ProdutoViewMapper>();
    builder.Services.AddHttpClient();

    builder.Services.AddScoped<IIntegrationRepository, IntegrationRepository>();
    builder.Services.AddScoped<IWebhookRepository, WebhookRepository>();

    builder.Services.AddScoped<IApplicationWriteDbConnection, ApplicationWriteDbConnection>();
    builder.Services.AddScoped<IApplicationReadDbConnection, ApplicationReadDbConnection>();

    builder.Services.AddFluentValidationAutoValidation(conf => { conf.DisableDataAnnotationsValidation = true; });

    builder.Services.AddOptions<VarejOnlineApiSettings>().Bind(builder.Configuration.GetSection(nameof(VarejOnlineApiSettings)));

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
    builder.Services.AddTransient<IEventHandler<CriarProdutosSimples>, CriarProdutosSimplesEventHandler>();
    builder.Services.AddTransient<IEventHandler<CriarProdutosConfiguraveis>, CriarProdutosConfiguraveisEventHandler>();
    builder.Services.AddTransient<IEventHandler<CriarProdutosKits>, CriarProdutosKitsEventHandler>();
    builder.Services.AddTransient<CriarProdutosConfiguraveisEventHandler>();
    builder.Services.AddTransient<IEventHandler<PriceTablesRequested>, PriceTablesRequestedEventHandler>();
    builder.Services.AddTransient<IEventHandler<PriceTablePageProcessed>, PriceTablesPageProcessedEventHandler>();
    builder.Services.AddTransient<IEventHandler<CompaniesRequested>, CompaniesRequestedEventHandler>();
    builder.Services.AddTransient<IEventHandler<StoresRequested>, StoresRequestedEventHandler>();
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