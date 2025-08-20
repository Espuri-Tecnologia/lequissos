using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using System;
using System.Collections.Generic;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher
{
    public static class EventTypeResolver
    {
        private static readonly Dictionary<string, Type> EventTypeMap = new()
        {
            { "IntegrationCreated", typeof(IntegrationCreated) },
            { "ProductsRequested", typeof(ProductsRequested) },
            { "CriarProdutosSimples", typeof(CriarProdutosSimples) },
            { "CriarProdutosKits", typeof(CriarProdutosKits) },
            { "CriarProdutosConfiguraveis", typeof(CriarProdutosConfiguraveis) },
            { "PriceTablesRequested", typeof(PriceTablesRequested) },
            { "PriceTablePageProcessed", typeof(PriceTablePageProcessed) },
            { "CompaniesRequested", typeof(CompaniesRequested) },
            { "InitialSync", typeof(InitialSync) }
        };

        public static Type Resolve(string eventType)
        {
            if (EventTypeMap.TryGetValue(eventType, out var type))
                return type;

            throw new ArgumentException($"Tipo de evento desconhecido: {eventType}");
        }
    }
}

