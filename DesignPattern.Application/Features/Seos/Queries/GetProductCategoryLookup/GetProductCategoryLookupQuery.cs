using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Queries.GetProductCategoryLookup;

public sealed record GetProductCategoryLookupQuery : IRequest<Result<List<CategoryListItemResponse>>>;
