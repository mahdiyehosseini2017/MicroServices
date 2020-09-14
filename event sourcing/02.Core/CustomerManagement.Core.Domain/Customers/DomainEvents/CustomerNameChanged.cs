using System;

public class CustomerNameChanged : IDomainEvent
{
    public CustomerNameChanged(string id,string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Id {get;}
    public string FirstName { get; }
    public string LastName { get; }
}