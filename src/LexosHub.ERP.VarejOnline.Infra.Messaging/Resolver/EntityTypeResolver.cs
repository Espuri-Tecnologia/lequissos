using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher
{
    public static class EventTypeResolver
    {
        private static readonly Dictionary<string, Type> EventTypeMap =
            new(StringComparer.OrdinalIgnoreCase)
            {
            { nameof(IntegrationCreated), typeof(IntegrationCreated) },
            { nameof(ProductsRequested), typeof(ProductsRequested) },
            { nameof(CriarProdutosSimples), typeof(CriarProdutosSimples) },
            { nameof(CriarProdutosKits), typeof(CriarProdutosKits) },
            { nameof(CriarProdutosConfiguraveis), typeof(CriarProdutosConfiguraveis) },
            { nameof(PriceTablesRequested), typeof(PriceTablesRequested) },
            { nameof(PriceTablePageProcessed), typeof(PriceTablePageProcessed) },
            { nameof(CompaniesRequested), typeof(CompaniesRequested) },
            { nameof(StoresRequested), typeof(StoresRequested) },
            { nameof(InitialSync), typeof(InitialSync) }
            };

        public static bool TryResolve(string eventType, out Type type)
            => EventTypeMap.TryGetValue(eventType, out type);

        // opcional: método para registrar novos tipos em runtime
        public static void Register<TEvent>(string? alias = null) where TEvent : BaseEvent
            => EventTypeMap[alias ?? typeof(TEvent).Name] = typeof(TEvent);
    }
}

