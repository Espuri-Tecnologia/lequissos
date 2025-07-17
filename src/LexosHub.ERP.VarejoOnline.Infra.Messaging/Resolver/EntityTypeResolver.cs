using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using System;
using System.Collections.Generic;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher
{
    public static class EventTypeResolver
    {
        private static readonly Dictionary<string, Type> EventTypeMap = new()
        {
            { "IntegrationCreated", typeof(IntegrationCreated) },
            { "ProductsRequested", typeof(ProductsRequested) },
            { "ProductsPageProcessed", typeof(ProductsPageProcessed) },
            { "PriceTablesRequested", typeof(PriceTablesRequested) },
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

