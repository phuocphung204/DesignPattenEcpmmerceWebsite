using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Queries.GetProductCategoryById;

public sealed class GetProductCategoryByIdQueryHandler : IRequestHandler<GetProductCategoryByIdQuery, Result<ProductCategoryResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetProductCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<ProductCategoryResponse>> Handle(GetProductCategoryByIdQuery request, CancellationToken cancellationToken)
  {
    var productCategoryId = request.Id;
    var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(productCategoryId, cancellationToken);

    if (category is null)
    {
      return Result<ProductCategoryResponse>.Failure(ProductCategoryErrors.NotFound);
    }

    return Result<ProductCategoryResponse>.Success(ProductCategoryResponse.FromDomain(category));
  }
}
