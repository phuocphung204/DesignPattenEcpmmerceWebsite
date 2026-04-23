using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Queries.GetBrandById;

public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<BrandResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetBrandByIdQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<BrandResponse>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
  {
    var brand = await _unitOfWork.BrandRepository.GetByIdAsync(request.Id, cancellationToken);
    if (brand is null)
    {
      return Result<BrandResponse>.Failure(BrandErrors.BrandNotFound);
    }

    return Result<BrandResponse>.Success(BrandResponse.FromDomain(brand));
  }
}
