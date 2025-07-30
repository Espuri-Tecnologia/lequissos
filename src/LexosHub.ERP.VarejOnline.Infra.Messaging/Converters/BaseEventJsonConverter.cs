using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Converters
{
    public class BaseEventJsonConverter : JsonConverter<BaseEvent>
    {
        public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);

            if (!document.RootElement.TryGetProperty("EventType", out var eventTypeProperty))
            {
                throw new JsonException("EventType property not found");
            }

            var eventTypeName = eventTypeProperty.GetString();
            if (string.IsNullOrEmpty(eventTypeName))
            {
                throw new JsonException("EventType property is null or empty");
            }

            var actualType = EventTypeResolver.Resolve(eventTypeName);
            var json = document.RootElement.GetRawText();
            return (BaseEvent?)JsonSerializer.Deserialize(json, actualType, options);
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
