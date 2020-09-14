using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("customers")]
public class CustomerController : Controller
{
    private readonly CustomerService _customerService;

    public CustomerController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<object> CreateCustomer([FromBody] SaveCustomerDto dto)
    {
        Guid insertetId = await _customerService.CreateAsync(dto.FirstName, dto.LastName);
        return new { customerId = insertetId.ToString() };
    }

    [HttpPut("{id}/update-name")]
    public async Task UpdateCustomer(string id, [FromBody] SaveCustomerDto dto)
    {
        await _customerService.UpdateAsync(id, dto.FirstName, dto.LastName);
        Ok();
    }

    [HttpPut("{id}/update-address")]
    public async Task UpdateCustomerAddress(string id, [FromBody] AddressDTO dto)
    {
        await _customerService.UpdateAsync(id, dto.Street, dto.City, dto.Country, dto.ZipCode);
        Ok();
    }

    [HttpGet("{id}")]
    public async Task<object> Get(string id)
    {
        var entity = await _customerService.GetAsync(id);
        return new { customer = entity };
    }

}