namespace DesignPattern.Domain.Errors;

public static class RatingErrors
{
  public static readonly Error RatingNotFound = Error.NotFound(
    "Rating.NotFound",
    "Rating not found.");

  public static readonly Error ProductNotFound = Error.NotFound(
    "Product.NotFound",
    "Product not found.");
}