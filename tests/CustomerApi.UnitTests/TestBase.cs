using System.Reflection;
using Bogus;
using CustomerApi.Application.Behaviors;
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
  private readonly List<Mock> _mocks = [];

  [OneTimeSetUp]
  public void OneTimeSetUp()
  {
    _faker = new Faker("pt_BR");

    _services = new ServiceCollection();
    AddRepositoriesToContainer();
    AddMediator();
  }

  [SetUp]
  public void Setup()
  {
    ResetMocks();
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
    var mediatRAssemblies = new[]
    {
        Assembly.GetAssembly(typeof(Customer)),
        Assembly.GetAssembly(typeof(CreateCustomerCommand))
    };

    _services.AddMediatR(cfg =>
    {
      cfg.RegisterServicesFromAssemblies(mediatRAssemblies!);
      cfg.AddOpenBehavior(typeof(CommandValidationPipelineBehavior<,>));
    });

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
