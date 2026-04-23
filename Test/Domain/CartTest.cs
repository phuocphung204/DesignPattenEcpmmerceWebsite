using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.ValueObjects;
using FluentAssertions;

namespace Test.Domain;

public class CartTests
{
  [Fact]
  public void AddItem_Should_IncreaseItemsCount()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var cart = Cart.Create(userId);
    var productId = Guid.NewGuid();

    var skuResult = StandardText.Create("SKU-123");
    skuResult.IsSuccess.Should().BeTrue(skuResult.Error.Message);

    var nameResult = Name.Create("Laptop Dell");
    nameResult.IsSuccess.Should().BeTrue(nameResult.Error.Message);

    var quantityResult = Quantity.Create(2);
    quantityResult.IsSuccess.Should().BeTrue(quantityResult.Error.Message);

    var sellingPriceResult = Price.Create(15000000m);
    sellingPriceResult.IsSuccess.Should().BeTrue(sellingPriceResult.Error.Message);

    var cartItem = CartItem.Create(
      productId,
      "link.jpg",
      skuResult.Value,
      nameResult.Value,
      quantityResult.Value,
      sellingPriceResult.Value,
      []);

    // Act
    var result = cart.AddItem(cartItem);

    // Assert
    result.IsSuccess.Should().BeTrue();
    cart.Items.Should().HaveCount(1);
    cart.Items.First().ProductId.Should().Be(productId);
  }

  [Fact]
  public void UpdateQuantity_Should_ModifyItemQuantity()
  {
    // Arrange
    var cart = Cart.Create(Guid.NewGuid());
    var productId = Guid.NewGuid();

    var skuResult = StandardText.Create("S");
    skuResult.IsSuccess.Should().BeTrue(skuResult.Error.Message);

    var nameResult = Name.Create("N");
    nameResult.IsSuccess.Should().BeTrue(nameResult.Error.Message);

    var quantityResult = Quantity.Create(1);
    quantityResult.IsSuccess.Should().BeTrue(quantityResult.Error.Message);

    var sellingPriceResult = Price.Create(10m);
    sellingPriceResult.IsSuccess.Should().BeTrue(sellingPriceResult.Error.Message);

    var cartItem = CartItem.Create(
      productId,
      "",
      skuResult.Value,
      nameResult.Value,
      quantityResult.Value,
      sellingPriceResult.Value,
      []);
    cart.AddItem(cartItem);

    // Act
    var updateResult = cart.UpdateQuantity(productId, 5);

    // Assert
    updateResult.IsSuccess.Should().BeTrue();
    cart.Items.First().Quantity.Value.Should().Be(5);
  }

  [Fact]
  public void AddItem_Should_MergeQuantity_When_ProductAlreadyExists()
  {
    // Arrange
    var cart = Cart.Create(Guid.NewGuid());
    var productId = Guid.NewGuid();

    var skuResult = StandardText.Create("SKU-123");
    skuResult.IsSuccess.Should().BeTrue(skuResult.Error.Message);

    var nameResult = Name.Create("Laptop Dell");
    nameResult.IsSuccess.Should().BeTrue(nameResult.Error.Message);

    var firstQuantityResult = Quantity.Create(2);
    firstQuantityResult.IsSuccess.Should().BeTrue(firstQuantityResult.Error.Message);

    var secondQuantityResult = Quantity.Create(3);
    secondQuantityResult.IsSuccess.Should().BeTrue(secondQuantityResult.Error.Message);

    var sellingPriceResult = Price.Create(15000000m);
    sellingPriceResult.IsSuccess.Should().BeTrue(sellingPriceResult.Error.Message);

    var firstItem = CartItem.Create(
      productId,
      "link.jpg",
      skuResult.Value,
      nameResult.Value,
      firstQuantityResult.Value,
      sellingPriceResult.Value,
      []);

    var secondItem = CartItem.Create(
      productId,
      "link.jpg",
      skuResult.Value,
      nameResult.Value,
      secondQuantityResult.Value,
      sellingPriceResult.Value,
      []);

    // Act
    cart.AddItem(firstItem);
    var addAgainResult = cart.AddItem(secondItem);

    // Assert
    addAgainResult.IsSuccess.Should().BeTrue();
    cart.Items.Should().HaveCount(1);
    cart.Items.First().Quantity.Value.Should().Be(5);
  }
}