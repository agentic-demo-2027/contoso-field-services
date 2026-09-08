// Field-level validation for inbound customer payloads.
using Contoso.Api.Models;

namespace Contoso.Api.Validation;

public static class CustomerValidator
{
    public static List<string> Validate(CreateCustomerRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors.Add("First name is required.");
        else if (request.FirstName.Length > 50)
            errors.Add("First name must be 50 characters or fewer.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            errors.Add("Last name is required.");
        else if (request.LastName.Length > 50)
            errors.Add("Last name must be 50 characters or fewer.");

        if (string.IsNullOrWhiteSpace(request.Email))
            errors.Add("Email is required.");
        else if (!request.Email.Contains('@'))
            errors.Add("Email must be a valid address.");

        if (string.IsNullOrWhiteSpace(request.Region))
            errors.Add("Region is required.");

        return errors;
    }
}
