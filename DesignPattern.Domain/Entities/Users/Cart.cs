using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.BaseEntity;

namespace DesignPattern.Domain.Entities.Users;

public class AttributeItem
{
  public string Name { get; private set; }
  public string Value { get; private set; }

  public AttributeItem(string name, string value)
  {
    Name = name;
    Value = value;
  }
}

public class Cart : BaseEntity
{
  public Guid UserId { get; private set; }
  public List<CartItem> Items { get; private set; } = new();

  private Cart(Guid userId)
  {
    UserId = userId;
  }

  public static Cart Create(Guid userId)
  {
    return new Cart(userId);
  }

  public Result AddItem(CartItem cartItem)
  {
    foreach (var item in Items)
    {
      if (item.ProductId == cartItem.ProductId)
      {
        var updateResult = item.UpdateQuantity(item.Quantity.Value + cartItem.Quantity.Value);
        if (updateResult.IsFailure)
          return updateResult.Error;

        return true;
      }
    }

    Items.Add(cartItem);
    return true;
  }

  public Result RemoveItem(Guid productId)
  {
    var item = Items.FirstOrDefault(i => i.ProductId == productId);
    if (item is null)
      return CartErrors.ItemNotFound;
    Items.Remove(item);
    return true;
  }

  public Result UpdateQuantity(Guid productId, int newQuantity)
  {
    var item = Items.FirstOrDefault(i => i.ProductId == productId);
    if (item is null)
      return CartErrors.ItemNotFound;

    if (newQuantity == 0)
      return RemoveItem(productId);

    var updateResult = item.UpdateQuantity(newQuantity);
    if (updateResult.IsFailure)
      return updateResult.Error;

    return true;
  }
}

public class CartItem
{
  public Guid ProductId { get; private set; }
  public string ImageLink { get; private set; }
  public StandardText Sku { get; private set; }
  public Name Name { get; private set; }
  public Quantity Quantity { get; private set; }
  public Price SellingPrice { get; private set; }
  public List<AttributeItem> Attributes { get; private set; } = new();

  private CartItem(Guid productId, string imageLink, StandardText sku, Name name, Quantity quantity, Price sellingPrice, List<AttributeItem> attributes)
  {
    ProductId = productId;
    ImageLink = imageLink;
    Sku = sku;
    Name = name;
    Quantity = quantity;
    SellingPrice = sellingPrice;
    Attributes = attributes ?? new List<AttributeItem>();
  }

  public static CartItem Create(Guid productId, string imageLink, StandardText sku, Name name, Quantity quantity, Price sellingPrice, List<AttributeItem> attributes)
  {
    return new CartItem(productId, imageLink, sku, name, quantity, sellingPrice, attributes ?? new List<AttributeItem>());
  }

  public Result UpdateQuantity(int newQuantity)
  {
    var quantityResult = Quantity.Create(newQuantity);
    if (quantityResult.IsFailure)
      return quantityResult.Error;

    Quantity = quantityResult.Value;
    return Result.Success();
  }
}