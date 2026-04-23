using DesignPattern.Application.Features.Products.Shared;
using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdCommand(Guid Id) : IRequest<Result<ProductDetailResponse>>;
