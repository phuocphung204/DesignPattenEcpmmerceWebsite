using AutoMapper;
using AutoMapper.Internal.Mappers;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Infrastructure.Mongo.Mappings;
public class EnumMappingProfile : Profile
{
    public EnumMappingProfile()
    {
        MapEnum<DiscountType>();
        MapEnum<OrderStatusEnum>();
        MapEnum<PaymentStatusEnum>();
        MapEnum<PaymentType>();
        MapEnum<ProductAttributeType>();
        MapEnum<ProductStatus>();
        MapEnum<ShippingType>();
        MapEnum<UserRole>();
        MapEnum<UserStatus>();
    }

    private void MapEnum<TEnum>() where TEnum : struct, Enum
    {
        CreateMap<string, TEnum>()
          .ConvertUsing(src => (TEnum)Enum.Parse(typeof(TEnum), src, true));

        CreateMap<TEnum, string>()
          .ConvertUsing(src => src.ToString());
    }
}