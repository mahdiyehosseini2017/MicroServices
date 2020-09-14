using System;
using System.Threading.Tasks;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Guid> CreateAsync(string firstName, string lastName)
    {
        Customer entity = Customer.Create(firstName, lastName);
        var id = await _customerRepository.SaveAsync(entity);
        return id;
    }

    public async Task<Guid> UpdateAsync(string id, string firstName, string lastName)
    {
        Customer entity = await _customerRepository.GetAsync(Guid.Parse(id));
        entity.ChangeName(firstName, lastName);
        await _customerRepository.SaveAsync(entity);
       
        return entity.Id;
    }

    public async Task<CustomerDTO> GetAsync(string id)
    {
        Customer entity = await _customerRepository.GetAsync(Guid.Parse(id));
       
        return entity;
    }

    public async Task<Guid> UpdateAsync(string id, string street, string city, string country, string zipCode)
    {
        Customer entity = await _customerRepository.GetAsync(Guid.Parse(id));
        entity.ChangeAddress(street, city, country, zipCode);
        await _customerRepository.SaveAsync(entity);
       
        return entity.Id;
    }
}