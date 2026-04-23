using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Infrastructure.Mongo.Documents;

public sealed class UserDocument : BaseDocument
{
  [BsonElement("fullName")]
  public string FullName { get; init; }

  [BsonElement("email")]
  public string Email { get; init; }


  [BsonElement("avatarLink")]
  public string AvatarLink { get; init; } = string.Empty;

  [BsonElement("passwordHash")]
  public string PasswordHash { get; init; }

  [BsonElement("role")]
  [BsonRepresentation(BsonType.String)]
  public UserRole Role { get; init; }

  [BsonElement("status")]
  [BsonRepresentation(BsonType.String)]
  public UserStatus Status { get; init; } = UserStatus.Active;

  [BsonElement("loyaltyPoints")]
  public int LoyaltyPoints { get; init; }

  [BsonElement("defaultAddressId")]
  public Guid DefaultAddressId { get; init; } = Guid.Empty;

  [BsonElement("addresses")]
  public List<VietNamAddress> Addresses { get; init; } = new();

  [BsonElement("linkedAccounts")]
  public List<LinkedAccountDocument> LinkedAccounts { get; init; } = new();

}

public sealed class VietNamAddress : BaseDocument
{
  [BsonElement("receiverName")]
  [BsonRequired]
  public required string ReceiverName { get; init; }

  [BsonElement("phoneNumber")]
  [BsonRequired]
  public required string PhoneNumber { get; init; }

  [BsonElement("province")]
  public string Province { get; init; } = string.Empty;

  [BsonElement("district")]
  public string District { get; init; } = string.Empty;

  [BsonElement("ward")]
  public string Ward { get; init; } = string.Empty;

  [BsonElement("street")]
  public string Street { get; init; } = string.Empty;

  [BsonElement("provinceCode")]
  public string ProvinceCode { get; init; } = string.Empty;

  [BsonElement("districtCode")]
  public string DistrictCode { get; init; } = string.Empty;

  [BsonElement("wardCode")]
  public string WardCode { get; init; } = string.Empty;

  [BsonElement("country")]
  public string Country { get; init; } = string.Empty;
}

public sealed class LinkedAccountDocument
{
  [BsonElement("provider")]
  [BsonRequired]
  public required string Provider { get; init; }

  [BsonElement("providerId")]
  [BsonRequired]
  public required string ProviderId { get; init; }

  [BsonElement("lastLogin")]
  public DateTime? LastLogin { get; init; }
}
