using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

namespace DesignPattern.Application.Features.WarehouseItems.Commands.CreateItem;

public record CreateItemDTO(
  Guid WarehouseId,
  string WarehouseName,
  Guid ProductId,
  string ProductName,
  string Sku,
  string VariantGroupId,
  int Quantity
  );
public record CreateItemCommand(CreateItemDTO dto) : IRequest<Result<WarehouseItemResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}

