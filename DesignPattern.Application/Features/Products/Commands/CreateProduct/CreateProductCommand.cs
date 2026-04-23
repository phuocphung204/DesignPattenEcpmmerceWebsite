using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.Products.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

namespace DesignPattern.Application.Features.Products.Commands.CreateProduct;

public record ProductAttributeDto(string Name, string Value, ProductAttributeType Type);
public record CreateProductDTO(
    string Name,
    Guid BrandId,
    Guid CategoryId,
    string BrandName,
    string CategoryName,
    string VariantGroupId,
    string Sku,
    decimal SellingPrice,
    decimal PurchasePrice,
    int StockQuantity,
    string ShortDescription,
    string DetailDescription,
    List<string> Images,
    List<ProductAttributeDto> Attributes,
    string? Title,
    string? MetaDescription);

public record CreateProductCommand(
  CreateProductDTO dto
) : IRequest<Result<ProductDetailResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}
