using CustomerApi.Domain.Customers;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Create
{
    public class CreateCustomerCommandHandler : BusinessValidator<CreateCustomerCommand>, IRequestHandler<CreateCustomerCommand, Result<long, Error>>
    {
        private readonly ICustomerRepository CustomerRepository;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            CustomerRepository = customerRepository;

            AddSyncBusinessRule(command =>
                command.BirthDate <= DateTime.Today.AddYears(-18),
                CustomerErrors.BirthDateIsNotValid());

            AddAsyncBusinessRule(async command =>
                await CustomerRepository.IsEmailUnique(command.Email),
                CustomerErrors.EmailIsNotUnique());
        }

        async Task<Result<long, Error>> IRequestHandler<CreateCustomerCommand, Result<long, Error>>.Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
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

            var newCustomer = new Customer(request.Name, request.BirthDate, request.Email);
            var id = await CustomerRepository.CreateCustomer(newCustomer);

            return id;
        }

    }
}
