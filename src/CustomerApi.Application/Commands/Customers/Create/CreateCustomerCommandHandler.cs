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

  async Task<Result<object, Error>> IRequestHandler<CreateCustomerCommand, Result<object, Error>>.Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
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
    var id = await _customerRepository.CreateCustomer(newCustomer);

    return new { id };
  }

  private void AddBusinessRules()
  {
    AddSyncBusinessRule(command =>
        command.BirthDate <= DateTime.Today.AddYears(-18),
        CustomerErrors.BirthDateIsNotValid());

    AddAsyncBusinessRule(async command =>
        await _customerRepository.IsEmailUnique(command.Email),
        CustomerErrors.EmailIsNotUnique());
  }
}
