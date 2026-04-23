using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

namespace DesignPattern.Application.Features.DashBoard.General;

public record TopProductResponse
(
  Guid ProductId,
  string ProductName,
  int QuantitySold
);
public record DashboardGeneralResponse
(
  int TotalUsers,
  int NewUsersThisMonth,
  int TotalOrders,
  decimal TotalRevenue,
  List<TopProductResponse> TopSellingProducts
);

public record GeneralDashBoardQuery() : IRequest<Result<DashboardGeneralResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Admin, UserRole.Manager };
}
