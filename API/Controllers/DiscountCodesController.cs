using DesignPattern.Application.Features.DiscountCodes.Commands.CreateDiscountCode;
using DesignPattern.Application.Features.DiscountCodes.Commands.UpdateDiscountCode;
using DesignPattern.Application.Features.DiscountCodes.Queries.GetDiscountCodes;
using DesignPattern.Application.Features.DiscountCodes.Queries.CalculateDiscount;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/discount-codes")]
public class DiscountCodesController : ControllerBase
{
  private readonly IMediator _mediator;

  public DiscountCodesController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("/api/discount-codes")]
  public async Task<IActionResult> GetDiscountCodes(
    [FromQuery] string? searchTerm,
    [FromQuery] bool? isActive,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    var query = new GetDiscountCodesQuery(searchTerm, isActive, startDate, endDate, pageIndex, pageSize);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh sách mã giảm giá thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lấy danh sách mã giảm giá thành công"));
  }

  [HttpPost("/api/admin/discount-codes")]
  public async Task<IActionResult> CreateDiscountCode(
    [FromBody] CreateDiscountCodeDTO dto,
    CancellationToken cancellationToken = default)
  {
    var command = new CreateDiscountCodeCommand(dto);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is ConflictError)
    {
      return Conflict(ApiResponse.FailureResult(
        "Tạo mã giảm giá thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Tạo mã giảm giá thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return CreatedAtAction(
      null, null,
      ApiResponse.SuccessResult(
        result.Value,
        "Tạo mã giảm giá thành công"));
  }

  [HttpPatch("/api/admin/discount-codes/{id:guid}")]
  public async Task<IActionResult> UpdateDiscountCode(
    [FromRoute] Guid id,
    [FromBody] UpdateDiscountCodeDto request,
    CancellationToken cancellationToken = default)
  {
    var command = new UpdateDiscountCodeCommand(id, request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy mã giảm giá để cập nhật",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Cập nhật mã giảm giá thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Cập nhật mã giảm giá thành công"));
  }

  [HttpPost("calculate")]
  public async Task<IActionResult> CalculateDiscount(
    [FromBody] CalculateDiscountDto request,
    CancellationToken cancellationToken = default)
  {
    var query = new CalculateDiscountQuery(request);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy mã giảm giá",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure && result.Error is ConflictError)
    {
      return Conflict(ApiResponse.FailureResult(
        "Áp dụng mã giảm giá thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lỗi xử lý mã giảm giá",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Áp dụng mã giảm giá thành công"));
  }
}
