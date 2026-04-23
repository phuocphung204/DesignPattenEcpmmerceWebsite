using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;
using DesignPattern.Application.Features.Carts.Shared;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Entities.Users;

namespace DesignPattern.Application.Features.Carts.Queries.GetCartByUserId;

public class GetCartByUserIdQueryHandler : IRequestHandler<GetCartByUserIdQuery, Result<CartResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetCartByUserIdQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<CartResponse>> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
  {
    var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
    if (cart is null)
      return CartErrors.NotFound;

    var cartItems = cart!.Items.Select(item => new CartItemResponse(
      item.ProductId,
      item.ImageLink,
      item.Sku.Value,
      item.Name.Value,
      item.Quantity.Value,
      item.SellingPrice.Amount,
      item.Attributes
    )).ToList();

    return new CartResponse(cart.UserId, cartItems);
  }
}
