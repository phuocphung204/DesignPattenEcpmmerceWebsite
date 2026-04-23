using Microsoft.AspNetCore.Mvc;
using MediatR;
using DesignPattern.Application.Features.Seos.Commands.CreateBrand;
using DesignPattern.Application.Features.Seos.Commands.UpdateBrand;
using DesignPattern.Application.Features.Seos.Commands.DeleteBrand;
using DesignPattern.Application.Features.Seos.Queries.GetBrands;
using DesignPattern.Application.Features.Seos.Queries.GetBrandById;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;

namespace API.Controllers;

[ApiController]
[Route("api/brands")]
public sealed class BrandsController : ControllerBase
{
  private readonly IMediator _mediator;

  public BrandsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("/api/admin/brands")]
  public async Task<IActionResult> GetBrands([FromQuery] GetBrandsDTO dto, CancellationToken cancellationToken)
  {
    var query = new GetBrandsQuery(dto);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh sách thương hiệu thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Lấy danh sách thương hiệu thành công"));
  }

  [HttpGet("/api/admin/brands/{id:guid}")]
  public async Task<IActionResult> GetBrandById([FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var query = new GetBrandByIdQuery(id);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy thương hiệu",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
          "Lấy thông tin thương hiệu thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Lấy thông tin thương hiệu thành công"));
  }

  [HttpPost("/api/admin/brands")]
  public async Task<IActionResult> CreateBrand([FromBody] CreateBrandDTO request, CancellationToken cancellationToken)
  {
    var command = new CreateBrandCommand(request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Tạo thương hiệu thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return CreatedAtAction(
      nameof(GetBrandById),
      new { id = result.Value.Id },
      ApiResponse.SuccessResult(result.Value, "Tạo thương hiệu thành công"));
  }

  [HttpPatch("/api/admin/brands/{id:guid}")]
  public async Task<IActionResult> UpdateBrand([FromRoute] Guid id, [FromBody] UpdateBrandDTO request, CancellationToken cancellationToken)
  {
    var command = new UpdateBrandCommand(id, request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy thương hiệu để cập nhật",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
          "Cập nhật thương hiệu thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Cập nhật thương hiệu thành công"));
  }

  [HttpDelete("/api/admin/brands/{id:guid}")]
  public async Task<IActionResult> DeleteBrand([FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var command = new DeleteBrandCommand(id);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy thương hiệu để xóa",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure && result.Error is ConflictError)
    {
      return Conflict(ApiResponse.FailureResult(
        "Xóa thương hiệu thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Xóa thương hiệu thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(new { }, "Xóa thương hiệu thành công"));
  }
}
