
using FluentValidation.Results;

namespace CustomerApi.SharedKernel;

public abstract class CommandValidator
{
  public ValidationResult ValidationResult { get; set; } = new ValidationResult();

  public List<Error> ValidationErrors { get; set; } = [];

  public virtual bool IsValid()
  {
    return ValidationResult.IsValid;
  }
}
