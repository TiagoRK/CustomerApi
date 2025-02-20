using CustomerApi.Application.Commands.Customers.Create;
using CustomerApi.Domain.Customers;
using Moq;

namespace CustomerApi.UnitTests.Customers;
public class CreateCustomerTests : CustomerTestBase
{
  [Test]
  public async Task CreateCustomer_Sucess()
  {
    var command = new CreateCustomerCommand
    {
      Name = _fakeCustomer.Name,
      BirthDate = _fakeCustomer.BirthDate,
      Email = _fakeCustomer.Email
    };

    _customerRepositoryMock
         .Setup(repo => repo.Create(It.IsAny<Customer>()))
         .ReturnsAsync(1);

    _customerRepositoryMock
         .Setup(repo => repo.IsEmailUnique(It.IsAny<string>()))
         .ReturnsAsync(true);

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Null);
      Assert.That(result.IsSuccess, Is.True);
    });
  }
}
