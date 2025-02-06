using CustomerApi.SharedKernel;
using FluentValidation;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Update;
public class UpdateCustomerCommand : CommandValidator, IRequest<Result<object, Error>>
{
  public string Name { get; set; }
  public DateTime BirthDate { get; set; }
  public string Email { get; set; }

  public override bool IsValid()
  {
    var validator = new InlineValidator<UpdateCustomerCommand>();

    validator.RuleFor(x => x.Name)
        .NotEmpty()
        .NotNull()
        .MaximumLength(255);

    validator.RuleFor(x => x.BirthDate)
        .NotEmpty()
        .NotNull()
        .LessThanOrEqualTo(DateTime.Today)
        .GreaterThan(DateTime.MinValue);

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
