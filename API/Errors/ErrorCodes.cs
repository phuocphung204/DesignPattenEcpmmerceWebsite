namespace API.Errors;

public static class ErrorCodes
{
  public static class System
  {
    public const string InternalError = "System.InternalError";
    public const string NullReference = "System.NullReference";
  }

  public static class Infrastructure
  {
    public static class Db
    {
      public const string Error = "Infrastructure.Db.Error";
      public const string DuplicateKey = "Infrastructure.Db.DuplicateKey";
      public const string Unavailable = "Infrastructure.Db.Unavailable";
    }

    public static class Mapping
    {
      public const string Error = "Infrastructure.Mapping.Error";
    }

    public const string Timeout = "Infrastructure.Timeout";
  }

  public static class Integration
  {
    public const string Timeout = "Integration.Timeout";
    public const string BadGateway = "Integration.BadGateway";
  }

  public static class Domain
  {
    public static class Auth
    {
      public const string NotAuthenticated = "Domain.Auth.NotAuthenticated";
      public const string NotAuthorized = "Domain.Auth.NotAuthorized";
    }

    public static class Validation
    {
      public const string Error = "Domain.Validation.Error";
      public const string ArgumentInvalid = "Domain.Validation.ArgumentInvalid";
      public const string FormatInvalid = "Domain.Validation.FormatInvalid";
    }
  }
}