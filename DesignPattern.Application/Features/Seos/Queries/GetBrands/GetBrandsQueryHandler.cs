using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Queries.GetBrands;

public sealed class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, Result<PagedResult<BrandListItemResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetBrandsQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<PagedResult<BrandListItemResponse>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
  {
    var dto = request.DTO;

    var pagedResult = await _unitOfWork.BrandRepository.SearchBrandsPagedAsync(
      nameSearchTerm: dto.Name,
      pageIndex: dto.PageIndex,
      pageSize: dto.PageSize,
      cancellationToken: cancellationToken);

    var responseItems = pagedResult.Items.Select(item => new BrandListItemResponse
    {
      Id = item.Id.ToString(),
      Name = item.Name,
      Slug = item.Slug,
      Level = item.Level,
      ParentBrand = item.ParentBrandId.HasValue
            ? new ParentBrandInfo
            {
              Id = item.ParentBrandId.Value.ToString(),
              Name = item.ParentBrandName
            }
            : null
    }).ToList();

    var response = new PagedResult<BrandListItemResponse>
    {
      Items = responseItems,
      TotalCount = pagedResult.TotalCount,
      PageIndex = pagedResult.PageIndex,
      PageSize = pagedResult.PageSize
    };

    return response;
  }
}
