using DesignPattern.Domain.ValueObjects.User;
using FluentAssertions;

namespace Test.Domain;

public class PhoneNumberTests
{
  [Theory]
  [InlineData("0901234567")] // Valid 10 digits
  [InlineData("0123456789")]
  public void Create_Should_ReturnSuccess_WhenFormatIsValid(string phone)
  {
    // Act
    var result = PhoneNumber.Create(phone);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Value.Should().Be(phone);
  }

  [Theory]
  [InlineData("")] // Empty
  [InlineData("090123456")] // 9 digits (too short)
  [InlineData("09012345678")] // 11 digits (too long)
  [InlineData("090123456a")] // Contains letters
  public void Create_Should_ReturnFailure_WhenFormatIsInvalid(string phone)
  {
    // Act
    var result = PhoneNumber.Create(phone);

    // Assert
    result.IsFailure.Should().BeTrue();

    // Verify correct error code
    if (string.IsNullOrEmpty(phone))
      result.Error.Code.Should().Be("PhoneNumber.Empty");
    else
      result.Error.Code.Should().Be("PhoneNumber.InvalidFormat");
  }
}