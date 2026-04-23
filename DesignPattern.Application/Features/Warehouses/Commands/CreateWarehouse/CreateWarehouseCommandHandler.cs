using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Entities.Inventory;
using DesignPattern.Application.Features.Warehouses.Shared;

namespace DesignPattern.Application.Features.Warehouses.Commands.CreateWarehouse;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Result<WarehouseResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  public CreateWarehouseCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<WarehouseResponse>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var warehouseResult = Warehouse.Create(dto.Name, dto.Address);
    if (warehouseResult.IsFailure)
    {
      return Result<WarehouseResponse>.Failure(warehouseResult.Error);
    }

    var warehouse = warehouseResult.Value;
    _unitOfWork.WarehouseRepository.Create(warehouse);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = new WarehouseResponse
    {
      Id = warehouse.Id,
      Name = warehouse.Name.Value,
      Address = warehouse.Address
    };
    return Result<WarehouseResponse>.Success(response);
  }
}