using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Application.Features.Carts.Shared;
using DesignPattern.Application.Features.Carts.Commands.AddCartItem;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.ValueObjects;

public class AddCartItemCommandHandler : IRequestHandler<AddCartItemCommand, Result<List<CartItemResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public AddCartItemCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<List<CartItemResponse>>> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
    if (cart is null)
      return CartErrors.NotFound;

    var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId, cancellationToken);
    if (product is null)
      return CartErrors.ProductNotFound;

    var quantity = Quantity.Create(dto.Quantity);
    if (quantity.IsFailure)
      return quantity.Error;

    var cartItem = CartItem.Create(
      product.Id,
      product.Images?.FirstOrDefault()!,
      product.Sku,
      product.Name,
      quantity.Value,
      product.SellingPrice,
      product.Attributes.Where(attr => attr.Type is ProductAttributeType.Appearance).Select(attr => new AttributeItem(attr.Name.Value, attr.Value.Value)).ToList()
    );

    var addResult = cart.AddItem(cartItem); // This method should handle adding the item and updating quantity if it already exists
    if (addResult.IsFailure)
      return addResult.Error;

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