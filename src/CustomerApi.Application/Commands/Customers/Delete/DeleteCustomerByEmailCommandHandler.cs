using CustomerApi.Domain.Customers;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Delete;
public class DeleteCustomerByEmailCommandHandler(ICustomerRepository customerRepository) : BusinessValidator<DeleteCustomerByEmailCommand>, IRequestHandler<DeleteCustomerByEmailCommand, Result<object, Error>
{
  private readonly ICustomerRepository _customerRepository = customerRepository;

  public async Task<Result<object, Error>> Handle(DeleteCustomerByEmailCommand request, CancellationToken cancellationToken)
  {
    var customerToDelete = await _customerRepository.GetByEmail(request.Email);

    if (customerToDelete == null)
    {
      return CustomerErrors.CustomerWithEmailNotFound(request.Email);
    }

    await _customerRepository.Delete(customerToDelete);

    throw new NotImplementedException();
  }
}
