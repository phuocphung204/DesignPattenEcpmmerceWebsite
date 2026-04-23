using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Entities.Inventory;

namespace DesignPattern.Application.Features.Warehouses.Queries.GetPageWarehouse;

public sealed class GetPageWarehouseQueryHandler : IRequestHandler<GetPageWarehouseQuery, PagedResult<WarehouseReadModel>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetPageWarehouseQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<PagedResult<WarehouseReadModel>> Handle(GetPageWarehouseQuery request, CancellationToken cancellationToken)
  {
    var warehouses = await _unitOfWork.WarehouseRepository.GetPagedAsync(
      new WarehouseCriteria
      {
        Name = request.Name,
        Address = request.Address
      },
      request.PageIndex,
      request.PageSize,
      cancellationToken);

    var warehouseReadModels = warehouses.Items.Select(w => new WarehouseReadModel(
      w.Id,
      w.Name.Value,
      w.Address
    )).ToList();

    return new PagedResult<WarehouseReadModel>
    {
      Items = warehouseReadModels,
      TotalCount = warehouses.TotalCount,
      PageIndex = warehouses.PageIndex,
      PageSize = warehouses.PageSize
    };
  }
}