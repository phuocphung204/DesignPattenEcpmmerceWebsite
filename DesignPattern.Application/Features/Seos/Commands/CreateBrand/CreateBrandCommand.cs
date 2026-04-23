using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.CreateBrand;

public record CreateBrandDTO(
    string Name,
    string? MetaDescription,
    string? Title,
    string? ParentBrandId
);

public record CreateBrandCommand(CreateBrandDTO dto) : IRequest<Result<BrandResponse>>;
