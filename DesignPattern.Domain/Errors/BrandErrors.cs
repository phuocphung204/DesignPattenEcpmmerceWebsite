using DesignPattern.Domain.Common;

namespace DesignPattern.Domain.Errors;

public static class BrandErrors
{
  public static readonly Error ParentBrandNotFound = Error.NotFound(
      "Infrastructure.Brand.ParentBrandNotFound",
      "The parent brand with the specified identifier was not found.");

  public static readonly Error BrandNotFound = Error.NotFound(
      "Infrastructure.Brand.NotFound",
      "The brand with the specified identifier was not found.");
}
