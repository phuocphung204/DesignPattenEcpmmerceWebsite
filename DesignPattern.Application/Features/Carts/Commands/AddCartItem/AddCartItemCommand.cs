using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Application.Features.Carts.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Carts.Commands.AddCartItem;

public record AddCartItemDto(
  Guid ProductId,
  int Quantity
);
public record AddCartItemCommand(
  AddCartItemDto dto
) : IRequest<Result<List<CartItemResponse>>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
}