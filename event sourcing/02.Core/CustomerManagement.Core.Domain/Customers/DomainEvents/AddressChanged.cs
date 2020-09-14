using System;

public class AddressChanged : IDomainEvent
{
    public AddressChanged(string id, string street, string city, string country, string zipCode)
    {
        Id = id;
        Street = street;
        City = city;
        Country = country;
        ZipCode = zipCode;
    }

    public string Id { get; }
    public string Street { get; }
    public string City { get; }
    public string Country { get; }
    public string ZipCode { get; }
}