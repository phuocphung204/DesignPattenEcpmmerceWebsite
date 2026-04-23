
namespace DesignPattern.Domain.Errors;

public static class CartErrors
{
  public static Error ItemNotFound => Error.NotFound(
    "Cart.ItemNotFound",
    "Item not found in the cart."
  );
  public static Error NotFound => Error.NotFound(
    "Cart.NotFound",
    "Cart not found for the specified user."
  );



  public static Error ProductNotFound => Error.NotFound(
    "Cart.ProductNotFound",
    "The specified product was not found."
  );
}
