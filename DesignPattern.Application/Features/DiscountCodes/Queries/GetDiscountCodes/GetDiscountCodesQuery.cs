using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.DiscountCodes.Queries.GetDiscountCodes;

public record GetDiscountCodesQuery(
    string? SearchTerm,
    bool? IsActive,
    DateTime? StartDate,
    DateTime? EndDate,
    int PageIndex = 1,
    int PageSize = 10
) : IRequest<Result<PagedResult<DiscountCodeReadModel>>>
{ }
