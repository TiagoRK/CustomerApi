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

    var businessErrors = await Validate(request);
    if (businessErrors.Count != 0)
    {
      return businessErrors;
    }

    var customerToDelete = await _customerRepository.GetByEmail(request.Email);

    await _customerRepository.Delete(customerToDelete!);

    //Ver se tem como realizar a validação e usar o dado que já pegou no banco

    return Result<object, Error>.SuccessWithNull();
  }

  public override void AddBusinessRules()
  {
    AddAsyncBusinessRule(
      async command => await _customerRepository.GetByEmail(command.Email) is not null,
      command => CustomerErrors.CustomerWithEmailNotFound(command.Email)
    );
  }
}
