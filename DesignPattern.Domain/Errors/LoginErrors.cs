namespace DesignPattern.Domain.Errors;

public static class LoginErrors
{
  public static readonly Error EmailUnregistered =
    Error.Unauthorized(
      "Login.EmailUnregistered",
      "Email is not registered.");

  public static readonly Error InvalidCredentials =
    Error.Unauthorized(
      "Login.InvalidCredentials",
      "Invalid email or password.");

  public static readonly Error UserInActive =
    Error.Unauthorized(
      "Login.UserInActive",
      "User is inactive.");

  public static readonly Error UserBanned =
    Error.Unauthorized(
      "Login.UserBanned",
      "User is banned.");
}
