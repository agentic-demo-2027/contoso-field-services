// In-memory store standing in for the production database.
// Kept deliberately thin so the demo focuses on cross-layer coordination, not persistence.
using Contoso.Api.Models;

namespace Contoso.Api.Data;

public class CustomerRepository
{
    private readonly List<Customer> _customers = new();
    private int _nextId = 1;

    public IReadOnlyList<Customer> GetAll() => _customers;

    public Customer? GetById(int id) => _customers.FirstOrDefault(c => c.Id == id);

    public Customer Add(Customer customer)
    {
        customer.Id = _nextId++;
        customer.CreatedUtc = DateTime.UtcNow;
        _customers.Add(customer);
        return customer;
    }

    public bool EmailExists(string email) =>
        _customers.Any(c => string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase));
}
