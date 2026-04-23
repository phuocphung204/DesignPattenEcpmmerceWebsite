using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.BaseEntity;

namespace DesignPattern.Domain.Entities.Users;

public class LinkedAccount
{
  public Name Provider { get; private set; }
  public ID ProviderId { get; private set; }
  public DateTime? LastLogin { get; private set; }

  private LinkedAccount(Name provider, ID providerId)
  {
    Provider = provider;
    ProviderId = providerId;
  }

  // This constructor is for rehydration only, not for creating new linked accounts
  private LinkedAccount(
    Name provider,
    ID providerId,
    DateTime? lastLogin) : this(provider, providerId)
  {
    LastLogin = lastLogin;
  }

  public static Result<LinkedAccount> Create(
    string provider,
    string providerId)
  {
    Result<Name> providerResult = Name.Create(provider);
    if (providerResult.IsFailure)
      return providerResult.Error;

    Result<ID> providerIdResult = ID.Create(providerId);
    if (providerIdResult.IsFailure)
      return providerIdResult.Error;

    LinkedAccount linkedAccount = new LinkedAccount(providerResult.Value, providerIdResult.Value);
    return linkedAccount;
  }

  public void UpdateLastLogin()
  {
    LastLogin = DateTime.UtcNow;
  }
}