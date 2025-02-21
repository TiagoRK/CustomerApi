﻿using CustomerApi.Application.Commands.Customers.Delete;
using Moq;

namespace CustomerApi.UnitTests.Customers;
public class DeleteCustomerByEmailTests : CustomerTestBase
{
  [Test]
  public async Task DeleteCustomer_Success()
  {
    var command = new DeleteCustomerByEmailCommand
    {
      Email = _fakeCustomer.Email
    };

    _customerRepositoryMock
         .Setup(repo => repo.GetByEmail(_fakeCustomer.Email))
         .ReturnsAsync(_fakeCustomer);

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Null);
      Assert.That(result.IsSuccess, Is.True);
    });
  }

  [Test]
  public async Task DeleteCustomer_CustomerNotFound_Failure()
  {
    var command = new DeleteCustomerByEmailCommand
    {
      Email = _fakeCustomer.Email
    };

    var result = await _mediator.Send(command);


    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Not.Null);
      Assert.That(result.IsSuccess, Is.False);
      Assert.That(result.Errors[0].Code, Is.EqualTo("Customer.CustomerWithEmailNotFound"));
      Assert.That(result.Errors[0].Description, Is.EqualTo($"Customer with {_fakeCustomer.Email} was not found."));
    });
  }

  [TestCase(null, "NotNullValidator", "'Email' não pode ser nulo.")]
  [TestCase("invalid-email", "EmailValidator", "'Email' é um endereço de email inválido.")]
  public async Task CreateCustomer_FieldValidations(string email, string expectedErrorCode, string expectedErrorMessage)
  {
    var command = new DeleteCustomerByEmailCommand
    {
      Email = email
    };

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Not.Null);
      Assert.That(result.IsSuccess, Is.False);
      Assert.That(result.Errors.Any(e => e.Code == expectedErrorCode), Is.True, $"Codigo de erro esperado '{expectedErrorCode}'");
      Assert.That(result.Errors.Any(e => e.Description.Contains(expectedErrorMessage)), Is.True, $"Mensagem de erro esperada '{expectedErrorMessage}'");
    });
  }
}
