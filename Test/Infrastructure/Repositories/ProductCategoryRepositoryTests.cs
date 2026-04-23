using DesignPattern.Domain.Entities.ProductCategory;
using DesignPattern.Infrastructure.Mongo;
using DesignPattern.Infrastructure.Mongo.Documents;
using DesignPattern.Infrastructure.Mongo.Repositories;
using FluentAssertions;
using MongoDB.Driver;
using Test.Infrastructure.Fixtures;
using Xunit;

namespace Test.Infrastructure.Repositories;

[Collection("MongoDB Collection")]
public class ProductCategoryRepositoryTests : BaseRepositoryTest
{
  private readonly MongoDbFixture _fixture;
  private readonly MongoContext _context;
  private readonly ProductCategoryRepository _repository;

  public ProductCategoryRepositoryTests(MongoDbFixture fixture)
  {
    _fixture = fixture;
    _context = new MongoContext(_fixture.Client, _fixture.Database);

    // Khởi tạo repository với db thật và mapper config dùng chung
    _repository = new ProductCategoryRepository(_context, Mapper);
  }

  [DockerFact]
  public async Task GetAllAsync_ShouldReturnData_WhenCollectionHasDocuments()
  {
    // Arrange
    var collection = _fixture.Database.GetCollection<ProductCategoryDocument>("productCategories");

    // Clear dữ liệu cũ trước khi test nếu cần
    await collection.DeleteManyAsync(FilterDefinition<ProductCategoryDocument>.Empty);

    var sampleDoc = new ProductCategoryDocument
    {
      Id = Guid.NewGuid(),
      Name = "Test Category",
      Level = 1,
      ParentCategoryId = null
    };
    await collection.InsertOneAsync(sampleDoc);

    // Act
    var result = await _repository.GetAllAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(1);
    result.First().Id.Should().Be(sampleDoc.Id);
    result.First().Name.Should().Be("Test Category");
  }

  [DockerFact]
  public async Task GetPagedAsyncV2_ShouldReturnPagedResult_WithCorrectFiltering()
  {
    // Arrange
    var collection = _fixture.Database.GetCollection<ProductCategoryDocument>("productCategories");
    await collection.DeleteManyAsync(FilterDefinition<ProductCategoryDocument>.Empty);

    var docs = new List<ProductCategoryDocument>
    {
      new ProductCategoryDocument { Id = Guid.NewGuid(), Name = "Category A", Level = 1 },
      new ProductCategoryDocument { Id = Guid.NewGuid(), Name = "Category B", Level = 1 },
      new ProductCategoryDocument { Id = Guid.NewGuid(), Name = "Category C", Level = 2 },
      new ProductCategoryDocument { Id = Guid.NewGuid(), Name = "Category D", Level = 1 },
      new ProductCategoryDocument { Id = Guid.NewGuid(), Name = "Category E", Level = 2 }
    };
    await collection.InsertManyAsync(docs);

    // Act
    // Filter level == 1 (3 items), Page 1, PageSize 2
    var result = await _repository.GetPagedAsyncV2(
        selector: x => x.Name,
        predicate: x => x.Level!.Value == 1,
        orderBy: null,
        pageIndex: 1,
        pageSize: 2
    );

    // Assert
    result.Should().NotBeNull();
    result.TotalCount.Should().Be(3);
    result.Items.Should().HaveCount(2); // Page 1 limits to 2
    result.PageIndex.Should().Be(1);
    result.PageSize.Should().Be(2);

    // Act - Page 2
    var resultPage2 = await _repository.GetPagedAsyncV2(
        selector: x => x.Name,
        predicate: x => x.Level!.Value == 1,
        orderBy: null,
        pageIndex: 2,
        pageSize: 2
    );

    // Assert
    resultPage2.Items.Should().HaveCount(1); // Page 2 has the remaining 1
  }

}
