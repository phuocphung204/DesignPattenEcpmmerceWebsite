namespace DesignPattern.Domain.Errors;

public static class ProductCategoryErrors
{
  public static readonly Error NotFound = Error.NotFound(
    "ProductCategory.NotFound",
    "Product category not found.");

  public static readonly Error HasSubCategories = Error.Conflict(
    "ProductCategory.HasSubCategories",
    "Cannot delete category because it has sub-categories.");
  public static readonly Error HasProducts = Error.Conflict(
    "ProductCategory.HasProducts",
    "Cannot delete category because it contains products.");

  public static readonly Error InvalidParentCategoryId = Error.Conflict(
    "ProductCategory.InvalidParentCategoryId",
    "Invalid parent category id.");
}