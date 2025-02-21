using CustomerApi.Domain.Constants;
using CustomerApi.Domain.Customers;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Update;
public class UpdateCustomerCommandHandler : BusinessValidator<UpdateCustomerCommand>, IRequestHandler<UpdateCustomerCommand, Result<Customer, Error>>
{
  private readonly ICustomerRepository _customerRepository;

  public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
  {
    _customerRepository = customerRepository;

    AddBusinessRules();
  }

  public async Task<Result<Customer, Error>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
  {
    if (!request.IsValid())
    {
      return request.ValidationErrors;
    }

    var businessErrors = await Validate(request);
    if (businessErrors.Count != 0)
    {
      return businessErrors;
    }

    var customer = await _customerRepository.GetByEmail(request.CurrentEmail);

    customer.UpdateName(request.Name);
    customer.UpdateBirthdate(request.BirthDate);
    customer.UpdateEmail(request.Email);

    await _customerRepository.Update(customer);

    return customer;
  }

  public override void AddBusinessRules()
  {
    AddSyncBusinessRule(
      command => command.BirthDate <= DateTime.Today.AddYears(-BusinessRulesConstants.MINIMUM_AGE),
      CustomerErrors.BirthDateIsNotValid()
    );

    AddAsyncBusinessRule(
      async command => await _customerRepository.GetByEmail(command.CurrentEmail) is not null,
      command => CustomerErrors.CustomerWithEmailNotFound(command.CurrentEmail)
    );

    AddAsyncBusinessRule(
      async command => await _customerRepository.IsEmailUnique(command.Email),
      CustomerErrors.EmailIsNotUnique()
    );
  }
}
