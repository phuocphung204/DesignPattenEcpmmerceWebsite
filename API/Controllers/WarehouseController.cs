using DesignPattern.Application.Features.Warehouses.Commands.CreateWarehouse;
using DesignPattern.Application.Features.Warehouses.Commands.DeleteWarehouse;
using DesignPattern.Application.Features.Warehouses.Commands.UpdateWarehouse;
using DesignPattern.Application.Features.Warehouses.Queries.GetPageWarehouse;
using DesignPattern.Application.Features.Warehouses.Queries.GetWarehouseById;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/warehouses")]
public sealed class WarehouseController : ControllerBase
{
  private readonly IMediator _mediator;

  public WarehouseController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("/api/admin/warehouses")]
  public async Task<IActionResult> GetWarehouses(
    [FromQuery] string? name,
    [FromQuery] string? address,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    var query = new GetPageWarehouseQuery
    {
      Name = name ?? string.Empty,
      Address = address ?? string.Empty,
      PageIndex = pageIndex,
      PageSize = pageSize
    };

    var result = await _mediator.Send(query, cancellationToken);

    return Ok(ApiResponse.SuccessResult(
      result,
      "Lay danh sach kho thanh cong"));
  }

  [HttpGet("/api/admin/warehouses/{id:guid}")]
  public async Task<IActionResult> GetWarehouseById(
    [FromRoute] Guid id,
    CancellationToken cancellationToken = default)
  {
    var query = new GetWarehouseByIdQuery(id);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Khong tim thay kho",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Lay kho that bai",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lay kho thanh cong"));
  }

  [HttpPost("/api/admin/warehouses")]
  public async Task<IActionResult> CreateWarehouse(
    [FromBody] CreateWarehouseDTO request,
    CancellationToken cancellationToken = default)
  {
    var command = new CreateWarehouseCommand(request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Tao kho that bai",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return CreatedAtAction(
      nameof(GetWarehouseById),
      new { id = result.Value.Id },
      ApiResponse.SuccessResult(result.Value, "Tao kho thanh cong"));
  }

  [HttpPatch("/api/admin/warehouses/{id:guid}")]
  public async Task<IActionResult> UpdateWarehouse(
    [FromRoute] Guid id,
    [FromBody] UpdateWarehouseDTO request,
    CancellationToken cancellationToken = default)
  {
    var command = new UpdateWarehouseCommand(id, request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Khong tim thay kho",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else
      {
        return BadRequest(ApiResponse.FailureResult(
          "Cap nhat kho that bai",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Cap nhat kho thanh cong"));
  }

  [HttpDelete("/api/admin/warehouses/{id:guid}")]
  public async Task<IActionResult> DeleteWarehouse(
    [FromRoute] Guid id,
    CancellationToken cancellationToken = default)
  {
    var command = new DeleteWarehouseCommand(id);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Khong tim thay kho",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is ConflictError)
      {
        return Conflict(ApiResponse.FailureResult(
          "Xoa kho that bai",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Xoa kho that bai",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      new { },
      "Xoa kho thanh cong"));
  }
}
