﻿using CustomerApi.Domain.Constants;
using CustomerApi.Domain.Customers;
using CustomerApi.Domain.Customers.DTO;
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

    var businessErrors = await Validate(request);
    if (businessErrors.Count != 0)
    {
      return businessErrors;
    }

    var newCustomer = new Customer(request.Name, request.BirthDate, request.Email);
    var createdId = await _customerRepository.Create(newCustomer);

    return new GetCustomerResponse()
    {
      Id = createdId,
      Name = request.Name,
      BirthDate = request.BirthDate,
      Email = request.Email,
    };
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
