using System.Diagnostics;
using API.Errors;
using AutoMapper;
using DesignPattern.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using MongoDB.Driver;

namespace API.Middlewares;

public sealed class ExceptionMiddleware : IExceptionHandler
{
  private readonly ILogger<ExceptionMiddleware> _logger;

  public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
  {
    _logger = logger;
  }

  public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
  {
    (int statusCode, string errorCode, string message) = MapException(exception);

    var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
    if (statusCode >= StatusCodes.Status500InternalServerError)
    {
      _logger.LogError(exception,
        "Unhandled exception. TraceId: {TraceId}, ErrorCode: {ErrorCode}",
        traceId,
        errorCode);
    }
    else
    {
      _logger.LogWarning(exception,
        "Handled exception. TraceId: {TraceId}, ErrorCode: {ErrorCode}",
        traceId,
        errorCode);
    }

    httpContext.Response.StatusCode = statusCode;
    httpContext.Response.ContentType = "application/json";

    var response = ApiResponse.FailureResult(
      msg: message,
      errorCode: errorCode,
      errors: null);

    await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    return true;
  }

  private static (int StatusCode, string ErrorCode, string Message) MapException(Exception exception)
  {
    return exception switch
    {
      ValidationException validationEx => (
        StatusCodes.Status400BadRequest,
        ErrorCodes.Domain.Validation.Error,
        BuildValidationMessage(validationEx)),

      AuthenticationException authEx => (
        StatusCodes.Status401Unauthorized,
        ErrorCodes.Domain.Auth.NotAuthenticated,
        authEx.Message),

      AuthorizationException authorizationEx => (
        StatusCodes.Status403Forbidden,
        ErrorCodes.Domain.Auth.NotAuthorized,
        authorizationEx.Message),

      UnauthorizedAccessException unauthorizedEx => (
        StatusCodes.Status403Forbidden,
        ErrorCodes.Domain.Auth.NotAuthorized,
        unauthorizedEx.Message),

      ArgumentException argumentEx => (
        StatusCodes.Status400BadRequest,
        ErrorCodes.Domain.Validation.ArgumentInvalid,
        argumentEx.Message),

      FormatException formatEx => (
        StatusCodes.Status400BadRequest,
        ErrorCodes.Domain.Validation.FormatInvalid,
        formatEx.Message),

      NullReferenceException nullRefEx => (
        StatusCodes.Status500InternalServerError,
        ErrorCodes.System.NullReference,
        nullRefEx.Message),

      AutoMapperMappingException mappingEx => (
        StatusCodes.Status500InternalServerError,
        ErrorCodes.Infrastructure.Mapping.Error,
        mappingEx.Message),

      MongoWriteException { WriteError.Category: ServerErrorCategory.DuplicateKey } mongoDupEx => (
        StatusCodes.Status409Conflict,
        ErrorCodes.Infrastructure.Db.DuplicateKey,
        mongoDupEx.Message),

      MongoConnectionException mongoConnectionEx => (
        StatusCodes.Status503ServiceUnavailable,
        ErrorCodes.Infrastructure.Db.Unavailable,
        mongoConnectionEx.Message),

      TimeoutException timeoutEx => (
        StatusCodes.Status504GatewayTimeout,
        ErrorCodes.Infrastructure.Timeout,
        timeoutEx.Message),

      TaskCanceledException taskCanceledEx => (
        StatusCodes.Status504GatewayTimeout,
        ErrorCodes.Integration.Timeout,
        taskCanceledEx.Message),

      HttpRequestException httpRequestEx => (
        StatusCodes.Status502BadGateway,
        ErrorCodes.Integration.BadGateway,
        httpRequestEx.Message),

      MongoException mongoEx => (
        StatusCodes.Status500InternalServerError,
        ErrorCodes.Infrastructure.Db.Error,
        mongoEx.Message),

      _ => (
        StatusCodes.Status500InternalServerError,
        ErrorCodes.System.InternalError,
        exception.Message)
    };
  }

  private static string BuildValidationMessage(ValidationException exception)
  {
    var details = exception.Errors
      .Select(error => error.ErrorMessage)
      .Where(message => !string.IsNullOrWhiteSpace(message))
      .Distinct()
      .ToArray();

    if (details.Length == 0)
    {
      return exception.Message;
    }

    return string.Join("; ", details);
  }
}
