using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using FluentValidation;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Queries.GetProductCategories;

public record GetProductCategoriesDTO(
  int PageIndex = 1,
  int PageSize = 10,
  string? Name = null
);

public sealed record GetProductCategoriesQuery(GetProductCategoriesDTO DTO)
  : IRequest<Result<PagedResult<CategoryListItemResponse>>>;

public class GetProductCategoriesQueryValidator
  : AbstractValidator<GetProductCategoriesQuery>
{
  public GetProductCategoriesQueryValidator()
  {
    RuleFor(x => x.DTO.PageIndex)
      .GreaterThan(0).WithMessage("Page index must be greater than 0.");

    RuleFor(x => x.DTO.PageSize)
      .GreaterThan(0).WithMessage("Page size must be greater than 0.");

    RuleFor(x => x.DTO.Name)
      .MaximumLength(100).WithMessage("Name must be less than 100 characters.");
  }
}
