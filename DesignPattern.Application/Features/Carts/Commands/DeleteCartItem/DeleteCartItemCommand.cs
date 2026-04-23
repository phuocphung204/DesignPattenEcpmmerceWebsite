using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Carts.Shared;
using DesignPattern.Application.Abstractions;

namespace DesignPattern.Application.Features.Carts.Commands.DeleteCartItem;

public record DeleteCartItemCommand(
  Guid ProductId
) : IRequest<Result<List<CartItemResponse>>>, IRequiresUserContext
{
  public Guid UserId { get; set; }
}