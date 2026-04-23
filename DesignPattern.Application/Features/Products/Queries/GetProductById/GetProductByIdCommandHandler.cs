using DesignPattern.Application.Features.Products.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdCommandHandler : IRequestHandler<GetProductByIdCommand, Result<ProductDetailResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  public GetProductByIdCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<ProductDetailResponse>> Handle(GetProductByIdCommand request, CancellationToken cancellationToken)
  {
    var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id, cancellationToken);
    if (product is null)
    {
      return Result<ProductDetailResponse>.Failure(ProductErrors.ProductNotFound);
    }
    var stockQuantityResult = await GetStockQuantity(product.Id, cancellationToken);
    if (stockQuantityResult.IsFailure)
    {
      return Result<ProductDetailResponse>.Failure(stockQuantityResult.Error);
    }
    var stockQuantity = stockQuantityResult.Value;
    // Update stock quantity in product entity
    product.UpdateStockQuantity(stockQuantity);
    _unitOfWork.ProductRepository.Update(product);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = new ProductDetailResponse(
      Id: product.Id,
      Name: product.Name.Value,
      BrandId: product.BrandId,
      CategoryId: product.CategoryId,
      BrandName: product.BrandName.Value,
      CategoryName: product.CategoryName.Value,
      Sku: product.Sku.Value,
      VariantGroupId: product.VariantGroupId.Value,
      Images: product.Images,
      Attributes: product.Attributes.Select(a => new ProductAttributeResponse(a.Name.Value, a.Value.Value, a.Type)).ToList(),
      SellingPrice: product.SellingPrice.Amount,
      PurchasePrice: product.PurchasePrice.Amount,
      StockQuantity: product.StockQuantity.Value,
      SoldQuantity: product.SoldQuantity.Value,
      Status: product.Status,
      ShortDescription: product.ShortDescription,
      DetailDescription: product.DetailDescription,
      Rating: product.Rating,
      Seo: new SeoResponse(product.Seo.Title?.Value, product.Seo.Description?.Value, product.Seo.Slug?.Value)
    );
    return Result<ProductDetailResponse>.Success(response);
  }

  private async Task<Result<int>> GetStockQuantity(Guid productId, CancellationToken cancellationToken)
  {
    var warehouseItems = await _unitOfWork.WarehouseItemRepository
     .GetListWarehouseItemByProductIdAsync(productId, cancellationToken);

    if (warehouseItems.Count == 0)
      return WarehouseErrors.WarehouseItemNotFound;

    var Stock = warehouseItems.Sum(item => item.GetStock().Value);
    return Stock;
  }
}
