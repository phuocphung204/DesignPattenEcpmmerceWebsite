using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Products.Shared;

public record UpdateProductResponse(
    Guid Id,
    string Name,
    Guid BrandId,
    string BrandName,
    Guid CategoryId,
    string CategoryName,
    string Sku,
    string VariantGroupId,
    List<string> Images,
    List<ProductAttributeResponse> Attributes,
    decimal SellingPrice,
    decimal PurchasePrice,
    ProductStatus Status,
    string ShortDescription,
    string DetailDescription
);
public record ProductDetailResponse(
    Guid Id,
    string Name,
    Guid BrandId,
    string BrandName,
    Guid CategoryId,
    string CategoryName,
    string Sku,
    string VariantGroupId,
    List<string> Images,
    List<ProductAttributeResponse> Attributes,
    decimal SellingPrice,
    decimal PurchasePrice,
    int StockQuantity,
    int SoldQuantity,
    ProductStatus Status,
    string ShortDescription,
    string DetailDescription,
    decimal Rating,
    SeoResponse Seo
);

public record ProductAttributeResponse(string Name, string Value, ProductAttributeType Type);
public record SeoResponse(string? Title, string? Description, string? Slug);