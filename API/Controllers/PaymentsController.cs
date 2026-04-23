

using Microsoft.AspNetCore.Mvc;
using MediatR;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;
using DesignPattern.Infrastructure.Payments.VNPay;
using DesignPattern.Application.Features.Payments.ProcessCallback;
using DesignPattern.Infrastructure.Payments.Momo;
using DesignPattern.Application.Features.Payments.CreatePayment;
using DesignPattern.Application.Abstractions.Payments;

namespace API.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly VnPayAdapter _vnPayAdapter;
  private readonly MomoAdapter _momoAdapter;

  public PaymentsController(IMediator mediator, VnPayAdapter vnPayAdapter, MomoAdapter momoAdapter)
  {
    _mediator = mediator;
    _vnPayAdapter = vnPayAdapter;
    _momoAdapter = momoAdapter;
  }

  /// <summary>
  /// Tạo thanh toán đơn hàng online qua cổng thanh toán được chọn.
  /// </summary>
  [HttpPost("create")]
  public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDTO dto, CancellationToken cancellationToken)
  {
    var command = new CreatePaymentCommand(dto);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Lấy địa chỉ thanh toán thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy đơn hàng để thanh toán",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is ConflictError)
      {
        return Conflict(ApiResponse.FailureResult(
          "Không thể tạo thanh toán cho đơn hàng hiện tại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Lấy địa chỉ thanh toán thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lấy địa chỉ thanh toán thành công"));
  }


  /// <summary>
  /// Nhận thông tin callback từ cổng thanh toán để cập nhật trạng thái thanh toán của đơn hàng.
  /// </summary>
  [HttpPost("mock/vnpay-callback")]
  public async Task<IActionResult> MockVnPayCallback([FromBody] VnPayResponse response, CancellationToken cancellationToken)
  {
    // Dùng adapter để chuyển đổi dữ liệu callback của VnPay thành đối tượng PaymentResult chung mà ứng dụng của mình hiểu
    var paymentResult = _vnPayAdapter.ProcessCallback(response);

    var command = new ProcessCallbackCommand(paymentResult);

    var result = await _mediator.Send(command, cancellationToken);
    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy đơn hàng để cập nhật thanh toán",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else if (result.Error is ConflictError)
      {
        return Conflict(ApiResponse.FailureResult(
          "Không thể cập nhật trạng thái thanh toán do trạng thái đơn hàng không hợp lệ",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else
      {
        return BadRequest(ApiResponse.FailureResult(
          "Cập nhật trạng thái thanh toán thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
    }
    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Xác nhận đơn hàng thành công"));
  }

  /// <summary>
  /// Nhận thông tin callback từ cổng thanh toán để cập nhật trạng thái thanh toán của đơn hàng.
  /// </summary>
  [HttpPost("mock/momo-callback")]
  public async Task<IActionResult> MockMomoCallback([FromBody] MoMoResponse response, CancellationToken cancellationToken)
  {
    // Dùng adapter để chuyển đổi dữ liệu callback của Momo thành đối tượng PaymentResult chung mà ứng dụng của mình hiểu
    var paymentResult = _momoAdapter.ProcessCallback(response);

    var command = new ProcessCallbackCommand(paymentResult);

    var result = await _mediator.Send(command, cancellationToken);
    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy đơn hàng để cập nhật thanh toán",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else if (result.Error is ConflictError)
      {
        return Conflict(ApiResponse.FailureResult(
          "Không thể cập nhật trạng thái thanh toán do trạng thái đơn hàng không hợp lệ",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else
      {
        return BadRequest(ApiResponse.FailureResult(
          "Cập nhật trạng thái thanh toán thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
    }
    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Xác nhận đơn hàng thành công"));
  }
}