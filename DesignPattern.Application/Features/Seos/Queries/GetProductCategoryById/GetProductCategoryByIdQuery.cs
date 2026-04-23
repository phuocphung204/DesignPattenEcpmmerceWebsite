using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Queries.GetProductCategoryById;

public sealed record GetProductCategoryByIdQuery(Guid Id) : IRequest<Result<ProductCategoryResponse>>;
