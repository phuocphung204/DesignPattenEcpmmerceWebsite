using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.DiscountCodes.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

namespace DesignPattern.Application.Features.DiscountCodes.Commands.UpdateDiscountCode;

public record UpdateDiscountCodeDto(
  DateTime? ExpirationDate,
  bool? IsActive,
  int? UsageLimit,
  decimal MinimumOrderAmount,
  decimal? FixedAmount,
  decimal? Percent,
  decimal? MaximumDiscountAmount

);

public record UpdateDiscountCodeCommand(
  Guid Id,
  UpdateDiscountCodeDto dto
) : IRequest<Result<DiscountCodeResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}
