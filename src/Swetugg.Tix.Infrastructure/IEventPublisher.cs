using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure
{

    public class PublishedEvents
    {
        public string AggregateId { get; set; }
        public IEnumerable<PublishedEvent> Events { get; set; }
    }

    public class PublishedEvent
    {
        public string AggregateId { get; set; }
        public string EventType { get; set; }
        public int Revision { get; set; }
        public object Body { get; set; }
        public Dictionary<string, object> Headers { get; set; }
    }

    public class PublishedEventConverter : JsonConverter<PublishedEvent>
    {
        class EventData
        {
            public string AggregateId { get; set; }
            public string EventType { get; set; }
            public int Revision { get; set; }
            public JsonElement Body { get; set; }
            public Dictionary<string, object> Headers { get; set; }
        }

        private readonly Assembly _eventAssembly;

        public PublishedEventConverter(Assembly eventAssembly)
        {
            _eventAssembly = eventAssembly;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return base.CanConvert(typeToConvert);
        }

        
        public override PublishedEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var data = JsonSerializer.Deserialize<EventData>(ref reader, options);
            return new PublishedEvent
            {
                AggregateId = data.AggregateId,
                EventType = data.EventType,
                Headers = data.Headers,
                Revision = data.Revision,
                Body = JsonSerializer.Deserialize(data.Body.GetRawText(), _eventAssembly.GetType(data.EventType), options)
            };
        }

        public override void Write(Utf8JsonWriter writer, PublishedEvent value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(value.AggregateId), value.AggregateId);
            writer.WriteString(nameof(value.EventType), value.EventType);
            writer.WriteNumber(nameof(value.Revision), value.Revision);
            writer.WritePropertyName(nameof(value.Headers));
            JsonSerializer.Serialize(writer, value.Headers, options);
            writer.WritePropertyName(nameof(value.Body));
            JsonSerializer.Serialize(writer, value.Body, options);
            writer.WriteEndObject();
        }
    }


    public interface IEventPublisher
    {
        Task Publish(PublishedEvents evts);
    }
}