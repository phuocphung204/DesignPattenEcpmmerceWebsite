using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Products.Shared;
using DesignPattern.Domain.Repositories;
namespace DesignPattern.Application.Features.Products.Queries.GetProducts;

// public record ProductSearchCriteria
// (
//   string? Name,
//   Guid? BrandId,
//   Guid? CategoryId,
//   string? VariantGroupId,
//   string? Sku,
//   decimal? MinRating,
//   decimal? MaxRating,
//   decimal? MinPrice,
//   decimal? MaxPrice,
//   SortOption? SortOption,
//   int PageIndex = 1,
//   int PageSize = 10
// );
public record GetProductsQuery(ProductSearchCriteria Criteria) : IRequest<Result<PagedResult<ProductOnSearchPageResponse>>>;

