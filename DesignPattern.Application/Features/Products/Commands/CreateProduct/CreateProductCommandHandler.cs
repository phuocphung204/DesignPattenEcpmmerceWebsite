using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Entities;
using DesignPattern.Application.Features.Products.Shared;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;

namespace DesignPattern.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDetailResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ISlugChecker _slugChecker;
  public CreateProductCommandHandler(IUnitOfWork unitOfWork, ISlugChecker slugChecker)
  {
    _unitOfWork = unitOfWork;
    _slugChecker = slugChecker;
  }
  public async Task<Result<ProductDetailResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    // Kiểm tra category tồn tại
    var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
    if (category is null)
      return ProductErrors.CategoryNotFound;
    // Kiểm tra brand tồn tại
    var brand = await _unitOfWork.BrandRepository.GetByIdAsync(dto.BrandId, cancellationToken);
    if (brand is null)
      return ProductErrors.BrandNotFound;

    // kiểm tra slug Seo
    var uniqueSlugResult = await _slugChecker.GenerateUniqueSlugAsync(dto.Name, cancellationToken);
    if (uniqueSlugResult.IsFailure)
      return uniqueSlugResult.Error;

    var SeoResult = Seo.Create(dto.Title, dto.MetaDescription, uniqueSlugResult.Value);
    if (SeoResult.IsFailure)
      return SeoResult.Error;

    var seo = SeoResult.Value;

    List<ProductAttribute> attributes = new();
    for (int i = 0; i < dto.Attributes.Count; i++)
    {
      var attr = dto.Attributes[i];
      var attributeResult = ProductAttribute.Create(attr.Name, attr.Value, attr.Type);
      if (attributeResult.IsFailure)
        return attributeResult.Error;
      attributes.Add(attributeResult.Value);
    }

    var productResult = Product.Create(
      name: dto.Name,
      brandId: dto.BrandId,
      brandName: dto.BrandName,
      categoryId: dto.CategoryId,
      categoryName: dto.CategoryName,
      sku: dto.Sku,
      variantGroupId: dto.VariantGroupId,
      images: dto.Images,
      attributes: attributes,
      sellingPrice: dto.SellingPrice,
      purchasePrice: dto.PurchasePrice,
      shortDescription: dto.ShortDescription,
      detailDescription: dto.DetailDescription,
      seo: seo
    );
    var product = productResult.Value;
    if (productResult.IsFailure)
      return productResult.Error;

    _unitOfWork.ProductRepository.Create(product);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = new ProductDetailResponse(
      Id: product.Id,
      Name: product.Name.Value,
      BrandId: product.BrandId,
      BrandName: product.BrandName.Value,
      CategoryId: product.CategoryId,
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

    return response;
  }
}
