using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.DeleteBrand;

public class DeleteBrandHandler : IRequestHandler<DeleteBrandCommand, Result>
{
  private readonly IUnitOfWork _unitOfWork;

  public DeleteBrandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
  {
    var brand = await _unitOfWork.BrandRepository.GetByIdAsync(request.Id, cancellationToken);
    if (brand is null)
    {
      return Result.Failure(BrandErrors.BrandNotFound);
    }

    // TODO: Check if brand is in use by products if needed

    _unitOfWork.BrandRepository.Delete(brand);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }
}
