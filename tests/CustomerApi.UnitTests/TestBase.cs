using Bogus;
using CustomerApi.Application.Commands.Customers.Create;
using CustomerApi.Domain.Customers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CustomerApi.UnitTests;

public abstract class TestBase
{
  protected IMediator _mediator;
  protected Faker _faker;
  protected Mock<ICustomerRepository> _customerRepositoryMock;
  protected IServiceProvider _serviceProvider;
  private ServiceCollection _services;
  private readonly List<Mock> _mocks = new();

  [SetUp]
  public void Setup()
  {
    _faker = new Faker("pt_BR");

    ResetMocks();

    _services = new ServiceCollection();
    AddRepositoriesToContainer();
    AddMediator();
  }

  private void ResetMocks()
  {
    foreach (var mock in _mocks)
    {
      mock.Reset();
    }
  }

  private void AddMediator()
  {
    _services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateCustomerCommandHandler>());
    _serviceProvider = _services.BuildServiceProvider();
    _mediator = _serviceProvider.GetRequiredService<IMediator>();
  }

  private void AddRepositoriesToContainer()
  {
    _customerRepositoryMock = new Mock<ICustomerRepository>();
    _mocks.Add(_customerRepositoryMock);

    _services.AddSingleton(_customerRepositoryMock.Object);
  }
}
