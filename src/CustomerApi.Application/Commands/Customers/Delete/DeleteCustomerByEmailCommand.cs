using CustomerApi.SharedKernel;
using FluentValidation;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Delete;
public class DeleteCustomerByEmailCommand : CommandValidator, IRequest<Result<object, Error>>
{
  public string Email { get; set; }

  public override bool IsValid()
  {
    var validator = new InlineValidator<DeleteCustomerByEmailCommand>();

    validator.RuleFor(x => x.Email)
        .NotEmpty()
        .NotNull()
        .EmailAddress();

    ValidationResult = validator.Validate(this);

    if (!ValidationResult.IsValid)
    {
      ValidationErrors = ValidationResult.Errors
      .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
      .ToList();
    }

    return ValidationResult.IsValid;
  }
}
