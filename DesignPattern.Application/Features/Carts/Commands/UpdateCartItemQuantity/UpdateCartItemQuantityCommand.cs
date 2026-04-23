using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Carts.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Carts.Commands.UpdateCartItemQuantity;

public record UpdateQuantityDto(
  int NewQuantity
);
public record UpdateCartItemQuantityCommand(
  Guid ProductId,
  UpdateQuantityDto dto
) : IRequest<Result<List<CartItemResponse>>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
}