using DesignPattern.Application.Features.Warehouses.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Warehouses.Queries.GetWarehouseById;

public sealed class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, Result<WarehouseResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetWarehouseByIdQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<WarehouseResponse>> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
  {
    var warehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(request.Id, cancellationToken);
    if (warehouse is null)
    {
      return Result<WarehouseResponse>.Failure(WarehouseErrors.WarehouseNotFound);
    }

    return Result<WarehouseResponse>.Success(new WarehouseResponse
    {
      Id = warehouse.Id,
      Name = warehouse.Name.Value,
      Address = warehouse.Address
    });
  }
}
