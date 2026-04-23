using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Inventory;
using DesignPattern.Infrastructure.Mongo.Documents;

namespace DesignPattern.Infrastructure.Mongo.Mappings;

public class WarehouseMappingProfile : Profile
{
  public WarehouseMappingProfile()
  {
    /// Warehouse
    CreateMap<Warehouse, WarehouseDocument>(MemberList.None)
      .IncludeBase<BaseEntity, BaseDocument>();
    CreateMap<WarehouseDocument, Warehouse>()
      .IncludeBase<BaseDocument, BaseEntity>();
  }
}