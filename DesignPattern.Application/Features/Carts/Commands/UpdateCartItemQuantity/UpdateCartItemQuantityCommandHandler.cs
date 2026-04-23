using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;
using DesignPattern.Application.Features.Carts.Shared;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Carts.Commands.UpdateCartItemQuantity;

public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, Result<List<CartItemResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public UpdateCartItemQuantityCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<List<CartItemResponse>>> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
    if (cart is null)
      return CartErrors.NotFound;

    var updateResult = cart.UpdateQuantity(request.ProductId, dto.NewQuantity);
    if (updateResult.IsFailure)
      return updateResult.Error;

    _unitOfWork.CartRepository.Update(cart);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = cart.Items.Select(item => new CartItemResponse(
      item.ProductId,
      item.ImageLink,
      item.Sku.Value,
      item.Name.Value,
      item.Quantity.Value,
      item.SellingPrice.Amount,
      item.Attributes
    )).ToList();

    return response;
  }
}