using System;

public class CustomerCreated : IDomainEvent
{
    public CustomerCreated(string id, string firstName, string latsName)
    {
        Id = id;
        FirstName = firstName;
        LatsName = latsName;
    }

    public string Id { get; }
    public string FirstName { get; }
    public string LatsName { get; }

}