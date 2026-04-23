using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Products.Shared;

namespace DesignPattern.Application.Features.Products.Queries.GetProductByGroupId;

public record GetProductByGroupIdQuery(string VariantGroupId)
  : IRequest<Result<List<ProductOnGroupResponse>>>;