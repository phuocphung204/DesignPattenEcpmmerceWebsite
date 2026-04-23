using Microsoft.AspNetCore.Mvc;
using MediatR;
using DesignPattern.Application.Features.Users.Commands.RegisterUser;
using DesignPattern.Application.Features.Users.Commands.UpdateUserProfile;
using DesignPattern.Application.Features.Users.Commands.CreateUserAddress;
using DesignPattern.Application.Features.Users.Commands.UpdateUserAddress;
using DesignPattern.Application.Features.Users.Commands.DeleteUserAddress;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;
using DesignPattern.Application.Features.Users.Queries.GetUserProfile;
using DesignPattern.Application.Features.Users.Queries.GetUsers;
using DesignPattern.Application.Features.Users.Queries.GetAddresses;

namespace API.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
  private readonly IMediator _mediator;

  public UsersController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("/api/users/me")]
  public async Task<IActionResult> GetUserProfile(CancellationToken cancellationToken)
  {
    var query = new GetUserProfileQuery();
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Không có quyền truy cập thông tin người dùng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      else if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy người dùng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }
      return BadRequest(ApiResponse.FailureResult(
        "Lấy thông tin người dùng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Lấy thông tin người dùng thành công"));
  }

  [HttpGet("/api/admin/users")]
  [EndpointDescription("Lấy danh sách người dùng")]
  public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
  {
    var query = new GetUsersQuery();
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Không có quyền truy cập danh sách người dùng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh sách người dùng thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lấy danh sách người dùng thành công"));
  }

  [HttpGet("me/addresses")]
  [EndpointDescription("Lấy danh sách địa chỉ của người dùng đang đăng nhập")]
  public async Task<IActionResult> GetAddresses(CancellationToken cancellationToken)
  {
    var query = new GetAddressesQuery();
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Lấy danh sách địa chỉ thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy người dùng",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh sách địa chỉ thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Lấy danh sách địa chỉ thành công"));
  }

  [HttpPost("registry")]
  public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDTO request, CancellationToken cancellationToken)
  {
    var command = new RegisterUserCommand(request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is ConflictError)
    {
      return Conflict(ApiResponse.FailureResult(
        "Đăng ký thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Đăng ký thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(new { }, "Đăng ký thành công"));
  }

  /// <summary>
  /// Cập nhật thông tin cá nhân của người dùng (Họ tên, Email, Ảnh đại diện, Địa chỉ mặc định)
  /// </summary>
  /// <param name="request">Thông tin cần cập nhật</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpPatch("profile")]
  [EndpointDescription("Cập nhật thông tin cá nhân người dùng (FullName, Email, AvatarLink, DefaultAddressId)")]
  public async Task<IActionResult> UpdateUserProfile(
    [FromBody] UpdateUserProfileDTO request,
    CancellationToken cancellationToken)
  {
    var command = new UpdateUserProfileCommand(request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Cập nhật thông tin cá nhân thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is NotFoundError)
      {
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy người dùng để cập nhật",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is ConflictError)
      {
        return Conflict(ApiResponse.FailureResult(
          "Cập nhật thông tin cá nhân thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      if (result.Error is ValidationError)
      {
        return BadRequest(ApiResponse.FailureResult(
          "Cập nhật thông tin cá nhân thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Cập nhật thông tin cá nhân thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Cập nhật thông tin cá nhân thành công"));
  }

  [HttpPost("addresses")]
  [EndpointDescription("Tạo địa chỉ mới cho người dùng đang đăng nhập")]
  public async Task<IActionResult> CreateUserAddress(
    [FromBody] CreateUserAddressDTO request,
    CancellationToken cancellationToken)
  {
    var command = new CreateUserAddressCommand(request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is UnauthorizedError)
    {
      return Unauthorized(ApiResponse.FailureResult(
        "Tạo địa chỉ thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy người dùng để tạo địa chỉ",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure && result.Error is ValidationError)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Tạo địa chỉ thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure && result.Error is ConflictError)
    {
      return Conflict(ApiResponse.FailureResult(
        "Tạo địa chỉ thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Tạo địa chỉ thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Tạo địa chỉ thành công"));
  }

  [HttpPatch("addresses/{addressId:guid}")]
  [EndpointDescription("Cập nhật địa chỉ của người dùng đang đăng nhập")]
  public async Task<IActionResult> UpdateUserAddress(
    [FromRoute] Guid addressId,
    [FromBody] UpdateUserAddressDTO request,
    CancellationToken cancellationToken)
  {
    var command = new UpdateUserAddressCommand(request, addressId);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
        return Unauthorized(ApiResponse.FailureResult(
          "Cập nhật địa chỉ thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));

      if (result.Error is NotFoundError)
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy người dùng để cập nhật địa chỉ",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));

      if (result.Error is ConflictError)
        return Conflict(ApiResponse.FailureResult(
          "Cập nhật địa chỉ thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));

      if (result.Error is ValidationError)
        return BadRequest(ApiResponse.FailureResult(
          "Cập nhật địa chỉ thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));

      return BadRequest(ApiResponse.FailureResult(
        "Cập nhật địa chỉ thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Cập nhật địa chỉ thành công"));
  }

  [HttpDelete("addresses/{addressId:guid}")]
  [EndpointDescription("Xóa địa chỉ của người dùng đang đăng nhập")]
  public async Task<IActionResult> DeleteUserAddress(
    [FromRoute] Guid addressId,
    CancellationToken cancellationToken)
  {
    var command = new DeleteUserAddressCommand(addressId);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
        return Unauthorized(ApiResponse.FailureResult(
          "Xóa địa chỉ thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));

      if (result.Error is NotFoundError)
        return NotFound(ApiResponse.FailureResult(
          "Không tìm thấy người dùng để xóa địa chỉ",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));

      if (result.Error is ValidationError)
        return BadRequest(ApiResponse.FailureResult(
          "Xóa địa chỉ thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));

      if (result.Error is ConflictError)
        return Conflict(ApiResponse.FailureResult(
          "Xóa địa chỉ thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));

      return BadRequest(ApiResponse.FailureResult(
        "Xóa địa chỉ thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(
      new { },
      "Xóa địa chỉ thành công"));
  }

}
