using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities;
using DesignPattern.Infrastructure.Mongo.Documents;

namespace DesignPattern.Infrastructure.Mongo.Mappings;

public class RatingMappingProfile : Profile
{
  public RatingMappingProfile()
  {
    // Rating
    CreateMap<Rating, RatingDocument>(MemberList.None)
      .IncludeBase<BaseEntity, BaseDocument>();
    CreateMap<RatingDocument, Rating>(MemberList.None)
      .IncludeBase<BaseDocument, BaseEntity>();
  }
}