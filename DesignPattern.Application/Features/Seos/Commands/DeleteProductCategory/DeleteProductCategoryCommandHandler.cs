using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.DeleteProductCategory;

public class DeleteProductCategoryCommandHandler
  : IRequestHandler<DeleteProductCategoryCommand, Result>
{
  private readonly IUnitOfWork _unitOfWork;

  public DeleteProductCategoryCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
  {
    var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(request.Id, cancellationToken);
    if (category is null)
    {
      return Result.Failure(ProductCategoryErrors.NotFound);
    }

    // TODO: kiểm tra có category con, với có sản phẩm thì không cho xóa

    _unitOfWork.ProductCategoryRepository.Delete(category);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }
}
