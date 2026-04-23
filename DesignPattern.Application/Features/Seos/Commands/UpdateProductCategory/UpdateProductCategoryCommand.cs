using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.UpdateProductCategory;

public record UpdateProductCategoryDTO(
  string? Name = null,
  string? MetaDescription = null,
  string? Title = null,
  string? ParentCategoryId = null
);

public record UpdateProductCategoryCommand(Guid Id, UpdateProductCategoryDTO dto)
  : IRequest<Result<ProductCategoryResponse>>;
