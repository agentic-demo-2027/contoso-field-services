// Customer registry endpoints.
//
// DEMO NOTE (scenario 2): this controller is deliberately "fat" - it mixes HTTP concerns,
// validation orchestration, business rules and data access in one place. That makes it the
// natural target for the Clean Architecture refactor scenario.
using Contoso.Api.Data;
using Contoso.Api.Models;
using Contoso.Api.Validation;
using Microsoft.AspNetCore.Mvc;

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
            c.Email,
            c.Region,
            DisplayName = c.FirstName,
            Created = c.CreatedUtc.ToString("yyyy-MM-dd")
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
        var errors = CustomerValidator.Validate(request);
        if (errors.Count > 0)
            return BadRequest(new { errors });

        // Business rule enforced directly in the controller - another refactor seam.
        if (_repository.EmailExists(request.Email))
            return Conflict(new { message = "A customer with that email already exists." });

        // Values are stored exactly as received - no sanitisation (scenario 1 addresses this).
        var customer = new Customer
        {
            FirstName = request.FirstName,
            Email = request.Email,
            Region = request.Region
        };

        var created = _repository.Add(customer);
        _logger.LogInformation("Created customer {CustomerId}", created.Id);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
