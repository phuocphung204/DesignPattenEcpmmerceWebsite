using DesignPattern.Application.Features.WarehouseItems.Commands.CreateItem;
using DesignPattern.Application.Features.WarehouseItems.Commands.DeleteItem;
using DesignPattern.Application.Features.WarehouseItems.Commands.ProccessAllocation;
using DesignPattern.Application.Features.WarehouseItems.Commands.UpdateItem;
using DesignPattern.Application.Features.WarehouseItems.Queries.GetItemById;
using DesignPattern.Application.Features.WarehouseItems.Queries.GetPage;
using DesignPattern.Application.Features.WarehouseItems.Queries.SearchWarehouseItems;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/warehouse-items")]
public sealed class WarehouseItemController : ControllerBase
{
  private readonly IMediator _mediator;

  public WarehouseItemController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("/api/admin/warehouse-items")]
  public async Task<IActionResult> GetPage(
    [FromQuery] Guid? warehouseId,
    [FromQuery] string? productName,
    [FromQuery] string? sku,
    [FromQuery] string? variantGroupId,
    [FromQuery] Guid? productId,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    var query = new GetWarehouseItemsPageQuery
    {
      WarehouseId = warehouseId,
      ProductName = productName,
      Sku = sku,
      VariantGroupId = variantGroupId,
      ProductId = productId,
      PageIndex = pageIndex,
      PageSize = pageSize
    };

    var result = await _mediator.Send(query, cancellationToken);

    return Ok(ApiResponse.SuccessResult(result, "Lay danh sach warehouse item thanh cong"));
  }

  [HttpGet("/api/admin/warehouse-items/{id:guid}")]
  public async Task<IActionResult> GetById(
    [FromRoute] Guid id,
    CancellationToken cancellationToken = default)
  {
    var query = new GetItemByIdQuery(id);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Khong tim thay warehouse item",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Lay warehouse item that bai",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    if (result.Value is null)
    {
      return NotFound(ApiResponse.FailureResult(
        "Khong tim thay warehouse item",
        WarehouseErrors.WarehouseItemNotFound.Code,
        SemicolonSeparatedListParser.Parse(WarehouseErrors.WarehouseItemNotFound.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Lay warehouse item thanh cong"));
  }

  [HttpPost("/api/admin/warehouse-items")]
  public async Task<IActionResult> Create(
    [FromBody] CreateItemDTO request,
    CancellationToken cancellationToken = default)
  {
    var command = new CreateItemCommand(request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Tao warehouse item that bai",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return CreatedAtAction(
      nameof(GetById),
      new { id = result.Value.Id },
      ApiResponse.SuccessResult(result.Value, "Tao warehouse item thanh cong"));
  }

  [HttpPatch("/api/admin/warehouse-items/{id:guid}")]
  public async Task<IActionResult> Update(
    [FromRoute] Guid id,
    [FromBody] UpdateItemDTO request,
    CancellationToken cancellationToken = default)
  {
    var command = new UpdateItemCommand(id, request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Khong tim thay warehouse item",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Cap nhat warehouse item that bai",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Cap nhat warehouse item thanh cong"));
  }

  [HttpDelete("/api/admin/warehouse-items/{id:guid}")]
  public async Task<IActionResult> Delete(
    [FromRoute] Guid id,
    CancellationToken cancellationToken = default)
  {
    var command = new DeleteItemCommand(id);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Khong tim thay warehouse item",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Xoa warehouse item that bai",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Xoa warehouse item thanh cong"));
  }

  [HttpGet("search")]
  public async Task<IActionResult> Search(
    [FromQuery] Guid? warehouseId,
    [FromQuery] string? productName,
    [FromQuery] string? sku,
    [FromQuery] string? variantGroupId,
    [FromQuery] Guid? productId,
    CancellationToken cancellationToken = default)
  {
    var query = new SearchWarehouseItemsQuery
    {
      WarehouseId = warehouseId,
      ProductName = productName,
      Sku = sku,
      VariantGroupId = variantGroupId,
      ProductId = productId
    };

    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Tim kiem warehouse item that bai",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Tim kiem warehouse item thanh cong"));
  }
}
