using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.CreateCategory;

public record CreateProductCategoryDTO(
  string Name,
  string MetaDescription,
  string Title,
  string? ParentCategoryId
);

public record CreateProductCategoryCommand(CreateProductCategoryDTO dto)
  : IRequest<Result<ProductCategoryResponse>>;
