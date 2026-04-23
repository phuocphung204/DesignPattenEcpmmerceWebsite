using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

using DesignPattern.Application.Features.DiscountCodes.Shared;
using DesignPattern.Application.Abstractions;

namespace DesignPattern.Application.Features.DiscountCodes.Commands.CreateDiscountCode;

public record CreateDiscountCodeDTO(
  string Code,
  decimal MinimumOrderAmount,
  int UsageLimit,
  DateTime ExpirationDate,
  DiscountType Type,
  decimal? FixedAmount = null,
  decimal? Percent = null,
  decimal? MaximumDiscountAmount = null
);

public record CreateDiscountCodeCommand(CreateDiscountCodeDTO dto)
  : IRequest<Result<DiscountCodeResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}
