using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Products.Shared;

public record ProductOnSearchPageResponse(
    Guid Id,
    string Name,
    string Image,
    decimal SellingPrice,
    int SoldQuantity,
    decimal Rating,
    string ShortDescription
);

public record ProductOnGroupResponse(
    Guid Id,
    string Name,
    string VariantGroupId,
    string Image,
    decimal SellingPrice,
    int SoldQuantity,
    decimal Rating,
    string ShortDescription,
    SeoResponse Seo

);