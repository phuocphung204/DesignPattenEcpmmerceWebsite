// using DesignPattern.Domain.Common;
// using DesignPattern.Domain.Entities.User;
// using DesignPattern.Domain.ValueObjects;
// using DesignPattern.Domain.ValueObjects.User;
// using DesignPattern.Domain.ValueObjects.BaseEntity;
// using FluentAssertions;
// using Xunit.Abstractions;
// using System.Text.Json;

// namespace Test.Domain;

// public class UserTests
// {
//   private readonly ITestOutputHelper _output;

//   public UserTests(ITestOutputHelper output)
//   {
//     _output = output;
//   }

//   [Fact]
//   public void CreateUser_Should_ReturnSuccess_WhenAllDataIsValid()
//   {
//     // Arrange
//     var id = ID.Create(Guid.NewGuid().ToString()).Value;
//     var name = Name.Create("John Doe").Value;
//     var email = EmailAddress.Create("johndoe@example.com").Value;
//     var phone = PhoneNumber.Create("0912345678").Value;
//     var pass = PasswordHash.Create("hashed_password_123").Value;
//     var point = LoyaltyPoint.Default();

//     // Act
//     // Call BaseEntity<User>.Create directly to test the Reflection mechanism
//     var result = BaseEntity<User>.Create(id, name, email, phone, pass, point);

//     // Assert
//     if (result.IsFailure)
//     {
//       _output.WriteLine($"Error: {result.Error.Code} - {result.Error.Message}");
//     }

//     result.IsSuccess.Should().BeTrue();
//     var user = result.Value;

//     user.Name.Should().Be(name);
//     user.Email.Should().Be(email);
//     user.PhoneNumber.Should().Be(phone);
//     user.LoyaltyPoints.Value.Should().Be(0);

//     _output.WriteLine($"User created: {user.Name.Value} - {user.Email.Value}");
//   }

//   [Fact]
//   public void CreateUserWrapper_Should_ReturnFailure_WhenPhoneInvalid()
//   {
//     // Act
//     var result = User.CreateUser(
//         fullName: "Jane Doe",
//         email: "jane@test.com",
//         phoneNumber: "123", // Invalid format
//         passwordHashHash: "pass"
//     );

//     // Assert
//     result.IsFailure.Should().BeTrue();
//     result.Error.Code.Should().Be("PhoneNumber.InvalidFormat");
//   }
// }