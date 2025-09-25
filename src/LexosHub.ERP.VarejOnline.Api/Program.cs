using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Lexos.DevEnv;
using Lexos.SQS;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Api.Jobs;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Persistence;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.SyncProcess;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Webhook;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Domain.Services;
using LexosHub.ERP.VarejOnline.Domain.Validators;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Context;
using LexosHub.ERP.VarejOnline.Infra.Data.Repositories.Integration;
using LexosHub.ERP.VarejOnline.Infra.Data.Repositories.Persistence;
using LexosHub.ERP.VarejOnline.Infra.Data.Repositories.SyncProcess;
using LexosHub.ERP.VarejOnline.Infra.Data.Repositories.Webhook;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto;
using LexosHub.ERP.VarejOnline.Infra.SyncIn.Interfaces;
using LexosHub.ERP.VarejOnline.Infra.SyncIn.Services;
using LexosHub.ERP.VarejOnline.Infra.SyncOut.Interfaces;
using LexosHub.ERP.VarejOnline.Infra.SyncOut.Services;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Services;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Diagnostics;

try
{
    Log.Information("Starting web application.");

    var builder = WebApplication.CreateBuilder(args);
    //builder.Host.UseSerilog((context, configuration) => LexosHub.ERP.VarejOnline.Api.Datadog.Setup(context, configuration));

    builder.RegisterHangfire();

    builder.Host.ConfigureAppConfiguration((context, config) =>
    {
        var localDev = context.HostingEnvironment.EnvironmentName == "LocalBdDev";
        context.Configuration.UseLexosEnv(localDev ? "LocalBdDev" : "LocalBdProd", "lexoshub-varejoonline");

        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        config.AddEnvironmentVariables("LX_");
    });

    Log.Information($"Environment: {builder.Environment.EnvironmentName}");
    Log.Information($"ConnectionString ErpDBConn: {DatabaseHandler.MontarConexao(builder.Configuration)}");

    builder.RegisterServices();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(DatabaseHandler.MontarConexao(builder.Configuration))
    );

    builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
    builder.Services.AddScoped<IValidator<HubIntegracaoDto>, HubIntegracaoDtoValidator>();

    builder.Services.AddOptions<SyncOutConfig>().Bind(builder.Configuration.GetSection(nameof(SyncOutConfig)));
    builder.Services.AddOptions<SyncInConfig>().Bind(builder.Configuration.GetSection(nameof(SyncInConfig)));
    builder.Services.AddOptions<VarejOnlineSqsConfig>().Bind(builder.Configuration.GetSection(nameof(VarejOnlineSqsConfig)));

    builder.Services.AddTransient<IIntegrationService, IntegrationService>();
    builder.Services.AddTransient<IAuthService, AuthService>();
    builder.Services.AddTransient<ISyncProcessService, SyncProcessService>();
    builder.Services.AddTransient<IVarejOnlineApiService, VarejOnlineApiService>();
    builder.Services.AddTransient<ISqsRepository, SqsRepository>();
    builder.Services.AddScoped<StockSyncJobService>();
    builder.Services.AddTransient<IWebhookService, WebhookService>();
    builder.Services.AddTransient<ISyncOutApiService, SyncOutApiService>();
    builder.Services.AddTransient<ISyncInApiService, SyncInApiService>();
    builder.Services.AddTransient<ProdutoViewMapper>();
    builder.Services.AddHttpClient();

    builder.Services.AddScoped<IIntegrationRepository, IntegrationRepository>();
    builder.Services.AddScoped<IWebhookRepository, WebhookRepository>();
    builder.Services.AddScoped<ISyncProcessRepository, SyncProcessRepository>();
    builder.Services.AddScoped<ISyncProcessItemRepository, SyncProcessItemRepository>();

    builder.Services.AddScoped<IApplicationWriteDbConnection, ApplicationWriteDbConnection>();
    builder.Services.AddScoped<IApplicationReadDbConnection, ApplicationReadDbConnection>();

    builder.Services.AddFluentValidationAutoValidation(conf => { conf.DisableDataAnnotationsValidation = true; });

    builder.Services.AddOptions<VarejOnlineApiSettings>().Bind(builder.Configuration.GetSection(nameof(VarejOnlineApiSettings)));

    builder.Services.AddSingleton<EventDispatcher>();
    builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
    builder.Services.AddSingleton<ISqslEventPublisher, SqslEventPublisher>();
    builder.Services.AddTransient<IEventHandler<IntegrationCreated>, IntegrationCreatedEventHandler>();
    builder.Services.AddTransient<IEventHandler<InitialSync>, InitialSyncEventHandler>();
    builder.Services.AddTransient<IEventHandler<ProductsRequested>, ProductsRequestedEventHandler>();
    builder.Services.AddTransient<IEventHandler<StocksRequested>, StocksRequestedEventHandler>();
    builder.Services.AddTransient<IEventHandler<CriarProdutosSimples>, CriarProdutosSimplesEventHandler>();
    builder.Services.AddTransient<IEventHandler<OrderCreated>, OrderCreatedEventHandler>();
    builder.Services.AddTransient<IEventHandler<OrderCancelled>, OrderCancelledEventHandler>();
    builder.Services.AddTransient<IEventHandler<OrderShipped>, OrderShippedEventHandler>();
    builder.Services.AddTransient<IEventHandler<OrderDelivered>, OrderDeliveredEventHandler>();
    builder.Services.AddTransient<IEventHandler<CriarProdutosConfiguraveis>, CriarProdutosConfiguraveisEventHandler>();
    builder.Services.AddTransient<IEventHandler<CriarProdutosKits>, CriarProdutosKitsEventHandler>();
    builder.Services.AddTransient<CriarProdutosConfiguraveisEventHandler>();
    builder.Services.AddTransient<IEventHandler<PriceTablesRequested>, PriceTablesRequestedEventHandler>();
    builder.Services.AddTransient<IEventHandler<PriceTablePageProcessed>, PriceTablesPageProcessedEventHandler>();
    builder.Services.AddTransient<IEventHandler<CompaniesRequested>, CompaniesRequestedEventHandler>();
    builder.Services.AddTransient<IEventHandler<StoresRequested>, StoresRequestedEventHandler>();
    builder.Services.AddTransient<IEventHandler<RegisterDefaultWebhooks>, RegisterDefaultWebhooksEventHandler>();
    builder.Services.AddTransient<IEventHandler<InvoicesRequested>, InvoicesRequestedEventHandler>();
    builder.Services.AddHostedService<SqsListenerService>();
   
    builder.Services.AddTransient<ISqsRepository, SqsRepository>();

    var app = builder.Build().SetupMiddlewares();

    if (app.Environment.IsDevelopment() || Debugger.IsAttached)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (Debugger.IsAttached)
    {
        using var scope = app.Services.CreateScope();
        var recurring = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        recurring.AddOrUpdate<StockSyncJobService>(
            StockSyncJobService.RecurringJobId,
            svc => svc.RunAsync(CancellationToken.None),
            "* * * * *"
        );
    }
    else
    {
        //RecurringJob.AddOrUpdate<SyncProcessJobService>(SyncProcessJobService.SyncStockContinuousJobId, x => x.AddSyncProcessToNewIntegrationJob(null), "*/3 * * * *");
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