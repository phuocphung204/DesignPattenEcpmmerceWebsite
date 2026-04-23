using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.Order;
using DesignPattern.Domain.ValueObjects.Payment;
using DesignPattern.Infrastructure.Mongo.Documents;

namespace DesignPattern.Infrastructure.Mongo.Mappings;

public class OrderMappingProfile : Profile
{
  public OrderMappingProfile()
  {
    // InventoryAllocation
    CreateMap<InventoryAllocation, InventoryAllocationDocument>(MemberList.None);
    CreateMap<InventoryAllocationDocument, InventoryAllocation>(MemberList.None);
    // OrderItem
    CreateMap<OrderItem, OrderItemDocument>(MemberList.None);
    CreateMap<OrderItemDocument, OrderItem>(MemberList.None);
    // OrderHistory
    CreateMap<OrderHistory, OrderHistoryDocument>(MemberList.None);
    CreateMap<OrderHistoryDocument, OrderHistory>(MemberList.None);

    // ShippingInfo
    CreateMap<ShippingInfo, ShippingInfoDocument>(MemberList.None);
    CreateMap<ShippingInfoDocument, ShippingInfo>(MemberList.None);
    /// PaymentMethod
    // Cash
    CreateMap<Cash, PaymentInfoDocument>(MemberList.None)
      .ForMember(dest => dest.Type, opt => opt.MapFrom(src => PaymentType.Cash))
      .ForMember(dest => dest.BankName, opt => opt.Ignore())
      .ForMember(dest => dest.AccountNumber, opt => opt.Ignore())
      .ForMember(dest => dest.CardNumber, opt => opt.Ignore())
      .ForMember(dest => dest.CardHolder, opt => opt.Ignore())
      .ForMember(dest => dest.CardType, opt => opt.Ignore());
    // CreditCard
    CreateMap<CreditCard, PaymentInfoDocument>(MemberList.None)
      .ForMember(dest => dest.Type, opt => opt.MapFrom(src => PaymentType.CreditCard))
      .ForMember(dest => dest.BankName, opt => opt.Ignore())
      .ForMember(dest => dest.AccountNumber, opt => opt.Ignore())
      .ForMember(dest => dest.CardNumber, opt => opt.MapFrom(src => src.CardNumber.Value))
      .ForMember(dest => dest.CardHolder, opt => opt.MapFrom(src => src.CardHolder.Value))
      .ForMember(dest => dest.CardType, opt => opt.MapFrom(src => src.CardType.Value));
    // BankTransfer
    CreateMap<BankTransfer, PaymentInfoDocument>(MemberList.None)
      .ForMember(dest => dest.Type, opt => opt.MapFrom(src => PaymentType.BankTransfer))
      .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.BankName.Value))
      .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber.Value))
      .ForMember(dest => dest.CardNumber, opt => opt.Ignore())
      .ForMember(dest => dest.CardHolder, opt => opt.Ignore())
      .ForMember(dest => dest.CardType, opt => opt.Ignore());
    // Polymorphic mapping for PaymentMethod
    CreateMap<PaymentInfo, PaymentInfoDocument>(MemberList.None)
      .Include<Cash, PaymentInfoDocument>()
      .Include<CreditCard, PaymentInfoDocument>()
      .Include<BankTransfer, PaymentInfoDocument>();
    CreateMap<PaymentInfoDocument, PaymentInfo>(MemberList.None)
      .ConvertUsing((src, context) =>
      {
        return src.Type switch
        {
          PaymentType.Cash => Cash.Rehydrate(
            src.PaymentProvider,
            src.PaymentChannel,
            src.TransactionId,
            src.PaidAt
          ),
          PaymentType.CreditCard => src.CardNumber is not string cardNumber || src.CardHolder is not string cardHolder || src.CardType is not string cardType
            ? throw new Exception("CardNumber, CardHolder or CardType is null")
            : CreditCard.Rehydrate(
              src.CardNumber,
              src.CardHolder,
              src.CardType,
              src.PaymentProvider,
              src.PaymentChannel,
              src.TransactionId,
              src.PaidAt
          ),
          PaymentType.BankTransfer => src.BankName is not string bankName || src.AccountNumber is not string accountNumber
            ? throw new Exception("BankName or AccountNumber is null")
              : BankTransfer.Rehydrate(
                src.BankName,
                src.AccountNumber,
                src.PaymentProvider,
                src.PaymentChannel,
                src.TransactionId,
                src.PaidAt
          ),
          _ => throw new Exception($"Unsupported payment type: {src.Type}")
        };
      });
    //// Order
    CreateMap<Order, OrderDocument>(MemberList.None)
      .IncludeBase<BaseEntity, BaseDocument>()
      .ForMember(dest => dest.DiscountCode, opt => opt.MapFrom(src => src.DiscountCode != null ? src.DiscountCode.Value : null))
      .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note != null ? src.Note.Value : string.Empty));

    CreateMap<OrderDocument, Order>(MemberList.None)
      .IncludeBase<BaseDocument, BaseEntity>()
      .ForCtorParam("discountCode", opt => opt.MapFrom(src => src.DiscountCode == null ? null : Code.Rehydrate(src.DiscountCode)))
      .ForCtorParam("note", opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Note) ? null : Note.Rehydrate(src.Note)));
  }
}