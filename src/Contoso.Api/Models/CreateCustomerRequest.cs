// Inbound payload for customer creation. Mirrors the SPA form fields.
namespace Contoso.Api.Models;

public class CreateCustomerRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}
