using System.Collections.Generic;
using System;

public class Customer : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Address Address { get; private set; }

    public Customer(IEnumerable<IDomainEvent> events) : base(events)
    {
    }

    protected Customer() { }

    public static Customer Create(string firstName, string lastName)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            throw new Exception("وارد کردن نام و نام خانوادگی الزامی است");

        Customer entity = new Customer();
        entity.Apply(new CustomerCreated(Guid.NewGuid().ToString(), firstName, lastName));
        return entity;
    }

    public void ChangeAddress(string street, string city, string country, string zipCode)
    {
        Apply(new AddressChanged(Id.ToString(), street, city, country, zipCode));
    }

    public void ChangeName(string firstName, string lastName)
    {
        Apply(new CustomerNameChanged(Id.ToString(), firstName, lastName));
    }

    public void On(CustomerCreated @event)
    {
        Id = Guid.Parse(@event.Id);
        FirstName = @event.FirstName;
        LastName = @event.LatsName;
    }

    public void On(CustomerNameChanged @event)
    {
        FirstName = @event.FirstName;
        LastName = @event.LastName;
    }

    public void On(AddressChanged @event)
    {
        Address = new Address()
        {
            City = @event.City,
            Country = @event.Country,
            Street = @event.Street,
            ZipCode = @event.ZipCode
        };
    }


}