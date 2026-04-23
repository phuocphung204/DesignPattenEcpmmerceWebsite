using System.Globalization;
using System.Text;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.BaseEntity;
using DesignPattern.Domain.Entities.Inventory;
using DesignPattern.Domain.Enums;
namespace DesignPattern.Domain.Entities;

public class Product : BaseEntity
{
  public Name Name { get; private set; }
  public Guid BrandId { get; private set; }
  public Name BrandName { get; private set; }
  public Guid CategoryId { get; private set; }
  public Name CategoryName { get; private set; }
  public StandardText VariantGroupId { get; private set; }
  public StandardText Sku { get; private set; }
  public List<string> Images { get; private set; } = new();
  public List<ProductAttribute> Attributes { get; private set; } = new();
  public Price SellingPrice { get; private set; } = Price.Zero;
  public Price PurchasePrice { get; private set; } = Price.Zero;
  public Quantity SoldQuantity { get; private set; } = Quantity.Zero;
  public Quantity StockQuantity { get; private set; } = Quantity.Zero;
  public decimal Rating { get; private set; } = 0.0m;
  public string ShortDescription { get; private set; } = string.Empty;
  public string DetailDescription { get; private set; } = string.Empty;
  public ProductStatus Status { get; private set; } = ProductStatus.Active;
  public Seo Seo { get; private set; } = new();

  public Product(
    Name name,
    Guid brandId,
    Guid categoryId,
    Name brandName,
    Name categoryName,
    StandardText variantGroupId,
    StandardText sku,
    List<string>? images,
    List<ProductAttribute>? attributes,
    Price sellingPrice,
    Price purchasePrice,
    string shortDescription,
    string detailDescription,
    Seo seo)
  {
    Name = name;
    BrandId = brandId;
    CategoryId = categoryId;
    BrandName = brandName;
    CategoryName = categoryName;
    VariantGroupId = variantGroupId;
    Sku = sku;
    Images = images ?? new List<string>();
    Attributes = attributes ?? new List<ProductAttribute>();
    SellingPrice = sellingPrice;
    PurchasePrice = purchasePrice;
    ShortDescription = shortDescription;
    DetailDescription = detailDescription;
    Seo = seo;
  }

  public Product(
    Name name,
    Guid brandId,
    Guid categoryId,
    Name brandName,
    Name categoryName,
    StandardText variantGroupId,
    StandardText sku,
    List<string>? images,
    List<ProductAttribute>? attributes,
    Price sellingPrice,
    Price purchasePrice,
    Quantity soldQuantity,
    Quantity stockQuantity,
    decimal rating,
    string shortDescription,
    string detailDescription,
    ProductStatus status)
  {
    Name = name;
    BrandId = brandId;
    CategoryId = categoryId;
    BrandName = brandName;
    CategoryName = categoryName;
    VariantGroupId = variantGroupId;
    Sku = sku;
    Images = images ?? new List<string>();
    Attributes = attributes ?? new List<ProductAttribute>();
    SellingPrice = sellingPrice;
    PurchasePrice = purchasePrice;
    SoldQuantity = soldQuantity;
    StockQuantity = stockQuantity;
    Rating = rating;
    ShortDescription = shortDescription;
    DetailDescription = detailDescription;
    Status = status;
  }

  public static Result<Product> Create(
    string name,
    Guid brandId,
    Guid categoryId,
    string brandName,
    string categoryName,
    string variantGroupId,
    string sku,
    List<string>? images,
    List<ProductAttribute>? attributes,
    decimal sellingPrice,
    decimal purchasePrice,
    string shortDescription,
    string detailDescription,
    Seo seo)
  {
    var nameResult = Name.Create(name);
    if (nameResult.IsFailure) return nameResult.Error;

    var brandNameResult = Name.Create(brandName);
    if (brandNameResult.IsFailure) return brandNameResult.Error;
    var categoryNameResult = Name.Create(categoryName);
    if (categoryNameResult.IsFailure) return categoryNameResult.Error;

    var variantGroupIdResult = StandardText.Create(variantGroupId);
    if (variantGroupIdResult.IsFailure) return variantGroupIdResult.Error;

    var skuResult = StandardText.Create(sku);
    if (skuResult.IsFailure) return skuResult.Error;

    var sellingPriceResult = Price.Create(sellingPrice);
    if (sellingPriceResult.IsFailure) return sellingPriceResult.Error;

    var purchasePriceResult = Price.Create(purchasePrice);
    if (purchasePriceResult.IsFailure) return purchasePriceResult.Error;

    var product = new Product(
      nameResult.Value,
      brandId,
      categoryId,
      brandNameResult.Value,
      categoryNameResult.Value,
      variantGroupIdResult.Value,
      skuResult.Value,
      images,
      attributes,
      sellingPriceResult.Value,
      purchasePriceResult.Value,
      shortDescription,
      detailDescription,
      seo
    );
    return product;
  }

  public static Product Rehydrate(
    string name,
    Guid brandId,
    Guid categoryId,
    string brandName,
    string categoryName,
    string variantGroupId,
    string sku,
    List<string>? images,
    List<ProductAttribute>? attributes,
    decimal sellingPrice,
    decimal purchasePrice,
    int soldQuantity,
    int stockQuantity,
    decimal rating,
    string shortDescription,
    string detailDescription,
    ProductStatus status)
  {
    return new Product(
      Name.Rehydrate(name),
      brandId,
      categoryId,
      Name.Rehydrate(brandName),
      Name.Rehydrate(categoryName),
      StandardText.Rehydrate(variantGroupId),
      StandardText.Rehydrate(sku),
      images,
      attributes,
      Price.Rehydrate(sellingPrice),
      Price.Rehydrate(purchasePrice),
      Quantity.Rehydrate(soldQuantity),
      Quantity.Rehydrate(stockQuantity),
      rating,
      shortDescription,
      detailDescription,
      status
    );
  }

  public Result Update(
    string? name,
    Guid? brandId,
    Guid? categoryId,
    string? brandName,
    string? categoryName,
    string? variantGroupId,
    string? sku,
    List<string>? images,
    List<ProductAttribute>? attributes,
    string? shortDescription,
    string? detailDescription,
    decimal? sellingPrice,
    decimal? purchasePrice,
    ProductStatus? status = null)
  {
    if (name is not null)
    {
      var nameResult = Name.Create(name);
      if (nameResult.IsFailure) return nameResult.Error;
      Name = nameResult.Value;
    }
    if (brandId is not null && brandName is not null)
    {
      BrandId = brandId.Value;
      var brandNameResult = Name.Create(brandName);
      if (brandNameResult.IsFailure) return brandNameResult.Error;
      BrandName = brandNameResult.Value;
    }
    if (categoryId is not null && categoryName is not null)
    {
      CategoryId = categoryId.Value;
      var categoryNameResult = Name.Create(categoryName);
      if (categoryNameResult.IsFailure) return categoryNameResult.Error;
      CategoryName = categoryNameResult.Value;
    }

    if (variantGroupId is not null)
    {
      var variantGroupIdResult = StandardText.Create(variantGroupId);
      if (variantGroupIdResult.IsFailure) return variantGroupIdResult.Error;
      VariantGroupId = variantGroupIdResult.Value;
    }
    if (sku is not null)
    {
      var skuResult = StandardText.Create(sku);
      if (skuResult.IsFailure) return skuResult.Error;
      Sku = skuResult.Value;
    }
    if (images is not null)
      Images = images;
    if (attributes is not null)
      Attributes = attributes;
    if (shortDescription is not null)
    {
      ShortDescription = shortDescription;
    }
    if (detailDescription is not null)
    {
      DetailDescription = detailDescription;
    }
    if (sellingPrice is not null)
    {
      var sellingPriceResult = Price.Create(sellingPrice.Value);
      if (sellingPriceResult.IsFailure) return sellingPriceResult.Error;
      SellingPrice = sellingPriceResult.Value;
    }
    if (purchasePrice is not null)
    {
      var purchasePriceResult = Price.Create(purchasePrice.Value);
      if (purchasePriceResult.IsFailure) return purchasePriceResult.Error;
      PurchasePrice = purchasePriceResult.Value;
    }
    if (status is not null)
    {
      Status = status.Value;
    }
    return true;
  }
  public void UpdateRating(decimal newRating)
  {
    Rating = newRating;
  }
  public void UpdateStockQuantity(int newStockQuantity)
  {
    StockQuantity = Quantity.Create(newStockQuantity).Value;
  }
  public void SetSeo(Seo seo)
  {
    Seo = seo;
  }
}