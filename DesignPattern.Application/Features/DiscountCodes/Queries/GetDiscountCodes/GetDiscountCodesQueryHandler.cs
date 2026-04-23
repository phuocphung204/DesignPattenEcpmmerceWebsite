using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.DiscountCodes.Queries.GetDiscountCodes;

public class GetDiscountCodesQueryHandler : IRequestHandler<GetDiscountCodesQuery, Result<PagedResult<DiscountCodeReadModel>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetDiscountCodesQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<PagedResult<DiscountCodeReadModel>>> Handle(GetDiscountCodesQuery request, CancellationToken cancellationToken)
  {
    var pagedResult = await _unitOfWork.DiscountCodeRepository.SearchDiscountCodesPagedAsync(
      request.SearchTerm,
      request.IsActive,
      request.StartDate,
      request.EndDate,
      request.PageIndex,
      request.PageSize,
      cancellationToken);

    return pagedResult;
  }
}
