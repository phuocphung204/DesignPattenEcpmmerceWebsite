using Microsoft.AspNetCore.Mvc;
using MediatR;
using DesignPattern.Application.Features.Users.Commands.LoginCommand;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly JwtService _jwtService;

  public AuthController(IMediator mediator, JwtService jwtService)
  {
    _mediator = mediator;
    _jwtService = jwtService;
  }

  /// <summary>
  /// Đăng nhập người dùng vào hệ thống.
  /// </summary>
  /// <param name="command">Thông tin đăng nhập (Email, Password)</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpPost("login")]
  [EndpointDescription("Đăng nhập người dùng")]
  public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
      if (result.Error is UnauthorizedError)
      {
        return Unauthorized(ApiResponse.FailureResult(
          "Đăng nhập thất bại",
          result.Error.Code,
          SemicolonSeparatedListParser.Parse(result.Error.Message)));
      }

      return BadRequest(ApiResponse.FailureResult(
        "Đăng nhập thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    result.Value.Token = _jwtService.GenerateToken(result.Value.UserId, result.Value.Email, result.Value.Role);

    return Ok(ApiResponse.SuccessResult(
      result.Value,
      "Đăng nhập thành công"));
  }

  [HttpHead("validate-token")]
  public IActionResult ValidateToken()
  {
    var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[1];

    // Kiểm tra xem đã giải mã thành công chưa
    var isAuth = User.Identity?.IsAuthenticated;

    // Lấy userId (từ ClaimTypes.NameIdentifier)
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    // Lấy email (từ ClaimTypes.Email)
    var email = User.FindFirstValue(ClaimTypes.Email);

    Console.WriteLine($">>> Token: {token}");
    Console.WriteLine($">>> Is Valid: {isAuth}");
    Console.WriteLine($">>> User Id: {userId}");
    Console.WriteLine($">>> Email: {email}");

    if (_jwtService.isValidToken(token!))
      return Ok();
    return BadRequest();
  }
}
