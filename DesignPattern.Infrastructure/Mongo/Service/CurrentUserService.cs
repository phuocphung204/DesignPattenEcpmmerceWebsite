using System.Security.Claims;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DesignPattern.Infrastructure.Mongo.Service;

public class CurrentUserService : ICurrentUserService
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CurrentUserService(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Guid UserId => Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

  public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

  public string? Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

  public UserRole Role =>
    Enum.Parse<UserRole>(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value!);

}
