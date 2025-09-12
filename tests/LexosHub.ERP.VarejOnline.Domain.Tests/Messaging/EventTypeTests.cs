using System.Collections.Generic;
using System.Text.Json;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class EventTypeTests
    {
        public static IEnumerable<object[]> Events => new List<object[]>
        {
            new object[] { new IntegrationCreated() },
            new object[] { new CompaniesRequested() },
            new object[] { new StoresRequested() },
            new object[] { new ProductsRequested() },
            new object[] { new CriarProdutosSimples() },
            new object[] { new CriarProdutosKits() },
            new object[] { new PriceTablesRequested() },
            new object[] { new PriceTablePageProcessed() },
            new object[] { new InvoicesRequested() },
            new object[] { new InitialSync() }
        };

        [Theory]
        [MemberData(nameof(Events))]
        public void Constructor_Should_Set_EventType_To_Class_Name(BaseEvent evt)
        {
            Assert.Equal(evt.GetType().Name, evt.EventType);
        }

        [Theory]
        [MemberData(nameof(Events))]
        public void EventTypeResolver_Should_Deserialize_To_Correct_Type(BaseEvent evt)
        {
            var json = JsonSerializer.Serialize(evt);
            var options = new JsonSerializerOptions();
            options.Converters.Add(new BaseEventJsonConverter());

            var deserialized = JsonSerializer.Deserialize<BaseEvent>(json, options)!;

            Assert.IsType(evt.GetType(), deserialized);
            Assert.Equal(evt.EventType, deserialized.EventType);
        }
    }
}
