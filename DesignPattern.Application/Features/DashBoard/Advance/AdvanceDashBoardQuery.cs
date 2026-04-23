using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

namespace DesignPattern.Application.Features.DashBoard.Advance;

public record AdvanceDashBoardQuery(
  int? Annual = null,
  int? Quarterly = null,
  int? Monthly = null,
  int? Weekly = null,
  DateTime? StartDate = null,
  DateTime? EndDate = null) : IRequest<Result<AdvanceDashboardResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Admin, UserRole.Manager };
}

public record AdvanceDashboardGeneralResponse(
  int TotalOrders,
  decimal TotalRevenue,
  decimal TotalProfit);

public record AdvanceDashboardCategoryResponse(
  string CategoryName,
  int TotalQuantity);

public record AdvanceDashboardTimelineResponse(
  int? Month,
  int? Week,
  int? Day,
  decimal TotalRevenue,
  decimal TotalProfit,
  int TotalProductsSold);

public record AdvanceDashboardResponse
(
  AdvanceDashboardGeneralResponse General,
  List<AdvanceDashboardCategoryResponse> Category,
  List<AdvanceDashboardTimelineResponse> Timeline
);