using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Exceptions;
using MediatR;

namespace DesignPattern.Application.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizeableRequest
{
  private readonly ICurrentUserService _currentUserService;

  public AuthorizationBehavior(ICurrentUserService currentUserService)
  {
    _currentUserService = currentUserService;
  }

  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
  {
    UserRole userRole = _currentUserService.Role;

    var isAuthorized = request.Roles.Contains(userRole);

    if (isAuthorized == false)
    {
      var errorResult = Error.Unauthorized(
        "Authorization.NotAuthorized",
        "You not authorized.");

      (bool shouldThrow, TResponse failureValue) = CreateFailureResponse.Create<TResponse>(errorResult);
      if (!shouldThrow)
      {
        return failureValue;
      }

      throw new AuthorizationException("You not authorized.");
    }

    return await next(cancellationToken);
  }
}
