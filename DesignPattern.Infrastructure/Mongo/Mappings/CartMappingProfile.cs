using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Infrastructure.Mongo.Documents;

namespace DesignPattern.Infrastructure.Mongo.Mappings;

public class CartMappingProfile : Profile
{
  public CartMappingProfile()
  {
    // CartItem
    CreateMap<CartItem, CartItemDocument>(MemberList.None);
    CreateMap<CartItemDocument, CartItem>(MemberList.None);
    /// Cart
    CreateMap<Cart, CartDocument>(MemberList.None)
      .IncludeBase<BaseEntity, BaseDocument>();
    CreateMap<CartDocument, Cart>(MemberList.None)
      .IncludeBase<BaseDocument, BaseEntity>();
  }
}