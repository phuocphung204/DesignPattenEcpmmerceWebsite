using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.BaseEntity;

namespace DesignPattern.Domain.Entities.Inventory;

public class Warehouse : BaseEntity
{
  public Name Name { get; private set; }
  public string Address { get; private set; }

  private Warehouse(Name name, string address)
  {
    Name = name;
    Address = address;
  }

  public static Result<Warehouse> Create(string name, string address)
  {
    var nameResult = Name.Create(name);
    if (nameResult.IsFailure) return nameResult.Error;

    return new Warehouse(nameResult.Value, address);
  }

  public Result Update(string name, string address)
  {
    var nameResult = Name.Create(name);
    if (nameResult.IsFailure) return nameResult.Error;

    Name = nameResult.Value;
    Address = address;
    return Result.Success();
  }
}