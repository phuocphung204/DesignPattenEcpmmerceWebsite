using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Products.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Entities;

namespace DesignPattern.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductAttributeDto(string Name, string Value, ProductAttributeType Type);
public record UpdateProductDto(
  string Name,
  Guid CategoryId,
  Guid BrandId,
  string BrandName,
  string CategoryName,
  string Sku,
  string VariantGroupId,
  string ShortDescription,
  string DetailDescription,
  List<string> Images,
  List<UpdateProductAttributeDto> Attributes,
  decimal SellingPrice,
  decimal PurchasePrice,
  ProductStatus Status
);

public record UpdateProductCommand(
  Guid Id,
  UpdateProductDto dto) : IRequest<Result<UpdateProductResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}
