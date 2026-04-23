using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities;
using DesignPattern.Domain.Entities.ProductCategory;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.Seo;
using DesignPattern.Infrastructure.Mongo.Documents;
using static DesignPattern.Infrastructure.Mongo.Repositories.BrandRepository;
using static DesignPattern.Infrastructure.Mongo.Repositories.ProductCategoryRepository;

namespace DesignPattern.Infrastructure.Mongo.Mappings;


public class MappingProfile : Profile
{
  public MappingProfile()
  {
    // CreateMap<string, StandardText>(MemberList.None)
    //   .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
    //   .ConvertUsing(s => StandardText.Create(s).Value);
    // CreateMap<StandardText, string>(MemberList.None).ConvertUsing(src => src.Value);

    // CreateMap<string, Name>(MemberList.None)
    //   .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
    //   .ConvertUsing(s => Name.Create(s).Value);
    // CreateMap<Name, string>(MemberList.None).ConvertUsing(src => src.Value);

    // CreateMap<string, Slug>(MemberList.None)
    //   .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
    //   .ConvertUsing(s => Slug.Create(s).Value);
    // CreateMap<Slug, string>(MemberList.None).ConvertUsing(src => src.Value);

    // CreateMap<int, Level>(MemberList.None)
    //   .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
    //   .ConvertUsing(l => Level.Create(l).Value);
    // CreateMap<Level, int>(MemberList.None).ConvertUsing(src => src.Value);

    // CreateMap<MetaDescription, string>(MemberList.None).ConvertUsing(src => src.Value);
    // CreateMap<MetaTitle, string>(MemberList.None).ConvertUsing(src => src.Value);

    // Base document
    CreateMap<BaseEntity, BaseDocument>(MemberList.None);
    CreateMap<BaseDocument, BaseEntity>(MemberList.None);

    // ProductCategory mappings
    CreateMap<Guid?, ProductCategory>(MemberList.None)
      .ConstructUsing(id => (ProductCategory)Activator.CreateInstance(typeof(ProductCategory), true)!)
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src))
      .ForMember(dest => dest.Seo, opt => opt.Ignore());

    // Seo mappings
    CreateMap<SeoDocument, Seo>(MemberList.None)
      .ConstructUsing(src => Seo.Create(src.MetaTitle, src.MetaDescription, src.Slug).Value)
      .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.MetaTitle))
      .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.MetaDescription))
      .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug));

    CreateMap<Seo, SeoDocument>(MemberList.None)
      .ConstructUsing(src => new())
      .ForMember(dest => dest.MetaTitle, opt => opt.MapFrom(src => src.Title != null ? src.Title.Value : null))
      .ForMember(dest => dest.MetaDescription, opt => opt.MapFrom(src => src.Description != null ? src.Description.Value : null))
      .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug != null ? src.Slug.Value : null))
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.RefEntityId, opt => opt.Ignore());

    // Map từ Document (Database) -> Entity (Domain)
    CreateMap<ProductCategoryDocument, ProductCategory>(MemberList.None)
      .IncludeBase<BaseDocument, BaseEntity>()
      .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
      .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level))
      .ForMember(dest => dest.ParentCategory, opt => opt.MapFrom(src => src.ParentCategoryId))
      // .ForMember(dest => dest.Seo, opt => opt.Ignore())
      ;

    // Map từ Entity (Domain) -> Document (Database) - CRITICAL for Expression Mapping
    CreateMap<ProductCategory, ProductCategoryDocument>(MemberList.None)
      .IncludeBase<BaseEntity, BaseDocument>()
      .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value)) // Helps translation of c.Name.Value -> d.Name
      .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level != null ? src.Level.Value : 0))
      .ForMember(dest => dest.ParentCategoryId, opt => opt.MapFrom(src => src.ParentCategory != null ? (Guid?)src.ParentCategory.Id : null));

    // Map từ ProductCategoryWithSeoRow -> Entity (Sử dụng trong GetByIdAsync hoặc MapExpression)
    CreateMap<ProductCategoryWithSeoRow, ProductCategory>(MemberList.None)
      .IncludeBase<ProductCategoryDocument, ProductCategory>()
      .ForMember(dest => dest.Seo, opt => opt.MapFrom(src => src.Seos.FirstOrDefault()));

    CreateMap<ProductCategory, ProductCategoryWithSeoRow>(MemberList.None)
      .IncludeBase<ProductCategory, ProductCategoryDocument>()
      .ForMember(dest => dest.Seo, opt => opt.MapFrom(src => src.Seo));

    // Brand mappings
    CreateMap<Guid?, Brand>(MemberList.None)
      .ConstructUsing(id => (Brand)Activator.CreateInstance(typeof(Brand), true)!)
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src))
      .ForMember(dest => dest.Seo, opt => opt.Ignore());

    CreateMap<BrandDocument, Brand>(MemberList.None)
      .IncludeBase<BaseDocument, BaseEntity>()
      .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
      .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level))
      .ForMember(dest => dest.ParentBrand, opt => opt.MapFrom(src => src.ParentId));

    CreateMap<Brand, BrandDocument>(MemberList.None)
      .IncludeBase<BaseEntity, BaseDocument>()
      .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
      .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level != null ? src.Level.Value : 0))
      .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentBrand != null ? (Guid?)src.ParentBrand.Id : null));

    CreateMap<BrandWithSeoRow, Brand>(MemberList.None)
      .IncludeBase<BrandDocument, Brand>()
      .ForMember(dest => dest.Seo, opt => opt.MapFrom(src => src.Seos.FirstOrDefault()));

    CreateMap<Brand, BrandWithSeoRow>(MemberList.None)
      .IncludeBase<Brand, BrandDocument>()
      .ForMember(dest => dest.Seo, opt => opt.MapFrom(src => src.Seo));
  }
}
