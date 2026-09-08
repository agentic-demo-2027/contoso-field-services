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
        var payload = new { firstName = "Ada", email = $"ada{Guid.NewGuid():N}@contoso.com", region = "EMEA" };

        var response = await client.PostAsJsonAsync("/api/customers", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_customer_rejects_missing_first_name()
    {
        var client = _factory.CreateClient();
        var payload = new { firstName = "", email = "nobody@contoso.com", region = "EMEA" };

        var response = await client.PostAsJsonAsync("/api/customers", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
