using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Warehouses.Shared;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;

namespace DesignPattern.Application.Features.Warehouses.Commands.UpdateWarehouse;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, Result<WarehouseResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  public UpdateWarehouseCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<WarehouseResponse>> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var warehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(request.Id, cancellationToken);
    if (warehouse is null)
    {
      return Result<WarehouseResponse>.Failure(WarehouseErrors.WarehouseNotFound);
    }

    var updateResult = warehouse.Update(dto.Name, dto.Address);
    if (updateResult.IsFailure)
    {
      return Result<WarehouseResponse>.Failure(updateResult.Error);
    }

    _unitOfWork.WarehouseRepository.Update(warehouse);
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