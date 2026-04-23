using System.Text.Json;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;
using DesignPattern.Domain.ValueObjects;
using FluentAssertions;
using Xunit.Abstractions;

namespace Test.Domain;

public class ResultTests
{
  private readonly ITestOutputHelper _output;

  // xUnit sẽ tự động "Inject" ITestOutputHelper vào Constructor
  public ResultTests(ITestOutputHelper output)
  {
    _output = output;
  }
  // 1. Kiểm tra trường hợp Thành công đơn giản
  [Fact]
  public void Success_Should_ReturnIsSuccessTrue_WhenCalled()
  {
    // Arrange & Act
    var result = Result.Success();

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Error.Should().Be(Error.None);
  }

  // 2. Kiểm tra trường hợp Thành công có trả về giá trị (Generic)
  [Fact]
  public void SuccessT_Should_ContainValue_AndIsSuccessBeTrue()
  {
    // Arrange
    var expectedValue = "Dell XPS 13";

    // Act
    var result = Result<string>.Success(expectedValue);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().Be(expectedValue);
  }

  // 3. Kiểm tra trường hợp Thất bại
  [Fact]
  public void Failure_Should_ReturnIsFailureTrue_AndContainError()
  {
    // Arrange
    var error = new Error("Product.NotFound", "Sản phẩm không tồn tại.");

    // Act
    var result = Result.Failure(error);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(error);
    result.Error.Code.Should().Be("Product.NotFound");
  }

  // 4. Kiểm tra việc ném Exception khi cố tình lấy Value từ Result lỗi
  [Fact]
  public void Value_Should_ThrowException_WhenResultIsFailure()
  {
    // Arrange
    var result = Result<int>.Failure(new Error("Err", "Msg"));

    // Act
    Action act = () => { var val = result.Value; };

    // Assert
    act.Should().Throw<InvalidOperationException>()
        .WithMessage("Cannot access the value of a failed Result.");
  }

  // Test extension "Ensure" của Result<T>
  [Theory]
  [InlineData("Hello")]
  [InlineData("Hi")]
  [InlineData("Chuỗi này lớn hơn 10 ký tự không.")]
  public void Ensure_Should_ReturnFailure_WhenConditionIsNotMet(string TestString)
  {
    // Arrange
    var result = Result<string>.From(TestString);

    // Act
    var finalResult = result
      .Ensure(val => val.Length > 5, new Error("Value.TooShort", "Giá trị phải có độ dài lớn hơn 5."))
      .Ensure(val => val.Length < 10, new Error("Value.TooLong", "Giá trị phải có độ dài nhỏ hơn 10."));

    // Assert
    finalResult.IsFailure.Should().BeTrue();
    // Kiểm tra Code phải là một trong các giá trị trong danh sách
    new[] { "Value.TooShort", "Value.TooLong" }.Should().Contain(finalResult.Error.Code);
  }

  // Test extension "Ensure" của Result<T> với điều kiện hợp lệ
  [Theory]
  [InlineData("HelloWorld")]
  public void Ensure_Should_ReturnSuccess_WhenConditionIsMet(string TestString)
  {
    // Arrange
    var result = Result<string>.From(TestString);

    // Act
    var finalResult = result
      .Ensure(val => val.Length > 5, new Error("Value.TooShort", "Giá trị phải có độ dài lớn hơn 5."))
      .Ensure(val => val.Length < 15, new Error("Value.TooLong", "Giá trị phải có độ dài nhỏ hơn 15."));

    // Assert
    finalResult.IsSuccess.Should().BeTrue();
    finalResult.Value.Should().Be(TestString);
  }

  // Test implicit conversion từ TValue sang Result<TValue>
  [Fact]
  public void ImplicitConversion_Should_CreateSuccessResult()
  {
    // Arrange
    string testValue = "Implicit Test";
    Error error = new Error("Test.Error", "This is a test error.");

    // Act
    Result<string> result = testValue; // Sử dụng implicit conversion
    Result<string> errorResult = error; // Sử dụng implicit conversion cho lỗi

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().Be(testValue);
    errorResult.IsFailure.Should().BeTrue();
    errorResult.Error.Should().Be(error);
  }

  [Fact]
  public void SaTest_Should_HaveCorrectTestString()
  {
    // Arrange
    var expectedName = Name.Create("Test Product").Value;
    var options = new JsonSerializerOptions { WriteIndented = true }; // Cho đẹp, dễ đọc
    string jsonLog = JsonSerializer.Serialize(expectedName, options);

    // Act
    var saTest = new SaTest(expectedName);

    // Assert
    _output.WriteLine($"SaTest TestString: {jsonLog}");
    saTest.TestString.Should().Be(expectedName);
  }
}

class SaTest
{
  public Name? TestString { get; private set; }
  public SaTest(Name? testString)
  {
    TestString = testString;
  }
}