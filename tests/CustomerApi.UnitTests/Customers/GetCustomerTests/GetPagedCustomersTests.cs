using CustomerApi.Application.Queries.Customers.GetPaged;
using CustomerApi.Domain.Customers;
using CustomerApi.SharedKernel;
using Moq;

namespace CustomerApi.UnitTests.Customers.GetCustomerTests;
public class GetPagedCustomerTests : CustomerTestBase
{
  [Test]
  public async Task GetPagedCustomers_Success()
  {
    var query = new GetPagedQuery
    {
      PageSize = 10,
      PageNumber = 1,
    };

    var fakeCustomers = Enumerable.Range(1, 10)
        .Select(_ => new Customer(
            _faker.Name.FullName(),
            _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
            _faker.Internet.Email()))
        .ToList();

    var pagedResponse = new PagedResponse<Customer>(
        fakeCustomers,
        query.PageNumber,
        query.PageSize,
        totalRecords: 100
    );

    _customerRepositoryMock
        .Setup(repo => repo.GetPaginated(query.PageSize, query.PageNumber))
        .ReturnsAsync(pagedResponse);

    var result = await _mediator.Send(query);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Null);
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Value, Is.Not.Null);
      Assert.That(result.Value.PageSize, Is.EqualTo(10));
      Assert.That(result.Value.PageNumber, Is.EqualTo(1));
      Assert.That(result.Value.TotalRecords, Is.EqualTo(100));
      Assert.That(result.Value.TotalPages, Is.EqualTo(10));
      Assert.That(result.Value.Data, Is.Not.Null);
      Assert.That(result.Value.Data.Count, Is.EqualTo(10));
    });
  }

  [Test]
  public async Task GetPagedCustomers_NoCustomersFound_ReturnsEmptyList()
  {
    var query = new GetPagedQuery { PageSize = 10, PageNumber = 1 };

    var pagedResponse = new PagedResponse<Customer>(new List<Customer>(), query.PageNumber, query.PageSize, totalRecords: 0);

    _customerRepositoryMock
        .Setup(repo => repo.GetPaginated(query.PageSize, query.PageNumber))
        .ReturnsAsync(pagedResponse);

    var result = await _mediator.Send(query);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Null);
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Value, Is.Not.Null);
      Assert.That(result.Value.Data, Is.Empty);
      Assert.That(result.Value.TotalRecords, Is.EqualTo(0));
      Assert.That(result.Value.TotalPages, Is.EqualTo(0));
    });
  }

  [Test]
  public async Task GetPagedCustomers_PageOutOfRange_ReturnsEmptyList()
  {
    var query = new GetPagedQuery { PageSize = 10, PageNumber = 100 };

    var pagedResponse = new PagedResponse<Customer>(new List<Customer>(), query.PageNumber, query.PageSize, totalRecords: 50);

    _customerRepositoryMock
        .Setup(repo => repo.GetPaginated(query.PageSize, query.PageNumber))
        .ReturnsAsync(pagedResponse);

    var result = await _mediator.Send(query);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Null);
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Value, Is.Not.Null);
      Assert.That(result.Value.Data, Is.Empty);
    });
  }

  [Test]
  public async Task GetPagedCustomers_LessThanPageSize_ReturnsPartialList()
  {
    var query = new GetPagedQuery { PageSize = 10, PageNumber = 1 };

    var fakeCustomers = Enumerable.Range(1, 5)
        .Select(_ => new Customer(
            _faker.Name.FullName(),
            _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
            _faker.Internet.Email()))
        .ToList();

    var pagedResponse = new PagedResponse<Customer>(fakeCustomers, query.PageNumber, query.PageSize, totalRecords: 5);

    _customerRepositoryMock
        .Setup(repo => repo.GetPaginated(query.PageSize, query.PageNumber))
        .ReturnsAsync(pagedResponse);

    var result = await _mediator.Send(query);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Null);
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Value, Is.Not.Null);
      Assert.That(result.Value.Data.Count, Is.EqualTo(5));
      Assert.That(result.Value.TotalRecords, Is.EqualTo(5));
      Assert.That(result.Value.TotalPages, Is.EqualTo(1));
    });
  }
}
