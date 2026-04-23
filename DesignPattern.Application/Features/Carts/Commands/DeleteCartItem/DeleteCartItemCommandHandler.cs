using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;
using DesignPattern.Application.Features.Carts.Shared;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Carts.Commands.DeleteCartItem;

public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, Result<List<CartItemResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public DeleteCartItemCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<List<CartItemResponse>>> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
  {
    var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
    if (cart is null)
      return CartErrors.NotFound;

    var deleteResult = cart.RemoveItem(request.ProductId);
    if (deleteResult.IsFailure)
      return deleteResult.Error;

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

    return Result<List<CartItemResponse>>.Success(response);
  }
}