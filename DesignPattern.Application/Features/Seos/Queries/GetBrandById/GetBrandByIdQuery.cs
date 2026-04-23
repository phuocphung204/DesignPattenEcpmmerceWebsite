using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Queries.GetBrandById;

public record GetBrandByIdQuery(Guid Id) : IRequest<Result<BrandResponse>>;
