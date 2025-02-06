using CustomerApi.Domain.Constants;
using CustomerApi.Domain.Customers;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Update;
public class UpdateCustomerCommandHandler : BusinessValidator<UpdateCustomerCommand>, IRequestHandler<UpdateCustomerCommand, Result<object, Error>>
{
  private readonly ICustomerRepository _customerRepository;

  public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
  {
    _customerRepository = customerRepository;

    AddBusinessRules();
  }

  public async Task<Result<object, Error>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
  {
    var customer = await _customerRepository.GetByEmail(request.Email);

    if (customer == null)
    {
      return CustomerErrors.CustomerWithEmailNotFound(request.Email);
    }

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
      async command => await _customerRepository.GetByEmail(command.Email) is not null,
      command => CustomerErrors.CustomerWithEmailNotFound(command.Email)
    );

    AddAsyncBusinessRule(
      async command => await _customerRepository.IsEmailUnique(command.Email),
      CustomerErrors.EmailIsNotUnique()
    );
  }
}
