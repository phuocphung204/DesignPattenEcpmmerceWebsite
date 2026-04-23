using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.UpdateBrand;

public record UpdateBrandDTO(
    string? Name,
    string? MetaDescription,
    string? Title,
    string? ParentBrandId
);

public record UpdateBrandCommand(Guid Id, UpdateBrandDTO dto) : IRequest<Result<BrandResponse>>;
