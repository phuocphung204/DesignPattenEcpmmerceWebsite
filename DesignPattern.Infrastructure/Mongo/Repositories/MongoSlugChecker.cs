using DesignPattern.Domain.Abstractions;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.Repositories;

public class MongoSlugChecker : ISlugChecker
{
  private readonly MongoContext _context;

  public MongoSlugChecker(MongoContext context)
  {
    _context = context;
  }

  public async Task<bool> CheckSlugExistence(string slug, CancellationToken cancellationToken)
  {
    var exists = await _context.GetCollection<SeoDocument>("seos")
      .Find<SeoDocument>(s => s.Slug == slug)
      .AnyAsync(cancellationToken);

    return exists;
  }
}
