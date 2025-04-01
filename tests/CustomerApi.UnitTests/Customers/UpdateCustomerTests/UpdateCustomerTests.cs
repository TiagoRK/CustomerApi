using CustomerApi.Application.Commands.Customers.Update;
using Moq;

namespace CustomerApi.UnitTests.Customers.UpdateCustomerTests;
public class UpdateCustomerTests : CustomerTestBase
{
  [Test]
  public async Task UpdateCustomer_Success()
  {
    var command = new UpdateCustomerCommand
    {
      CurrentEmail = _fakeCustomer.Email,
      Name = "New Name",
      BirthDate = new DateTime(2001, 07, 25),
      Email = "new_email@email.com"
    };

    _customerRepositoryMock
         .Setup(repo => repo.GetByEmail(_fakeCustomer.Email, false))
         .ReturnsAsync(_fakeCustomer);

    _customerRepositoryMock
         .Setup(repo => repo.IsEmailUnique(command.Email))
         .ReturnsAsync(true);

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Null);
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Value.Name, Is.EqualTo(command.Name));
      Assert.That(result.Value.BirthDate, Is.EqualTo(command.BirthDate));
      Assert.That(result.Value.Email, Is.EqualTo(command.Email));
    });
  }

  [Test]
  public async Task UpdateCustomer_IsNotOfLegalAge_Failure()
  {
    var command = new UpdateCustomerCommand
    {
      CurrentEmail = _fakeCustomer.Email,
      Name = "New Name",
      BirthDate = DateTime.Now.AddYears(-1),
      Email = "new_email@email.com"
    };

    _customerRepositoryMock
         .Setup(repo => repo.GetByEmail(_fakeCustomer.Email, false))
         .ReturnsAsync(_fakeCustomer);

    _customerRepositoryMock
         .Setup(repo => repo.IsEmailUnique(command.Email))
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
  public async Task UpdateCustomer_EmailIsNotUnique_Failure()
  {
    var command = new UpdateCustomerCommand
    {
      CurrentEmail = _fakeCustomer.Email,
      Name = "New Name",
      BirthDate = new DateTime(2001, 07, 25),
      Email = "new_email@email.com"
    };

    _customerRepositoryMock
         .Setup(repo => repo.GetByEmail(_fakeCustomer.Email, false))
         .ReturnsAsync(_fakeCustomer);

    _customerRepositoryMock
         .Setup(repo => repo.IsEmailUnique(command.Email))
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

  [TestCase("test@email.com", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "2000-01-01", "test@example.com", "MaximumLengthValidator", "'Name' must be 255 characters or fewer. You entered 256 characters.")]
  [TestCase("test@email.com", "John Doe", "2100-01-01", "test@example.com", "LessThanOrEqualValidator", "'Birth Date' must be less than or equal to")]
  [TestCase("test@email.com", "John Doe", "0001-01-01", "test@example.com", "GreaterThanValidator", "'Birth Date' must be greater than '01/01/0001 00:00:00'.")]
  [TestCase("", "John Doe", "2000-01-01", "test@example.com", "NotEmptyValidator", "'Current Email' must not be empty.")]
  [TestCase("invalid-email", "John Doe", "2000-01-01", "test@example.com", "EmailValidator", "'Current Email' is not a valid email address.")]
  [TestCase("test@email.com", "John Doe", "2000-01-01", "invalid-email", "EmailValidator", "'Email' is not a valid email address.")]
  public async Task UpdateCustomer_FieldValidations(string currentEmail, string name, string birthDate, string email, string expectedErrorCode, string expectedErrorMessage)
  {
    var command = new UpdateCustomerCommand
    {
      CurrentEmail = currentEmail,
      Name = name,
      BirthDate = string.IsNullOrEmpty(birthDate) ? default : DateTime.Parse(birthDate),
      Email = email
    };

    _customerRepositoryMock
         .Setup(repo => repo.GetByEmail(currentEmail, false))
         .ReturnsAsync(_fakeCustomer);

    _customerRepositoryMock
         .Setup(repo => repo.IsEmailUnique(command.Email))
         .ReturnsAsync(true);

    var result = await _mediator.Send(command);

    Assert.Multiple(() =>
    {
      Assert.That(result.Errors, Is.Not.Null);
      Assert.That(result.IsSuccess, Is.False);
      Assert.That(result.Errors.Any(e => e.Code == expectedErrorCode), Is.True, $"Error code expected '{expectedErrorCode}'");
      Assert.That(result.Errors.Any(e => e.Description.Contains(expectedErrorMessage)), Is.True, $"Error message expected '{expectedErrorMessage}'");
    });
  }
}
