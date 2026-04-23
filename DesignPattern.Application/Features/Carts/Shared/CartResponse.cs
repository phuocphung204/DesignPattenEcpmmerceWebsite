using DesignPattern.Domain.Entities.Users;

namespace DesignPattern.Application.Features.Carts.Shared;

public record CartResponse(
  Guid UserId,
  List<CartItemResponse> Items
);
