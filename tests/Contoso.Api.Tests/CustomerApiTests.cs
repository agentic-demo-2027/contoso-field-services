// End-to-end tests over the customer endpoints.
// These are what an agent runs to prove a cross-stack change actually works.
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Contoso.Api.Tests;

public class CustomerApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CustomerApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Health_returns_ok()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_customer_succeeds_with_valid_payload()
    {
        var client = _factory.CreateClient();
        var payload = new { firstName = "Ada", lastName = "Lovelace", email = $"ada{Guid.NewGuid():N}@contoso.com", region = "EMEA" };

        var response = await client.PostAsJsonAsync("/api/customers", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_customer_rejects_missing_first_name()
    {
        var client = _factory.CreateClient();
        var payload = new { firstName = "", lastName = "Customer", email = "nobody@contoso.com", region = "EMEA" };

        var response = await client.PostAsJsonAsync("/api/customers", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_customer_rejects_missing_last_name()
    {
        var client = _factory.CreateClient();
        var payload = new { firstName = "Ada", lastName = "", email = $"missing-last{Guid.NewGuid():N}@contoso.com", region = "EMEA" };

        var response = await client.PostAsJsonAsync("/api/customers", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_customer_sanitizes_name_fields_before_storage()
    {
        var client = _factory.CreateClient();
        var payload = new
        {
            firstName = "<script>alert(1)</script>Ada\t",
            lastName = "\u0001<em>Lovelace</em>",
            email = $"sanitized{Guid.NewGuid():N}@contoso.com",
            region = "EMEA"
        };

        var response = await client.PostAsJsonAsync("/api/customers", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var customer = await response.Content.ReadFromJsonAsync<Customer>();
        Assert.NotNull(customer);
        Assert.Equal("alert(1)Ada", customer!.FirstName);
        Assert.Equal("Lovelace", customer.LastName);
    }

    private sealed class Customer
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
