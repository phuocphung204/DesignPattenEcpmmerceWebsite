namespace DesignPattern.Domain.Errors;

public record Error(string Code, string Message)
{
  public static readonly Error None = new(string.Empty, string.Empty);
  public static readonly Error NullValue = new("Error.NullValue", "Value cannot be null.");

  public static Error NotFound(string code, string message) =>
      new NotFoundError(code, message);

  public static Error Validation(string code, string message) =>
      new ValidationError(code, message);

  public static Error Conflict(string code, string message) =>
      new ConflictError(code, message);

  public static Error Unauthorized(string code, string message) =>
      new UnauthorizedError(code, message);

  public static Error Failure(string code, string message) =>
      new(code, message);
}


public sealed record NotFoundError(string Code, string Message)
    : Error(Code, Message);

public sealed record ValidationError(string Code, string Message)
    : Error(Code, Message);

public sealed record ConflictError(string Code, string Message)
    : Error(Code, Message);

public sealed record UnauthorizedError(string Code, string Message)
    : Error(Code, Message);
