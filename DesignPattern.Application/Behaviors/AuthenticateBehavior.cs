using System.Security.Authentication;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Errors;
using MediatR;

namespace DesignPattern.Application.Behaviors;

public class UserContextBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequiresUserContext // Chỉ áp dụng cho các Request cần User
{
  private readonly ICurrentUserService _currentUserService;

  public UserContextBehavior(ICurrentUserService currentUserService)
  {
    _currentUserService = currentUserService;
  }

  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
  {
    // 1. Kiểm tra xem user đã đăng nhập chưa
    if (!_currentUserService.IsAuthenticated)
    {
      var errorResult = Error.Unauthorized(
        "Login.NotAuthenticated",
        "You not authenticated.");

      (bool shouldThrow, TResponse failureValue) = CreateFailureResponse.Create<TResponse>(errorResult);
      if (!shouldThrow)
      {
        return failureValue;
      }

      throw new AuthenticationException("You not authenticated.");
    }

    // 2. Bơm UserId từ claims vào request trước khi vào handler
    request.UserId = _currentUserService.UserId;

    return await next(cancellationToken);
  }
}
