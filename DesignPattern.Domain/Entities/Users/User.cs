using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.User;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.Entities.Users;

public class User : BaseEntity
{
  private readonly Percent _loyaltyPointToMoneyRate = Percent.TenPercent;
  public Name FullName { get; private set; }
  public EmailAddress Email { get; private set; }
  public PasswordHash PasswordHash { get; private set; }
  public Quantity LoyaltyPoints { get; private set; } = Quantity.Zero;
  public UserRole Role { get; private set; }
  public UserStatus Status { get; private set; } = UserStatus.Active;
  public string AvatarLink { get; private set; } = "";
  public Guid? DefaultAddressId { get; private set; } = Guid.Empty;
  public List<Address> Addresses { get; private set; } = new();
  public List<LinkedAccount> LinkedAccounts { get; private set; } = new();

  private User(
    Name fullName,
    EmailAddress email,
    PasswordHash password,
    UserRole role,
    Address address)
  {
    FullName = fullName;
    Email = email;
    PasswordHash = password;
    Role = role;
    AddAddress(address);
    SetDefaultAddress(address.Id);
  }

  private User() { } // EF Core constructor

  public static Result<User> Create(
    string fullName,
    string email,
    string password,
    UserRole role,
    Address address)
  {

    var nameResult = Name.Create(fullName);
    if (nameResult.IsFailure)
      return nameResult.Error;

    var emailResult = EmailAddress.Create(email);
    if (emailResult.IsFailure)
      return emailResult.Error;

    var passResult = PasswordHash.Create(password);
    if (passResult.IsFailure)
      return passResult.Error;

    var user = new User(
        nameResult.Value,
        emailResult.Value,
        passResult.Value,
        role,
        address);

    return user;
  }
  // Methods for managing loyalty points
  public bool CheckInsufficientLoyaltyPoints(Quantity pointsToRedeem)
  {
    return LoyaltyPoints < pointsToRedeem;
  }
  // This method is called when order is completed, we will add points to user based on the grand total of the order.
  public void AddLoyaltyPoints(Price GrandAmount)
  {
    LoyaltyPoints = LoyaltyPoints + _loyaltyPointToMoneyRate.ApplyToQuantity(GrandAmount);
    Console.WriteLine($"[LoyaltyPoints] Added points. Remaining points: {LoyaltyPoints}");
  }
  // This method is called when user uses points to redeem for discount, so we deduct points immediately.
  public Result RedeemLoyaltyPoints(Quantity points)
  {
    if (CheckInsufficientLoyaltyPoints(points))
      return new Error("Insufficient.Points", "User does not have enough points to redeem");
    LoyaltyPoints = LoyaltyPoints - points;
    Console.WriteLine($"[LoyaltyPoints] Redeemed {points} points. Remaining points: {LoyaltyPoints}");
    return true;
  }
  // This method is called when order is returned, we will deduct points that were added for the order.
  public void DeductLoyaltyPoints(Price GrandAmount)
  {
    LoyaltyPoints = LoyaltyPoints - _loyaltyPointToMoneyRate.ApplyToQuantity(GrandAmount);
    Console.WriteLine($"[LoyaltyPoints] Deducted points. Remaining points: {LoyaltyPoints}");
  }

  // Methods for managing addresses
  public Result SetDefaultAddress(Guid addressId)
  {
    Console.WriteLine(">>> Setting default address to: " + addressId);
    if (!Addresses.Any(a => a.Id == addressId))
      return UserErrors.AddressNotFound;
    DefaultAddressId = addressId;
    return true;
  }

  public Result AddAddress(Address address)
  {
    Addresses.Add(address);
    return true;
  }

  public Result RemoveAddress(Guid addressId)
  {
    var address = Addresses.FirstOrDefault(a => a.Id == addressId);

    if (address == null)
      return UserErrors.AddressNotFound;

    Addresses.Remove(address);

    if (DefaultAddressId == addressId)
      DefaultAddressId = Addresses.FirstOrDefault()?.Id;

    return true;
  }

  public Result UpdateAddress(
    Guid addressId,
    string? receiverName,
    string? phoneNumber,
    string? country,
    string? province,
    string? district,
    string? ward,
    string? street,
    string? provinceCode,
    string? districtCode,
    string? wardCode)
  {
    var address = Addresses.FirstOrDefault(a => a.Id == addressId);

    if (address == null)
      return UserErrors.AddressNotFound;

    var updateResult = address.Update(receiverName, phoneNumber, country, province, district, ward, street, provinceCode, districtCode, wardCode);

    if (updateResult.IsFailure)
      return updateResult.Error;

    return true;
  }

  // Methods for managing Linked Accounts
  public void AddLinkedAccount(LinkedAccount linkedAccount)
  {
    LinkedAccounts.Add(linkedAccount);
  }

  public Result RemoveLinkedAccount(string providerId)
  {
    var account = LinkedAccounts.FirstOrDefault(la => la.ProviderId.Value == providerId);

    if (account == null)
      return UserErrors.LinkedAccountNotFound;

    LinkedAccounts.Remove(account);
    return true;
  }

  public Result UpdateProfile(
    string? fullName,
    // string? phoneNumber,
    string? email,
    string? avatarLink)
  {
    if (fullName != null)
    {
      Result<Name> nameResult = Name.Create(fullName);
      if (nameResult.IsFailure)
        return nameResult.Error;
      FullName = nameResult.Value;
    }

    if (email != null)
    {
      Result<EmailAddress> emailResult = EmailAddress.Create(email);
      if (emailResult.IsFailure)
        return emailResult.Error;
      Email = emailResult.Value;
    }

    if (avatarLink != null)
    {
      AvatarLink = avatarLink;
    }

    return true;
  }

}
