using MediatR;
using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.WarehouseItems.Commands.DeleteItem;

public record DeleteItemCommand(Guid Id) : IRequest<Result<WarehouseItemResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}
