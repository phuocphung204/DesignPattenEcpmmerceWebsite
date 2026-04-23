using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Application.Features.Products.Shared;
namespace DesignPattern.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductOnSearchPageResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;
  public GetProductsQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<PagedResult<ProductOnSearchPageResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
  {
    var criteria = request.Criteria;

    var pagedResult = await _unitOfWork.ProductRepository.SearchProductsPagedAsync(criteria, cancellationToken: cancellationToken);

    var products = pagedResult.Items.Select(p => new ProductOnSearchPageResponse(
      Id: p.Id,
      Name: p.Name,
      Image: p.Image,
      SellingPrice: p.SellingPrice,
      SoldQuantity: p.SoldQuantity,
      Rating: p.Rating,
      ShortDescription: p.ShortDescription
    )).ToList();
    var newPagedResult = new PagedResult<ProductOnSearchPageResponse>
    {
      Items = products,
      TotalCount = pagedResult.TotalCount,
      PageIndex = pagedResult.PageIndex,
      PageSize = pagedResult.PageSize,
    };

    return newPagedResult;
  }
}