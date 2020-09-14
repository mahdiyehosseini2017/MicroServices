using System;
using System.Collections.Generic;
using System.Text;

public class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

    public AggregateRoot(IEnumerable<IDomainEvent> @events)
    {
        if (events == null)
            return;

        foreach (var domainEvent in events)
        {
            Mutate(domainEvent);
            Version++;
        }

    }

    protected AggregateRoot() { }

    public int Version { get; private set; }

    public AggregateRoot(int version, List<IDomainEvent> domainEvents)
    {
        Version = version;
        _domainEvents = domainEvents;
    }

    public void Apply(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
            Apply(@event);

    }

    protected void Apply(IDomainEvent @event)
    {
        Mutate(@event);
        AddDomainEvent(@event);
    }

    protected void AddDomainEvent(IDomainEvent @event) =>
        _domainEvents.Add(@event);

    protected void RemoveDomainEvent(IDomainEvent @event) =>
        _domainEvents.Remove(@event);

    protected void ClearDomainEvent() =>
        _domainEvents.Clear();

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    private void Mutate(IDomainEvent @event)
    {
        var instanceType = this.GetType();
        var eventtype = @event.GetType();

        if (instanceType.GetMethod("On", new Type[] { eventtype }) == null)
            throw new System.Exception("رویداد انتخابی وجود ندارد");

        ((dynamic)this).On((dynamic)@event);

    }



}