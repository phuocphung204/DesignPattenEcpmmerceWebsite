using DesignPattern.Domain.Entities.Users;

namespace DesignPattern.Application.Features.Carts.Shared;

public record CartItemResponse(
  Guid ProductId,
  string ImageLink,
  string Sku,
  string Name,
  int Quantity,
  decimal SellingPrice,
  List<AttributeItem> Attributes
);
