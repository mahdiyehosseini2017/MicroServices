using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Es.Framework;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace EventStore.ES
{
    public class ESEventStore : IEventStore
    {
        private readonly IEventStoreConnection _connection;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore
        };

        public ESEventStore(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        public Task<IReadOnlyCollection<EventStoreItem>> GetAll(DateTime? afterDateTime)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<IDomainEvent>> LoadAsync(Guid aggregateRootId, string aggregateName)
        {
            string stream = GetStreamName(aggregateRootId, aggregateName);

            var streamEvents = await _connection.ReadStreamEventsForwardAsync(stream, 0, 1000, false);
            var domainEvents = streamEvents.Events.Select(c => TransformEvent(c.Event.Data)).ToList();
            return domainEvents;
        }

        public async Task SaveAsync(Guid aggregateId, string aggregateName, int originatingVersion, IReadOnlyCollection<IDomainEvent> events)
        {
            string stream = GetStreamName(aggregateId, aggregateName);

            var eventData = events.Select(ev => new EventData(
                eventId: Guid.NewGuid(),
                type: ev.GetType().ToString(),
                isJson: false,
                data: SerializeData(ev),
                metadata: SerializeData(new EventMetaData 
                { CreatedAt = DateTime.Now, Version = ExpectedVersion.Any, ClrType = ev.GetType().AssemblyQualifiedName })
            ));

            await _connection.AppendToStreamAsync(stream, ++originatingVersion, eventData);
        }

        private static string GetStreamName(Guid aggregateId, string aggregateName) =>
            $"{aggregateName}-{aggregateId.ToString()}";

        private IDomainEvent TransformEvent(byte[] eventData)
        {
            string jsonData = Encoding.UTF8.GetString(eventData);
            return DeserializeData(jsonData);
        }

        private IDomainEvent DeserializeData(string data)
        {
            var o = JsonConvert.DeserializeObject(data, _jsonSerializerSettings);
            var @event = o as IDomainEvent;
            return @event;
        }

        private byte[] SerializeData(object obj)
        {
            var data = JsonConvert.SerializeObject(obj, Formatting.Indented, _jsonSerializerSettings);
            return Encoding.UTF8.GetBytes(data);
        }

    }
}

