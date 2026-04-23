using Microsoft.AspNetCore.Mvc;
using MediatR;
using DesignPattern.Application.Features.Carts.Commands.AddCartItem;
using DesignPattern.Application.Features.Carts.Commands.UpdateCartItemQuantity;
using DesignPattern.Application.Features.Carts.Commands.DeleteCartItem;
using DesignPattern.Application.Features.Carts.Queries.GetCartByUserId;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Common;

namespace API.Controllers;

[ApiController]
[Route("api/carts")]
public sealed class CartsController : ControllerBase
{
  private readonly IMediator _mediator;

  public CartsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Get cart items for the current user
  /// </summary>
  [HttpGet]
  public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
  {
    var query = new GetCartByUserIdQuery();
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy giỏ hàng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      return BadRequest(ApiResponse.FailureResult(
        "Lấy giỏ hàng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    return Ok(ApiResponse.SuccessResult(result.Value, "Lấy giỏ hàng thành công"));
  }



  /// <summary>
  /// Add item to cart
  /// </summary>
  [HttpPost("items")]
  public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto, CancellationToken cancellationToken)
  {
    var request = new AddCartItemCommand(dto);
    var result = await _mediator.Send(request, cancellationToken);
    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Người dùng chưa đăng nhập",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy giỏ hàng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else
      {
        return BadRequest(ApiResponse.FailureResult(
          "Thêm sản phẩm vào giỏ hàng thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Thêm sản phẩm vào giỏ hàng thành công"));
  }

  /// <summary>
  /// Update item quantity in cart
  /// </summary>
  [HttpPatch("items/{productId:guid}")]
  public async Task<IActionResult> UpdateItemQuantity([FromRoute] Guid productId, [FromBody] UpdateQuantityDto dto, CancellationToken cancellationToken)
  {
    var command = new UpdateCartItemQuantityCommand(productId, dto);
    var result = await _mediator.Send(command, cancellationToken);
    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Người dùng chưa đăng nhập",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy giỏ hàng hoặc sản phẩm",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else
      {
        return BadRequest(ApiResponse.FailureResult(
          "Cập nhật số lượng sản phẩm thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Cập nhật số lượng sản phẩm thành công"));
  }

  /// <summary>
  /// Delete item from cart
  /// </summary>
  [HttpDelete("items/{productId:guid}")]
  public async Task<IActionResult> DeleteItem([FromRoute] Guid productId, CancellationToken cancellationToken)
  {
    var command = new DeleteCartItemCommand(productId);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Người dùng chưa đăng nhập",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy giỏ hàng hoặc sản phẩm",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else
      {
        return BadRequest(ApiResponse.FailureResult(
          "Xóa sản phẩm khỏi giỏ hàng thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Xóa sản phẩm khỏi giỏ hàng thành công"));
  }
}
