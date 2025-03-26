using CustomerApi.Application.Queries.Customers.GetByEmail;
using CustomerApi.Domain.Customers;
using Moq;

namespace CustomerApi.UnitTests.Customers.GetCustomerTests;
public class GetCustomerByEmailTests : CustomerTestBase
{
  [Test]
  public async Task GetCustomerByEmail_Success()
  {
    var query = new GetByEmailQuery
    {
      Email = "new_email@email.com"
    };

    _fakeCustomer.Email = query.Email;

    _customerRepositoryMock
         .Setup(repo => repo.GetByEmail(query.Email, false))
         .ReturnsAsync(_fakeCustomer);

    var result = await _mediator.Send(query);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Null);
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Value.Name, Is.EqualTo(_fakeCustomer.Name));
      Assert.That(result.Value.BirthDate, Is.EqualTo(_fakeCustomer.BirthDate));
      Assert.That(result.Value.Email, Is.EqualTo(query.Email));
    });
  }

  [Test]
  public async Task GetCustomerByEmail_NotFound()
  {
    var query = new GetByEmailQuery
    {
      Email = "emailtest"
    };

    _fakeCustomer.Email = query.Email;

    _customerRepositoryMock
        .Setup(repo => repo.GetByEmail(query.Email, false))
        .ReturnsAsync((Customer?)null);

    var result = await _mediator.Send(query);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Null);
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Value, Is.Null);
    });
  }
}
