using Microsoft.AspNetCore.Mvc;
using MediatR;
using DesignPattern.Application.Features.DashBoard.General;
using DesignPattern.Application.Features.DashBoard.Advance;

namespace API.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
public sealed class DashboardController : ControllerBase
{
  private readonly IMediator _mediator;

  public DashboardController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Get general dashboard statistics (e.g. total users, total orders, total revenue)
  /// </summary>
  [HttpGet("general")]
  public async Task<IActionResult> GetGeneralDashboard(CancellationToken cancellationToken)
  {
    var query = new GeneralDashBoardQuery();
    var result = await _mediator.Send(query, cancellationToken);
    return Ok(ApiResponse.SuccessResult(result.Value, "Lấy thông tin thống kê thành công"));
  }
  /// <summary>
  /// Get advanced dashboard statistics with time filters (e.g. total orders, revenue by category
  /// and timeline)
  /// </summary>
  /// 
  [HttpGet("advance")]
  public async Task<IActionResult> GetAdvanceDashboard(
    [FromQuery] int? annual,
    [FromQuery] int? quarterly,
    [FromQuery] int? monthly,
    [FromQuery] int? weekly,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate,
    CancellationToken cancellationToken)
  {
    var query = new AdvanceDashBoardQuery(
      Annual: annual,
      Quarterly: quarterly,
      Monthly: monthly,
      Weekly: weekly,
      StartDate: startDate,
      EndDate: endDate);
    var result = await _mediator.Send(query, cancellationToken);
    return Ok(ApiResponse.SuccessResult(result.Value, "Lấy thông tin thống kê nâng cao thành công"));
  }


}