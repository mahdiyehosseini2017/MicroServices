using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Es.Framework;
using Marten;
using Newtonsoft.Json;

public class MartenEventStore : IEventStore
{
    private readonly IDocumentStore _store;
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.All,
        NullValueHandling = NullValueHandling.Ignore
    };

    public MartenEventStore(IDocumentStore store)
    {
        _store = store;
    }
    public Task<IReadOnlyCollection<EventStoreItem>> GetAll(DateTime? afterDateTime)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyCollection<IDomainEvent>> LoadAsync(Guid aggregateRootId, string aggregateName)
    {
        using (var session = _store.OpenSession())
        {
            var eventsList = await session.Events.FetchStreamAsync(aggregateRootId);
            var domainEvents = eventsList.Select(c => TransformEvent(c.Data)).ToList().AsReadOnly();

            return domainEvents;

        };
    }

    public Task SaveAsync(Guid aggregateId, string aggregateName, int originatingVersion, IReadOnlyCollection<IDomainEvent> events)
    {
        using (var session = _store.OpenSession())
        {
            session.Events.Append(aggregateId, originatingVersion + 1,events);
            session.SaveChanges();
        };

        return Task.CompletedTask;
    }

    private IDomainEvent TransformEvent(object eventData)
    {
        var evt = eventData as IDomainEvent;

        return evt;
    }
}