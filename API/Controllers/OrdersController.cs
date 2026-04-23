using Microsoft.AspNetCore.Mvc;
using MediatR;
using DesignPattern.Application.Features.Orders.Commands.CancelOrder;
using DesignPattern.Application.Features.Orders.Commands.ConfirmOrder;
using DesignPattern.Application.Features.Orders.Commands.CreateOrder;
using DesignPattern.Application.Features.Orders.Commands.UpdateOrderStatus;
using DesignPattern.Application.Features.Orders.Queries.GetOrderById;
using DesignPattern.Application.Features.Orders.Queries.GetOrdersByUserId;
using DesignPattern.Application.Features.Orders.Queries.SearchListOrder;
using DesignPattern.Application.Features.Orders.Queries.GetAllOrders;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;

namespace API.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
  private readonly IMediator _mediator;
  public OrdersController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Lấy danh sách đơn hàng của người dùng đang đăng nhập.
  /// </summary>
  [HttpGet]
  [EndpointDescription("Lấy danh sách đơn hàng của người dùng")]
  public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
  {
    var query = new GetOrdersByUserIdQuery();
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Lấy danh sách đơn hàng thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh sách đơn hàng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lấy danh sách đơn hàng thành công"));
  }

  /// <summary>
  /// Lấy chi tiết đơn hàng theo id cho người dùng đang đăng nhập.
  /// </summary>
  [HttpGet("{orderId:guid}")]
  [EndpointDescription("Lấy chi tiết đơn hàng theo id")]
  public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId, CancellationToken cancellationToken)
  {
    var query = new GetOrderByIdQuery(orderId);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Lấy chi tiết đơn hàng thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy đơn hàng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Lấy chi tiết đơn hàng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lấy chi tiết đơn hàng thành công"));
  }

  /// <summary>
  /// Tạo đơn hàng mới cho người dùng đang đăng nhập.
  /// </summary>
  [HttpPost]
  [EndpointDescription("Tạo đơn hàng mới")]
  public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request, CancellationToken cancellationToken)
  {
    var command = new CreateOrderCommand(request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Tạo đơn hàng thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy dữ liệu để tạo đơn hàng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is ConflictError)
      {
        return Conflict(ApiResponse.FailureResult(
          "Không thể tạo đơn hàng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Tạo đơn hàng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return CreatedAtAction(
      nameof(GetOrderById),
      new { orderId = result.Value.Id },
      ApiResponse.SuccessResult(
        result.Value,
        "Tạo đơn hàng thành công"));
  }


  /// <summary>
  /// Lấy danh sách tất cả đơn hàng (dành cho manager/admin).
  /// </summary>
  [HttpGet("/api/admin/orders")]
  [EndpointDescription("Lấy danh sách tất cả đơn hàng")]
  public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken)
  {
    var query = new GetAllOrdersQuery();
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Lấy danh sách đơn hàng thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh sách đơn hàng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lấy danh sách đơn hàng thành công"));
  }

  /// <summary>
  /// Xác nhận đơn hàng (manager).
  /// </summary>
  [HttpPatch("/api/admin/orders/{orderId:guid}/confirm")]
  [EndpointDescription("Xác nhận đơn hàng")]
  public async Task<IActionResult> ConfirmOrder([FromRoute] Guid orderId, CancellationToken cancellationToken)
  {
    var command = new ConfirmOrderCommand(orderId);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Xác nhận đơn hàng thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy đơn hàng để xác nhận",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is ConflictError)
      {
        return Conflict(ApiResponse.FailureResult(
          "Không thể xác nhận đơn hàng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Xác nhận đơn hàng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Xác nhận đơn hàng thành công"));
  }

  /// <summary>
  /// Cập nhật trạng thái đơn hàng (manager).
  /// </summary>
  [HttpPatch("/api/admin/orders/{orderId:guid}/status")]
  [EndpointDescription("Cập nhật trạng thái đơn hàng")]
  public async Task<IActionResult> UpdateOrderStatus(
    [FromRoute] Guid orderId,
    [FromBody] UpdateOrderStatusDTO request,
    CancellationToken cancellationToken)
  {
    var command = new UpdateOrderStatusCommand(orderId, request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Cập nhật trạng thái đơn hàng thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy đơn hàng để cập nhật trạng thái",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is ConflictError)
      {
        return Conflict(ApiResponse.FailureResult(
          "Không thể cập nhật trạng thái đơn hàng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Cập nhật trạng thái đơn hàng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Cập nhật trạng thái đơn hàng thành công"));
  }

  /// <summary>
  /// Tìm kiếm danh sách đơn hàng theo khoảng thời gian, id gần đúng và phân trang (manager/admin).
  /// </summary>
  [HttpGet("/api/admin/orders/search")]
  [EndpointDescription("Tìm kiếm danh sách đơn hàng có phân trang")]
  public async Task<IActionResult> SearchOrders(
    [FromQuery] DateTime start,
    [FromQuery] DateTime end,
    [FromQuery] string? idSearch,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    var query = new SearchListOrderQuery(start, end, idSearch, pageIndex, pageSize);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Tìm kiếm danh sách đơn hàng thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Tìm kiếm danh sách đơn hàng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Tìm kiếm danh sách đơn hàng thành công"));
  }

  /// <summary>
  /// Hủy đơn hàng của người dùng đang đăng nhập.
  /// </summary>
  [HttpPatch("{orderId:guid}/cancel")]
  [EndpointDescription("Hủy đơn hàng")]
  public async Task<IActionResult> CancelOrder(
    [FromRoute] Guid orderId,
    [FromBody] CancelOrderCommandDto request,
    CancellationToken cancellationToken)
  {
    var command = new CancelOrderCommand(orderId, request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Hủy đơn hàng thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy đơn hàng để hủy",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is ConflictError)
      {
        return Conflict(ApiResponse.FailureResult(
          "Không thể hủy đơn hàng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Hủy đơn hàng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Hủy đơn hàng thành công"));
  }
}
