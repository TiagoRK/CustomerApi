using CustomerApi.Domain.Customers;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Delete;
public class DeleteCustomerByEmailCommandHandler : IRequestHandler<DeleteCustomerByEmailCommand, Result<object, Error>?>
{
  private readonly ICustomerRepository _customerRepository;

  public DeleteCustomerByEmailCommandHandler(ICustomerRepository customerRepository)
  {
    _customerRepository = customerRepository;
  }

  public async Task<Result<object, Error>?> Handle(DeleteCustomerByEmailCommand request, CancellationToken cancellationToken)
  {
    if (!request.IsValid())
    {
      return request.ValidationErrors;
    }

    var customerToDelete = await _customerRepository.GetByEmail(request.Email);

    if (customerToDelete == null)
    {
      return Result<object, Error>.SuccessWithNull();
    }

    await _customerRepository.Delete(customerToDelete!);

    return null;
  }
}
