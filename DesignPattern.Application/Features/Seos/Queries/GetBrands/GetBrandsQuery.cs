using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Common;
using FluentValidation;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Queries.GetBrands;

public record GetBrandsDTO(
    int PageIndex = 1,
    int PageSize = 10,
    string? Name = null
);

public sealed record GetBrandsQuery(GetBrandsDTO DTO) : IRequest<Result<PagedResult<BrandListItemResponse>>>;

public class GetBrandsQueryValidator : AbstractValidator<GetBrandsQuery>
{
  public GetBrandsQueryValidator()
  {
    RuleFor(x => x.DTO.PageIndex).GreaterThan(0).WithMessage("PageIndex phải lớn hơn 0");
    RuleFor(x => x.DTO.PageSize).GreaterThan(0).WithMessage("PageSize phải lớn hơn 0");
    RuleFor(x => x.DTO.Name).MaximumLength(100).WithMessage("Name phải nhỏ hơn 100 ký tự");
  }
}
