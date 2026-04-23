using DesignPattern.Domain.Entities;
using DesignPattern.Domain.Entities.DiscountCodes;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.Enums;
using FluentAssertions;

namespace Test.Domain;

public class DiscountCodeTests
{
  [Fact]
  public void ApplyDiscount_Should_ReturnCorrectDiscount_ForPercentage()
  {
    // Arrange

    var percentageDiscountCode = PercentageDiscountCode.Create(
      code: "SALE10",
      minimumOrderAmount: 200000m, // Don toi thieu 200k
      usageLimit: 100,
      expirationDate: DateTime.UtcNow.AddDays(5),
      percent: 10,
      maximumDiscountAmount: 50000m
    ).Value;

    var subtotal = Price.Create(300000m).Value; // 10% của 300k = 30k

    // Act
    var discount = percentageDiscountCode.CheckAppliableDiscount(subtotal);

    // Assert
    discount.IsSuccess.Should().BeTrue(discount.Error.Message);
    discount.Value.Amount.Should().Be(300000m * 0.1m); // 30,000
  }

  [Fact]
  public void ApplyDiscount_Should_ThrowException_WhenSubtotalTooLow()
  {
    // Arrange

    var fixedDiscountCode = FixedDiscountCode.Create(
      code: "FIX20",
      minimumOrderAmount: 200000m,
      usageLimit: 100,
      expirationDate: DateTime.UtcNow.AddDays(5),
      amount: 20000m
    ).Value;

    var subtotal = Price.Create(150000m).Value; // Mua có 150k, không đủ min 200k

    // Act
    var result = fixedDiscountCode.CheckAppliableDiscount(subtotal);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Code.Should().Be("DiscountCode.MinimumOrderAmountNotMet");
  }
}