using CustomerApi.Domain.Constants;
using CustomerApi.Domain.Customers;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Create;

public class CreateCustomerCommandHandler : BusinessValidator<CreateCustomerCommand>, IRequestHandler<CreateCustomerCommand, Result<object, Error>>
{
  private readonly ICustomerRepository _customerRepository;

  public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
  {
    _customerRepository = customerRepository;

    AddBusinessRules();
  }

  public async Task<Result<object, Error>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
  {
    if (!request.IsValid())
    {
      return request.ValidationErrors;
    }

    using var validationCts = new CancellationTokenSource();
    var businessErrors = await Validate(request, validationCts.Token);
    if (businessErrors.Count != 0)
    {
      return businessErrors;
    }

    var newCustomer = new Customer(request.Name, request.BirthDate, request.Email);
    await _customerRepository.Create(newCustomer);

    return Result<object, Error>.SuccessWithNull();
  }

  public override void AddBusinessRules()
  {
    AddSyncBusinessRule(
      command => command.BirthDate <= DateTime.Today.AddYears(-BusinessRulesConstants.MINIMUM_AGE),
      CustomerErrors.BirthDateIsNotValid()
    );

    AddAsyncBusinessRule(
      async command => await _customerRepository.IsEmailUnique(command.Email),
      CustomerErrors.EmailIsNotUnique()
    );
  }
}
