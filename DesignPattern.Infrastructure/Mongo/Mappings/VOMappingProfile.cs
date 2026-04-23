using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.BaseEntity;
using DesignPattern.Domain.ValueObjects.Seo;
using DesignPattern.Domain.ValueObjects.User;
using DesignPattern.Infrastructure.Mongo.Documents;

namespace DesignPattern.Infrastructure.Mongo.Mappings;

public class VOMappingProfile : Profile
{
  public VOMappingProfile()
  {
    //ID
    CreateMap<ID, string>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<string, ID>(MemberList.None)
      .ConvertUsing(src => ID.Rehydrate(src));
    // Slug
    CreateMap<Slug, string>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<string, Slug>(MemberList.None)
      .ConvertUsing(src => Slug.Rehydrate(src));
    // MetaTitle
    CreateMap<MetaTitle, string>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<string, MetaTitle>(MemberList.None)
      .ConvertUsing(src => MetaTitle.Rehydrate(src));
    // MetaDescription
    CreateMap<MetaDescription, string>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<string, MetaDescription>(MemberList.None)
      .ConvertUsing(src => MetaDescription.Rehydrate(src));
    // LoyaltyPoint
    CreateMap<LoyaltyPoint, int>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<int, LoyaltyPoint>(MemberList.None)
      .ConvertUsing(src => LoyaltyPoint.Rehydrate(src));
    // PasswordHash
    CreateMap<PasswordHash, string>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<string, PasswordHash>(MemberList.None)
      .ConvertUsing(src => PasswordHash.Rehydrate(src));
    // PhoneNumber
    CreateMap<PhoneNumber, string>(MemberList.None)
    .ConvertUsing(src => src.Value);
    CreateMap<string, PhoneNumber>(MemberList.None)
      .ConvertUsing(src => PhoneNumber.Rehydrate(src));
    // Code
    CreateMap<Code, string>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<string, Code>(MemberList.None)
      .ConvertUsing(src => Code.Rehydrate(src));
    // EmailAddress
    CreateMap<EmailAddress, string>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<string, EmailAddress>(MemberList.None)
      .ConvertUsing(src => EmailAddress.Rehydrate(src));
    // Level
    CreateMap<Level, int>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<int, Level>(MemberList.None)
      .ConvertUsing(src => Level.Rehydrate(src));
    // Name
    CreateMap<Name, string>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<string, Name>(MemberList.None)
      .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
      .ConvertUsing(src => Name.Rehydrate(src));
    // Note
    CreateMap<Note, string>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<string, Note>(MemberList.None)
      .ConvertUsing(src => Note.Rehydrate(src));
    // Percent
    CreateMap<Percent, decimal>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<decimal, Percent>(MemberList.None)
      .ConvertUsing(src => Percent.Rehydrate(src));
    // Price
    CreateMap<Price, decimal>(MemberList.None)
      .ConvertUsing(src => src.Amount);
    CreateMap<decimal, Price>(MemberList.None)
      .ConvertUsing(src => Price.Rehydrate(src));
    // Quantity
    CreateMap<Quantity, int>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<int, Quantity>(MemberList.None)
      .ConvertUsing(src => Quantity.Rehydrate(src));
    // RatingValue
    CreateMap<RatingValue, int>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<int, RatingValue>(MemberList.None)
      .ConvertUsing(src => RatingValue.Rehydrate(src));
    // StandardText
    CreateMap<StandardText, string>(MemberList.None)
      .ConvertUsing(src => src.Value);
    CreateMap<string, StandardText>(MemberList.None)
      .ConvertUsing(src => StandardText.Rehydrate(src));

    // Address
    // Address
    CreateMap<Address, VietNamAddress>(MemberList.None)
      .IncludeBase<BaseEntity, BaseDocument>()
      .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ReceiverPhone));
    CreateMap<VietNamAddress, Address>(MemberList.None)
      .IncludeBase<BaseDocument, BaseEntity>()
      .ForMember(dest => dest.ReceiverPhone, opt => opt.MapFrom(src => src.PhoneNumber));

    // AttributeItem
    CreateMap<AttributeItem, AttributeDocument>(MemberList.None);
    CreateMap<AttributeDocument, AttributeItem>(MemberList.None);
  }
}
