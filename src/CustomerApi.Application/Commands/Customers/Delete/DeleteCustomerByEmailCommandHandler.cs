using CustomerApi.Domain.Customers;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Delete;
public class DeleteCustomerByEmailCommandHandler(ICustomerRepository customerRepository) : BusinessValidator<DeleteCustomerByEmailCommand>, IRequestHandler<DeleteCustomerByEmailCommand, Result<object, Error>>
{
  private readonly ICustomerRepository _customerRepository = customerRepository;

  public async Task<Result<object, Error>> Handle(DeleteCustomerByEmailCommand request, CancellationToken cancellationToken)
  {
    var customerToDelete = await _customerRepository.GetByEmail(request.Email);

    await _customerRepository.Delete(customerToDelete!);

    //Ver se tem como realizar a validação e usar o dado que já pegou no banco

    throw new NotImplementedException();
  }

  public override void AddBusinessRules()
  {
    AddAsyncBusinessRule(
      async command => await _customerRepository.GetByEmail(command.Email) is not null,
      command => CustomerErrors.CustomerWithEmailNotFound(command.Email)
    );
  }
}
