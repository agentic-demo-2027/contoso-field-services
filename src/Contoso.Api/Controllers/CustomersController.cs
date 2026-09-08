// Customer registry endpoints.
//
// DEMO NOTE (scenario 2): this controller is deliberately "fat" - it mixes HTTP concerns,
// validation orchestration, business rules and data access in one place. That makes it the
// natural target for the Clean Architecture refactor scenario.
using Contoso.Api.Data;
using Contoso.Api.Models;
using Contoso.Api.Validation;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Contoso.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly CustomerRepository _repository;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(CustomerRepository repository, ILogger<CustomersController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<object>> GetAll()
    {
        // Response shaping done inline rather than through a mapper - part of the refactor target.
        var result = _repository.GetAll().Select(c => new
        {
            c.Id,
            c.FirstName,
            c.LastName,
            c.Email,
            c.Region,
            DisplayName = $"{c.FirstName} {c.LastName}",
            c.CreatedUtc
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Customer> GetById(int id)
    {
        var customer = _repository.GetById(id);
        if (customer is null)
            return NotFound(new { message = $"Customer {id} not found." });

        return Ok(customer);
    }

    [HttpPost]
    public ActionResult<Customer> Create([FromBody] CreateCustomerRequest request)
    {
        var sanitizedRequest = new CreateCustomerRequest
        {
            FirstName = SanitizeName(request.FirstName),
            LastName = SanitizeName(request.LastName),
            Email = request.Email,
            Region = request.Region
        };

        var errors = CustomerValidator.Validate(sanitizedRequest);
        if (errors.Count > 0)
            return BadRequest(new { errors });

        // Business rule enforced directly in the controller - another refactor seam.
        if (_repository.EmailExists(sanitizedRequest.Email))
            return Conflict(new { message = "A customer with that email already exists." });

        var customer = new Customer
        {
            FirstName = sanitizedRequest.FirstName,
            LastName = sanitizedRequest.LastName,
            Email = sanitizedRequest.Email,
            Region = sanitizedRequest.Region
        };

        var created = _repository.Add(customer);
        _logger.LogInformation("Created customer {CustomerId}", created.Id);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    private static string SanitizeName(string value)
    {
        var withoutMarkup = Regex.Replace(value, "<.*?>", string.Empty);
        return new string(withoutMarkup.Where(c => !char.IsControl(c)).ToArray()).Trim();
    }
}
