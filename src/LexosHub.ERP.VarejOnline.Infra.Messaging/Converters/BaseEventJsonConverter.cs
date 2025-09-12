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
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            // tenta EventType e eventType
            if (!TryGetProp(root, "EventType", out var p) && !TryGetProp(root, "eventType", out p))
            {
                // fallback seguro: tenta desserializar como BaseEvent (mantém EventType via ctor)
                return root.Deserialize<BaseEvent>(options);
            }

            var eventTypeName = p.GetString();
            if (string.IsNullOrWhiteSpace(eventTypeName))
                return root.Deserialize<BaseEvent>(options);

            if (!EventTypeResolver.TryResolve(eventTypeName!, out var actualType))
                return root.Deserialize<BaseEvent>(options);

            // evite alocação de string extra usando Deserialize direto do JsonElement
            return (BaseEvent?)root.Deserialize(actualType, options);
        }

        private static bool TryGetProp(JsonElement root, string name, out JsonElement value)
        {
            foreach (var prop in root.EnumerateObject())
            {
                if (string.Equals(prop.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = prop.Value;
                    return true;
                }
            }
            value = default;
            return false;
        }
        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
