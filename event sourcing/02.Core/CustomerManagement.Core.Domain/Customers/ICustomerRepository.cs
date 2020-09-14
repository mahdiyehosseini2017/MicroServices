using System;
using System.Threading.Tasks;

public interface ICustomerRepository
{
    Task<Guid> SaveAsync(Customer customer);
    Task<Customer> GetAsync(Guid id);
}