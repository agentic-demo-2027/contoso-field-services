// Customer record as stored by the field services registry.
// NOTE: deliberately has no LastName - demo scenario 1 adds it across the whole stack.
namespace Contoso.Api.Models;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
}
