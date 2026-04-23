// using System.Reflection;
// using System.Text.Json;
// using DesignPattern.Domain.Common;
// using DesignPattern.Domain.Entities;
// using DesignPattern.Domain.ValueObjects;
// using DesignPattern.Domain.ValueObjects.BaseEntity;
// using DesignPattern.Domain.ValueObjects.Product;
// using FluentAssertions;
// using Xunit.Abstractions;

// namespace Test.Domain;

// public class ProductTest
// {
//   private readonly ITestOutputHelper _output;
//   private readonly JsonSerializerOptions _jsonOptions;
//   public ProductTest(ITestOutputHelper output)
//   {
//     _output = output;
//     _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
//   }

//   [Fact]
//   public void CreateProduct_Should_ReturnSuccessResult()
//   {
//     // Arrange
//     ProductName productName = ProductName.Create("Laptop").Value;
//     Price productPrice = Price.Create(999).Value;
//     ID productId = ID.Create("1234567890").Value;

//     // Act
//     Result<Product> productResult = Product.Create(
//         productId,
//         productName,
//         productPrice
//       );

//     if (productResult.IsFailure)
//     {
//       _output.WriteLine($"Tạo Product thất bại lỗi: {productResult.Error.Code} - {productResult.Error.Message}");
//     }

//     productResult.IsSuccess.Should().BeTrue();
//     string json = JsonSerializer.Serialize(productResult.Value, _jsonOptions);
//     _output.WriteLine($"Thông tin sản phẩm: {json}");
//   }

//   [Fact]
//   public void GetProductProperty_Should_ReturnCorrectValue()
//   {
//     int numOfProperty = typeof(Product).GetProperties().Length;
//     HashSet<PropertyInfo> types = Product.GetTypes();
//     int numOfType = types.Count;

//     _output.WriteLine($"Số lượng kiểu dữ liệu của thuộc tính trong Product: {numOfType}");
//     for (int i = 0; i < numOfType; i++)
//     {
//       _output.WriteLine($"Kiểu dữ liệu của thuộc tính {i + 1}: {types.ElementAt(i).Name}");
//     }

//     _output.WriteLine($"\nSố lượng thuộc tính của Product: {numOfProperty}");
//     for (int i = 0; i < numOfProperty; i++)
//     {
//       _output.WriteLine($"Tên thuộc tính {i + 1}: {typeof(Product).GetProperties()[i].Name}");
//     }

//     numOfType.Should().Be(numOfProperty);
//   }
// }