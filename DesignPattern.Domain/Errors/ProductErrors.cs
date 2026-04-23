namespace DesignPattern.Domain.Errors;

public static class ProductErrors
{
  public static readonly Error SlugAlreadyExists = Error.Conflict(
      "Product.SlugAlreadyExists",
      "A product with the same slug already exists. Please choose a different slug.");

  public static readonly Error CategoryNotFound = Error.NotFound(
      "ProductCategory.NotFound",
      "The product category with the specified identifier was not found.");

  public static readonly Error BrandNotFound = Error.NotFound(
      "Brand.NotFound",
      "The brand with the specified identifier was not found.");
  public static readonly Error ProductNotFound = Error.NotFound(
      "Product.NotFound",
      "The product with the specified identifier was not found.");

  public static Error NotFound(Guid productId) => Error.NotFound(
      "Product.NotFound",
      $"The product with ID '{productId}' was not found.");

}