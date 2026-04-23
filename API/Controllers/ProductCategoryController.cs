using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DesignPattern.Application.Features.Seos.Commands.CreateCategory;
using DesignPattern.Application.Features.Seos.Queries.GetProductCategoryById;
using DesignPattern.Application.Helpers;
using DesignPattern.Application.Features.Seos.Queries.GetProductCategories;
using DesignPattern.Application.Features.Seos.Queries.GetProductCategoryLookup;
using DesignPattern.Application.Features.Seos.Commands.DeleteProductCategory;
using DesignPattern.Application.Features.Seos.Commands.UpdateProductCategory;
using DesignPattern.Domain.Errors;

namespace API.Controllers;

[ApiController]
[Route("api/product-categories")]
public sealed class ProductCategoryController : ControllerBase
{
  private readonly IMediator _mediator;

  public ProductCategoryController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Lấy danh sách danh mục sản phẩm, hỗ trợ tìm kiếm và phân trang.
  /// </summary>
  /// <param name="dto">Tham số tìm kiếm và phân trang (name, pageIndex, pageSize)</param>
  /// <param name="cancellationToken"></param>
  /// <returns>Danh sách danh mục sản phẩm phân trang</returns>
  [HttpGet("/api/admin/product-categories")]
  [EndpointDescription("Lấy danh sách danh mục sản phẩm (phân trang, tìm kiếm theo tên)")]
  public async Task<IActionResult> GetProductCategories(
    [FromQuery] GetProductCategoriesDTO dto,
    CancellationToken cancellationToken)
  {
    var query = new GetProductCategoriesQuery(dto);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh mục sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lấy danh mục sản phẩm thành công"));
  }

  /// <summary>
  /// Lấy danh sách lookup danh mục sản phẩm
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet("lookup")]
  [EndpointDescription("Lấy danh sách lookup danh mục sản phẩm, " +
  "dùng cho Dropdown Search Suggestion (Gợi ý tìm kiếm)")]
  public async Task<IActionResult> GetProductCategoryLookup(CancellationToken cancellationToken)
  {
    var query = new GetProductCategoryLookupQuery();
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh sách lookup danh mục thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lấy danh sách lookup danh mục thành công"));
  }

  /// <summary>
  /// Lấy danh mục sản phẩm theo ID
  /// </summary>
  /// <param name="id">ID của danh mục sản phẩm</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet("/api/admin/product-categories/{id:guid}")]
  public async Task<IActionResult> GetProductCategoryById([FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var query = new GetProductCategoryByIdQuery(id);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy danh mục sản phẩm",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh mục sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lấy danh mục sản phẩm thành công"));
  }

  [HttpPost("/api/admin/product-categories")]
  public async Task<IActionResult> CreateProductCategory([FromBody] CreateProductCategoryDTO request, CancellationToken cancellationToken)
  {
    var command = new CreateProductCategoryCommand(request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Tạo danh mục sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return CreatedAtAction(
      nameof(GetProductCategoryById),
      new { id = result.Value.Id },
      ApiResponse.SuccessResult(
        result.Value,
        "Tạo danh mục sản phẩm thành công"));
  }

  /// <summary>
  /// Xóa danh mục sản phẩm theo ID
  /// </summary>
  /// <param name="id">ID của danh mục sản phẩm</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpDelete("/api/admin/product-categories/{id:guid}")]
  [EndpointDescription("Xóa danh mục sản phẩm theo ID")]
  public async Task<IActionResult> DeleteProductCategory([FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var command = new DeleteProductCategoryCommand(id);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy danh mục sản phẩm để xóa",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy danh mục sản phẩm để xóa",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Xóa danh mục sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      new { },
      "Xóa danh mục sản phẩm thành công"));
  }

  [HttpPatch("/api/admin/product-categories/{id:guid}")]
  [EndpointDescription("Cập nhật danh mục sản phẩm theo ID")]
  public async Task<IActionResult> UpdateProductCategory([FromRoute] Guid id, [FromBody] UpdateProductCategoryDTO request, CancellationToken cancellationToken)
  {
    var command = new UpdateProductCategoryCommand(id, request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy danh mục sản phẩm để cập nhật",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Cập nhật danh mục sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Cập nhật danh mục sản phẩm thành công"));
  }

}
