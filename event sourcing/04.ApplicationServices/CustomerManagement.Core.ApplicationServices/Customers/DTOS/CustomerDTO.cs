public class CustomerDTO
{
    public CustomerDTO(string id, string firstName, string lastName, string street, string city, string country,
                       string zipCode)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Street = street;
        City = city;
        Country = country;
        ZipCode = zipCode;
    }

    public string Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Street { get; }
    public string City { get; }
    public string Country { get; }
    public string ZipCode { get; }

    public static implicit operator CustomerDTO(Customer entity) =>
        new CustomerDTO(entity.Id.ToString(), entity.FirstName, entity.LastName, entity?.Address?.Street,
                        entity?.Address?.City, entity?.Address?.Country, entity?.Address?.ZipCode);

}