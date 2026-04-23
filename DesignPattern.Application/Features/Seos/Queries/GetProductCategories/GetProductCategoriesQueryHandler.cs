using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Queries.GetProductCategories;

public sealed class GetProductCategoriesQueryHandler
  : IRequestHandler<GetProductCategoriesQuery, Result<PagedResult<CategoryListItemResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetProductCategoriesQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<PagedResult<CategoryListItemResponse>>> Handle(
    GetProductCategoriesQuery request,
    CancellationToken cancellationToken)
  {
    var dto = request.DTO;

    var pagedResult = await _unitOfWork.ProductCategoryRepository.SearchCategoriesPagedAsync(
      nameSearchTerm: dto.Name,
      pageIndex: dto.PageIndex,
      pageSize: dto.PageSize,
      cancellationToken: cancellationToken
    );

    // Map từ domain read model sang application response DTO
    var responseItems = pagedResult.Items.Select(item => new CategoryListItemResponse
    {
      Id = item.Id.ToString(),
      Name = item.Name,
      Slug = item.Slug,
      Level = item.Level,
      ParentCategory = item.ParentCategoryId.HasValue
        ? new ParentCategoryInfo
        {
          Id = item.ParentCategoryId.Value.ToString(),
          Name = item.ParentCategoryName
        }
        : null
    }).ToList();

    var response = new PagedResult<CategoryListItemResponse>
    {
      Items = responseItems,
      TotalCount = pagedResult.TotalCount,
      PageIndex = pagedResult.PageIndex,
      PageSize = pagedResult.PageSize
    };

    return Result<PagedResult<CategoryListItemResponse>>.Success(response);
  }
}
