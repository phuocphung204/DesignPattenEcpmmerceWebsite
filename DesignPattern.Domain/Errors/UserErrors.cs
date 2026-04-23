namespace DesignPattern.Domain.Errors;

public static class UserErrors
{
  public static readonly Error AddressNotFound = Error.NotFound(
    "User.AddressNotFound",
    "Address not found for the specified address ID.");
  public static readonly Error UserNotFound = Error.NotFound(
    "User.NotFound",
    "User not found.");

  public static readonly Error LinkedAccountNotFound = Error.NotFound(
    "User.LinkedAccountNotFound",
    "Linked account not found for the specified provider ID.");

  public static readonly Error PhoneAlreadyExists = Error.Conflict(
    "User.PhoneAlreadyExists",
    "Phone number is already in use by another user.");
}