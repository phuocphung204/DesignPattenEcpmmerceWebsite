using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Infrastructure.Mongo.Documents;
using static DesignPattern.Infrastructure.Mongo.Documents.ProductDocument;
using static DesignPattern.Infrastructure.Mongo.Repositories.ProductRepository;

namespace DesignPattern.Infrastructure.Mongo.Mappings;

public class ProductMappingProfile : Profile
{
  public ProductMappingProfile()
  {
    // ProductAttribute
    CreateMap<ProductAttribute, ProductAttributeDocument>(MemberList.None);
    CreateMap<ProductAttributeDocument, ProductAttribute>(MemberList.None)
      .ConstructUsing(src => ProductAttribute.Rehydrate(
        Name.Rehydrate(src.Name),
        StandardText.Rehydrate(src.Value),
        src.Type));

    /// Product
    CreateMap<Product, ProductDocument>(MemberList.None)
      .IncludeBase<BaseEntity, BaseDocument>();

    CreateMap<ProductDocument, Product>(MemberList.None)
      .IncludeBase<BaseDocument, BaseEntity>()
      .ConstructUsing(src => Product.Rehydrate(
        src.Name,
        src.BrandId,
        src.CategoryId,
        src.BrandName,
        src.CategoryName,
        src.VariantGroupId,
        src.Sku,
        src.Images,
        src.Attributes.Select(attr => ProductAttribute.Rehydrate(
          Name.Rehydrate(attr.Name),
          StandardText.Rehydrate(attr.Value),
          attr.Type)).ToList(),
        src.SellingPrice,
        src.PurchasePrice,
        src.SoldQuantity,
        src.StockQuantity,
        src.Rating,
        src.ShortDescription,
        src.DetailDescription,
        src.Status));

    // Map từ ProductWithSeoRow -> Entity (Sử dụng trong GetByIdAsync hoặc MapExpression)
    CreateMap<ProductWithSeoRow, Product>(MemberList.None)
      .IncludeBase<ProductDocument, Product>()
      .ForMember(dest => dest.Seo, opt => opt.MapFrom(src => src.Seos.FirstOrDefault()));

    CreateMap<Product, ProductWithSeoRow>(MemberList.None)
      .IncludeBase<Product, ProductDocument>()
      .ForMember(dest => dest.Seo, opt => opt.MapFrom(src => src.Seo));
  }
}