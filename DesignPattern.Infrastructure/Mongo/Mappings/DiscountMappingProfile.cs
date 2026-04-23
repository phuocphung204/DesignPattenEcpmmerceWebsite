using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.DiscountCodes;
using DesignPattern.Domain.Enums;
using DesignPattern.Infrastructure.Mongo.Documents;

namespace DesignPattern.Infrastructure.Mongo.Mappings;

public class DiscountMappingProfile : Profile
{
  public DiscountMappingProfile()
  {
    /// Polymorphic mapping for DiscountCode
    CreateMap<DiscountCode, DiscountCodeDocument>(MemberList.None)
      .Include<FixedDiscountCode, DiscountCodeDocument>()
      .Include<PercentageDiscountCode, DiscountCodeDocument>()
      .IncludeBase<BaseEntity, BaseDocument>();
    // FixedDiscountCode
    CreateMap<FixedDiscountCode, DiscountCodeDocument>(MemberList.None)
      .IncludeBase<DiscountCode, DiscountCodeDocument>()
      .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.FixedAmount.Amount))
      .ForMember(dest => dest.Percent, opt => opt.Ignore())
      .ForMember(dest => dest.MaximumDiscountAmount, opt => opt.Ignore());
    // PercentageDiscountCode
    CreateMap<PercentageDiscountCode, DiscountCodeDocument>(MemberList.None)
      .IncludeBase<DiscountCode, DiscountCodeDocument>()
      .ForMember(dest => dest.Amount, opt => opt.Ignore())
      .ForMember(dest => dest.Percent, opt => opt.MapFrom(src => src.Percent.Value))
      .ForMember(dest => dest.MaximumDiscountAmount, opt => opt.MapFrom(src => src.MaximumDiscountAmount.Amount));
    
    CreateMap<DiscountCodeDocument, DiscountCode>()
      .ConvertUsing((src, context) =>
      {
        return src.Type switch
        {
            DiscountType.FixedAmount => src.Amount is not decimal amount
                    ? throw new Exception("Amount is null")
                    : FixedDiscountCode.Rehydrate(
                src.Id,
                src.CreatedAt,
                src.UpdatedAt, // có thể null nhưng ko cần xử lý vì đã đặt DateTime? cho tham số này
                src.Code,
                src.MinimumOrderAmount,
                src.UsageLimit,
                src.UsedCount,
                src.ExpirationDate,
                src.IsActive,
                amount
            ),
            
            DiscountType.Percentage => src.Percent is not decimal percent || src.MaximumDiscountAmount is not decimal maximumDiscountAmount
                    ? throw new Exception("Percentage or MaximumDiscountAmount is null")
                    : PercentageDiscountCode.Rehydrate(
                src.Id,
                src.CreatedAt,
                src.UpdatedAt, // có thể null nhưng ko cần xử lý vì đã đặt DateTime? cho tham số này
                src.Code,
                src.MinimumOrderAmount,
                src.UsageLimit,
                src.UsedCount,
                src.ExpirationDate,
                src.IsActive,
                percent,
                maximumDiscountAmount
            ),

            _ => throw new Exception("Invalid DiscountType")
        };
      });
  }
}
