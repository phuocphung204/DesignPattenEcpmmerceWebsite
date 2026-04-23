using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Products.Shared;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<ProductDetailResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<ProductDetailResponse>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
  {
    var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id, cancellationToken);
    if (product is null)
      return Result<ProductDetailResponse>.Failure(ProductErrors.ProductNotFound);

    _unitOfWork.ProductRepository.Delete(product);
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
}
