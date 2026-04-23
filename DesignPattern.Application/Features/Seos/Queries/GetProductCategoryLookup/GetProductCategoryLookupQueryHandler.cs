using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Queries.GetProductCategoryLookup;

public sealed class GetProductCategoryLookupQueryHandler
  : IRequestHandler<GetProductCategoryLookupQuery, Result<List<CategoryListItemResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetProductCategoryLookupQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<List<CategoryListItemResponse>>> Handle(
    GetProductCategoryLookupQuery request,
    CancellationToken cancellationToken
  )
  {
    var pagedResult = await _unitOfWork.ProductCategoryRepository.SearchCategoriesPagedAsync(
      nameSearchTerm: null,
      pageIndex: 1,
      pageSize: 20,
      cancellationToken: cancellationToken
    );

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

    return responseItems;
  }
}
