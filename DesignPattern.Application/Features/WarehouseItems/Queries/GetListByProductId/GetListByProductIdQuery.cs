using MediatR;
using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Common;

namespace DesignPattern.Application.Features.WarehouseItems.Queries.GetListByProductId;

public class GetListByProductIdQuery : IRequest<Result<List<WarehouseItemResponse>>>
{
  public Guid ProductId { get; set; }
}