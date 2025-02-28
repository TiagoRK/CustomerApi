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

  [TestCase("", "2000-01-01", "test@example.com", "NotEmptyValidator", "'Name' deve ser informado.")]
  [TestCase(null, "2000-01-01", "test@example.com", "NotNullValidator", "'Name' não pode ser nulo.")]
  [TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "2000-01-01", "test@example.com", "MaximumLengthValidator", "'Name' deve ser menor ou igual a 255 caracteres. Você digitou 256 caracteres.")]
  [TestCase("John Doe", "", "test@example.com", "NotEmptyValidator", "'Birth Date' deve ser informado.")]
  [TestCase("John Doe", "2100-01-01", "test@example.com", "LessThanOrEqualValidator", "'Birth Date' deve ser inferior ou igual a")]
  [TestCase("John Doe", "0001-01-01", "test@example.com", "GreaterThanValidator", "'Birth Date' deve ser superior a '01/01/0001 00:00:00'.")]
  [TestCase("John Doe", "2000-01-01", "", "NotEmptyValidator", "'Email' deve ser informado.")]
  [TestCase("John Doe", "2000-01-01", null, "NotNullValidator", "'Email' não pode ser nulo.")]
  [TestCase("John Doe", "2000-01-01", "invalid-email", "EmailValidator", "'Email' é um endereço de email inválido.")]
  public async Task CreateCustomer_FieldValidations(string name, string birthDate, string email, string expectedErrorCode, string expectedErrorMessage)
  {
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
      Assert.That(result.Errors.Any(e => e.Code == expectedErrorCode), Is.True, $"Codigo de erro esperado '{expectedErrorCode}'");
      Assert.That(result.Errors.Any(e => e.Description.Contains(expectedErrorMessage)), Is.True, $"Mensagem de erro esperada '{expectedErrorMessage}'");
    });
  }
}
