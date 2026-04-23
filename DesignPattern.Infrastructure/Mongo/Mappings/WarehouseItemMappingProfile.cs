using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Inventory;
using DesignPattern.Infrastructure.Mongo.Documents;


namespace DesignPattern.Infrastructure.Mongo.Mappings;

public class WarehouseItemMappingProfile : Profile
{
  public WarehouseItemMappingProfile()
  {
    // WarehouseItem
    CreateMap<WarehouseItem, WarehouseItemDocument>(MemberList.None)
      .IncludeBase<BaseEntity, BaseDocument>();
    CreateMap<WarehouseItemDocument, WarehouseItem>(MemberList.None)
      .IncludeBase<BaseDocument, BaseEntity>();
  }
}