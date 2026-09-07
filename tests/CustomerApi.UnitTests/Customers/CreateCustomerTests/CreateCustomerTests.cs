using System.Globalization;
using CustomerApi.Application.Commands.Customers.Create;
using Moq;

namespace CustomerApi.UnitTests.Customers.CreateCustomerTests;
public class CreateCustomerTests : CustomerTestBase
{
  [Test]
  public async Task CreateCustomer_Success()
  {
    var command = new CreateCustomerCommand
    {
      Name = _fakeCustomer.Name,
      BirthDate = _fakeCustomer.BirthDate,
      Email = _fakeCustomer.Email
    };

    _customerRepositoryMock
         .Setup(repo => repo.Create(_fakeCustomer))
         .ReturnsAsync(1);

    _customerRepositoryMock
         .Setup(repo => repo.IsEmailUnique(_fakeCustomer.Email))
         .ReturnsAsync(true);

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Null);
      Assert.That(result.IsSuccess, Is.True);
    });
  }

  [Test]
  public async Task CreateCustomer_IsNotOfLegalAge_Failure()
  {
    CultureInfo.CurrentUICulture = new CultureInfo("en");

    var command = new CreateCustomerCommand
    {
      Name = _fakeCustomer.Name,
      BirthDate = DateTime.Now.AddYears(-1),
      Email = _fakeCustomer.Email
    };

    _customerRepositoryMock
         .Setup(repo => repo.Create(_fakeCustomer))
         .ReturnsAsync(1);

    _customerRepositoryMock
         .Setup(repo => repo.IsEmailUnique(_fakeCustomer.Email))
         .ReturnsAsync(true);

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Not.Null);
      Assert.That(result.IsSuccess, Is.False);
      Assert.That(result.Errors[0].Code, Is.EqualTo("Customer.BirthDateIsNotValid"));
      Assert.That(result.Errors[0].Description, Is.EqualTo("Customer must be at least 18 years old."));
    });
  }

  [Test]
  public async Task CreateCustomer_EmailIsNotUnique_Failure()
  {
    CultureInfo.CurrentUICulture = new CultureInfo("en");

    var command = new CreateCustomerCommand
    {
      Name = _fakeCustomer.Name,
      BirthDate = _fakeCustomer.BirthDate,
      Email = _fakeCustomer.Email
    };

    _customerRepositoryMock
         .Setup(repo => repo.Create(_fakeCustomer))
         .ReturnsAsync(1);

    _customerRepositoryMock
         .Setup(repo => repo.IsEmailUnique(_fakeCustomer.Email))
         .ReturnsAsync(false);

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Not.Null);
      Assert.That(result.IsSuccess, Is.False);
      Assert.That(result.Errors[0].Code, Is.EqualTo("Customer.EmailIsNotUnique"));
      Assert.That(result.Errors[0].Description, Is.EqualTo("The provided email is not unique."));
    });
  }

  [Test]
  public async Task CreateCustomer_IsNotOfLegalAge_Failure_PortugueseCulture()
  {
    CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");

    var command = new CreateCustomerCommand
    {
      Name = _fakeCustomer.Name,
      BirthDate = DateTime.Now.AddYears(-1),
      Email = _fakeCustomer.Email
    };

    _customerRepositoryMock
         .Setup(repo => repo.Create(_fakeCustomer))
         .ReturnsAsync(1);

    _customerRepositoryMock
         .Setup(repo => repo.IsEmailUnique(_fakeCustomer.Email))
         .ReturnsAsync(true);

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Not.Null);
      Assert.That(result.IsSuccess, Is.False);
      Assert.That(result.Errors[0].Code, Is.EqualTo("Customer.BirthDateIsNotValid"));
      Assert.That(result.Errors[0].Description, Is.EqualTo("O cliente deve ter pelo menos 18 anos."));
    });
  }

  [Test]
  public async Task CreateCustomer_EmailIsNotUnique_Failure_PortugueseCulture()
  {
    CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");

    var command = new CreateCustomerCommand
    {
      Name = _fakeCustomer.Name,
      BirthDate = _fakeCustomer.BirthDate,
      Email = _fakeCustomer.Email
    };

    _customerRepositoryMock
         .Setup(repo => repo.Create(_fakeCustomer))
         .ReturnsAsync(1);

    _customerRepositoryMock
         .Setup(repo => repo.IsEmailUnique(_fakeCustomer.Email))
         .ReturnsAsync(false);

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Not.Null);
      Assert.That(result.IsSuccess, Is.False);
      Assert.That(result.Errors[0].Code, Is.EqualTo("Customer.EmailIsNotUnique"));
      Assert.That(result.Errors[0].Description, Is.EqualTo("O e-mail fornecido não é único."));
    });
  }

  [TestCase("", "2000-01-01", "test@example.com", "NotEmptyValidator", "'Name' must not be empty.")]
  public async Task CreateCustomer_FieldValidations(string name, string birthDate, string email, string expectedErrorCode, string expectedErrorMessage)
  {
    CultureInfo.CurrentUICulture = new CultureInfo("en");

    var command = new CreateCustomerCommand
    {
      Name = name,
      BirthDate = string.IsNullOrEmpty(birthDate) ? default : DateTime.Parse(birthDate),
      Email = email
    };

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Not.Null);
      Assert.That(result.IsSuccess, Is.False);
      Assert.That(result.Errors!.Any(e => e.Code == expectedErrorCode), Is.True, $"Codigo de erro esperado '{expectedErrorCode}'");
      Assert.That(result.Errors!.Any(e => e.Description.Contains(expectedErrorMessage)), Is.True, $"Mensagem de erro esperada '{expectedErrorMessage}'");
    });
  }
}
