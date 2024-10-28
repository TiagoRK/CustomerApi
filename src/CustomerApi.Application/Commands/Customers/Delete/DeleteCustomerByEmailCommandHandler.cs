using CustomerApi.Domain.Customers;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Delete;
public class DeleteCustomerByEmailCommandHandler : BusinessValidator<DeleteCustomerByEmailCommand>, IRequestHandler<DeleteCustomerByEmailCommand, Result<object, Error>>
{
  private readonly ICustomerRepository _customerRepository;

  public DeleteCustomerByEmailCommandHandler(ICustomerRepository customerRepository)
  {
    _customerRepository = customerRepository;

    AddBusinessRules();
  }

  public async Task<Result<object, Error>> Handle(DeleteCustomerByEmailCommand request, CancellationToken cancellationToken)
  {
    if (!request.IsValid())
    {
      return request.ValidationErrors;
    }

    var (errors, values) = await Validate(request, cancellationToken);
    if (errors.Count != 0)
    {
      return errors;
    }

    var customerToDelete = values[nameof(Customer)] as Customer;
    await _customerRepository.Delete(customerToDelete!);

    return Result<object, Error>.SuccessWithNull();
  }

  private void AddBusinessRules()
  {
    AddValueReturningAsyncBusinessRule(
        nameof(Customer),
        async command =>
        {
          var customer = await _customerRepository.GetByEmail(command.Email);
          return (customer != null, customer!);
        },
        command => CustomerErrors.CustomerWithEmailNotFound(command.Email)
    );
  }
}
