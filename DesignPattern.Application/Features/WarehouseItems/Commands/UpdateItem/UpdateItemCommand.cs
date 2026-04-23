using MediatR;
using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.WarehouseItems.Commands.UpdateItem;

public record UpdateItemDTO
(
  string? Name,
  string? Sku,
  string? VariantGroupId,
  int? Quantity
);
public record UpdateItemCommand(
  Guid Id,
  UpdateItemDTO dto) : IRequest<Result<WarehouseItemResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}

