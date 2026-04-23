using DesignPattern.Domain.Common;
using MediatR;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.DiscountCodes.Queries.CalculateDiscount;

public record CalculateDiscountDto(
  string Code,
  decimal SubTotal
);

public sealed class CalculateDiscountResponse
{
  public decimal OriginalAmount { get; init; }
  public decimal DiscountAmount { get; init; }
  public decimal FinalAmount { get; init; }
  public string DiscountType { get; init; }
  public decimal? Percent { get; init; }
  public decimal? Amount { get; init; }


  public CalculateDiscountResponse() { }

}

public record CalculateDiscountQuery(
  CalculateDiscountDto Dto
) : IRequest<Result<CalculateDiscountResponse>>
{ }
