using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Es.Framework;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;

public class SqlEventStore : IEventStore
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.All,
        NullValueHandling = NullValueHandling.Ignore
    };

    private readonly string sqlConnectionString;
    public SqlEventStore(IConfiguration configuration)
    {
        sqlConnectionString = configuration.GetConnectionString("EventStoreDb");
    }

    public async Task<IReadOnlyCollection<EventStoreItem>> GetAll(DateTime? afterDateTime)
    {
        string where = afterDateTime.HasValue ? $"WHERE CreatedAt >  '{afterDateTime}' " : "";

        var query = $"SELECT * FROM EventStore {where} ORDER BY CreatedAt,[Version] ASC";

        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            var events = (await sqlConnection.QueryAsync<EventStoreItem>(query.ToString())).ToList();
            return events;
        }
    }

    public async Task<IReadOnlyCollection<IDomainEvent>> LoadAsync(Guid aggregateRootId, string aggregateName)
    {
        if (aggregateRootId == null) throw new InvalidOperationException("AggregateRootId cannot be null");
        if (string.IsNullOrWhiteSpace(aggregateName)) throw new InvalidOperationException("AggregateName cannot be null");

        var query = @"SELECT * FROM EventStore WHERE [AggregateId] = @AggregateId 
                    and [Aggregate] = @Aggregate ORDER BY [Version] ASC";

        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            var events = (await sqlConnection.QueryAsync<EventStoreItem>(query.ToString(),
                new { AggregateId = aggregateRootId, Aggregate = aggregateName })).ToList();
            var domainEvents = events.Select(c => DeserializeData(c.Data)).Where(x => x != null).ToList().AsReadOnly();
            return domainEvents;
        }
    }

    public async Task SaveAsync(Guid aggregateId, string aggregateName, int originatingVersion, IReadOnlyCollection<IDomainEvent> events)
    {
        if (events.Count() < 1)
            return;

        var query = @"INSERT INTO [dbo].[EventStore]
           ([Id],[CreatedAt],[Version],[Name],[AggregateId],[Data],[Aggregate])
           VALUES (@Id, @CreatedAt, @Version, @Name, @AggregateId, @Data, @Aggregate)";

        var insertEvents = events.Select(c => new
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Version = originatingVersion + 1,
            Name = c.GetType().Name,
            AggregateId = aggregateId,
            Data = SerializeData(c),
            Aggregate = aggregateName,
        });

        using (var sqlConnection = new SqlConnection(sqlConnectionString))
            await sqlConnection.ExecuteAsync(query, insertEvents);
    }

    private IDomainEvent DeserializeData(string data)
    {
        var o = JsonConvert.DeserializeObject(data, _jsonSerializerSettings);
        var @event = o as IDomainEvent;
        return @event;
    }

    private string SerializeData(IDomainEvent @event)
    {
        var data = JsonConvert.SerializeObject(@event, Formatting.Indented, _jsonSerializerSettings);
        return data;
    }
}