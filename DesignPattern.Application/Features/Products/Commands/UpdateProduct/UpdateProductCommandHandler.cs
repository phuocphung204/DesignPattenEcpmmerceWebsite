using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Products.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Entities;
using DesignPattern.Domain.ValueObjects;

namespace DesignPattern.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<UpdateProductResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ISlugChecker _slugChecker;

  public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ISlugChecker slugChecker)
  {
    _unitOfWork = unitOfWork;
    _slugChecker = slugChecker;
  }

  public async Task<Result<UpdateProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    // Kiểm tra product tồn tại
    var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id, cancellationToken);
    if (product is null)
      return ProductErrors.ProductNotFound;

    // Kiểm tra brand tồn tại
    var brand = await _unitOfWork.BrandRepository.GetByIdAsync(dto.BrandId, cancellationToken);
    if (brand is null)
      return ProductErrors.BrandNotFound;
    // Kiểm tra category tồn tại
    var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
    if (category is null)
      return ProductErrors.CategoryNotFound;

    List<ProductAttribute> attributes = new();
    for (int i = 0; i < dto.Attributes.Count; i++)
    {
      var attr = dto.Attributes[i];
      var attributeResult = ProductAttribute.Create(attr.Name, attr.Value, attr.Type);
      if (attributeResult.IsFailure)
        return attributeResult.Error;
      attributes.Add(attributeResult.Value);
    }

    var result = product.Update(
      name: dto.Name,
      brandId: dto.BrandId,
      categoryId: dto.CategoryId,
      brandName: dto.BrandName,
      categoryName: dto.CategoryName,
      sku: dto.Sku,
      variantGroupId: dto.VariantGroupId,
      images: dto.Images,
      attributes: attributes,
      shortDescription: dto.ShortDescription,
      detailDescription: dto.DetailDescription,
      sellingPrice: dto.SellingPrice,
      purchasePrice: dto.PurchasePrice,
      status: dto.Status);
    if (result.IsFailure)
      return result.Error;

    _unitOfWork.ProductRepository.Update(product);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = new UpdateProductResponse(
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
      Status: product.Status,
      ShortDescription: product.ShortDescription,
      DetailDescription: product.DetailDescription
    );

    return response;
  }
}
