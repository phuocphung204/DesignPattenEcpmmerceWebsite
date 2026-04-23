using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Infrastructure.Mongo.Documents;

namespace DesignPattern.Infrastructure.Mongo.Mappings;

public class UserMappingProfile : Profile
{
  public UserMappingProfile()
  {
    // LinkedAccountResolutionContext
    CreateMap<LinkedAccount, LinkedAccountDocument>(MemberList.None);
    CreateMap<LinkedAccountDocument, LinkedAccount>(MemberList.None);

    /// User
    CreateMap<User, UserDocument>(MemberList.None)
      .IncludeBase<BaseEntity, BaseDocument>();
    CreateMap<UserDocument, User>()
      .IncludeBase<BaseDocument, BaseEntity>();
  }
}