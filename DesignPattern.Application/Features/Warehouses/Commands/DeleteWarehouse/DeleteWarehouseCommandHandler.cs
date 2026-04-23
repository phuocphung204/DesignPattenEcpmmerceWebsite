using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Warehouses.Commands.DeleteWarehouse;

public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, Result>
{
  private readonly IUnitOfWork _unitOfWork;

  public DeleteWarehouseCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
  {
    var warehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken);
    if (warehouse is null)
    {
      return Result.Failure(WarehouseErrors.WarehouseNotFound);
    }

    _unitOfWork.WarehouseRepository.Delete(warehouse);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }
}