using System;
using System.Threading.Tasks;

public class CustomerRepository : ICustomerRepository
{
    private readonly IEventStore _eventStore;

    public CustomerRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<Customer> GetAsync(Guid id)
    {
        var events = await _eventStore.LoadAsync(id, typeof(Customer).Name);
        return new Customer(events);
    }

    public async Task<Guid> SaveAsync(Customer customer)
    {
       await _eventStore.SaveAsync(customer.Id, typeof(Customer).Name, customer.Version, customer.DomainEvents);
        return customer.Id;
    }
}