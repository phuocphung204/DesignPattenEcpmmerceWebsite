using MediatR;
using Microsoft.AspNetCore.Mvc;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;
using DesignPattern.Application.Features.Ratings.Commands.CreateRating;
using DesignPattern.Application.Features.Ratings.Commands.UpdateRating;
using DesignPattern.Application.Features.Ratings.Commands.DeleteRating;
using DesignPattern.Application.Features.Ratings.Queries.GetRatingById;
using DesignPattern.Application.Features.Ratings.Queries.GetRatingsByProductId;
using DesignPattern.Application.Features.Ratings.Shared;

namespace API.Controllers;

[ApiController]
[Route("api/ratings")]
public sealed class RatingsController : ControllerBase
{
  private readonly IMediator _mediator;

  public RatingsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("/api/admin/ratings/{id:guid}")]
  public async Task<IActionResult> GetRatingById([FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var query = new GetRatingByIdQuery(id);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy đánh giá",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lấy thông tin đánh giá thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Lấy thông tin đánh giá thành công"));
  }

  [HttpGet("products/{productId:guid}")]
  public async Task<IActionResult> GetRatingsByProductId([FromRoute] Guid productId, CancellationToken cancellationToken)
  {
    var query = new GetRatingsByProductIdQuery(productId);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh sách đánh giá theo sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value ?? new List<RatingResponse>(), "Lấy danh sách đánh giá theo sản phẩm thành công"));
  }

  [HttpPost]
  public async Task<IActionResult> CreateRating([FromBody] CreateRatingDto dto, CancellationToken cancellationToken)
  {
    var command = new CreateRatingCommand(dto);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is UnauthorizedError)
    {
      return Unauthorized(ApiResponse.FailureResult(
        "Người dùng chưa đăng nhập",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy sản phẩm để đánh giá",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Tạo đánh giá thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return CreatedAtAction(
      nameof(GetRatingById),
      new { id = result.Value.Id },
      ApiResponse.SuccessResult(result.Value, "Tạo đánh giá thành công"));
  }

  [HttpPatch("{id:guid}")]
  public async Task<IActionResult> UpdateRating([FromRoute] Guid id, [FromBody] UpdateRatingDTO dto, CancellationToken cancellationToken)
  {
    var command = new UpdateRatingCommand(id, dto);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Người dùng không có quyền cập nhật đánh giá này",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy đánh giá để cập nhật",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else
      {
        return BadRequest(ApiResponse.FailureResult(
          "Cập nhật đánh giá thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
    }
    return Ok(ApiResponse.SuccessResult(result.Value, "Cập nhật đánh giá thành công"));
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var command = new DeleteRatingCommand(id);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is UnauthorizedError)
    {
      return Unauthorized(ApiResponse.FailureResult(
        "Người dùng không có quyền xóa đánh giá này",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy đánh giá để xóa",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Xóa đánh giá thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(new { }, "Xóa đánh giá thành công"));
  }
}