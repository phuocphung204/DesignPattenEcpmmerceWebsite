using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.BaseEntity;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.Entities;

public class Rating : BaseEntity
{
  public Guid ProductId { get; private set; }
  public Guid UserId { get; private set; }
  public RatingValue Value { get; private set; } = RatingValue.Zero;
  public string Comment { get; private set; }

  private Rating(Guid productId, Guid userId, RatingValue value, string comment)
  {
    ProductId = productId;
    UserId = userId;
    Value = value;
    Comment = comment;
  }

  public static Result<Rating> Create(Guid productId, Guid userId, int ratingValue, string comment)
  {
    var ratingValueResult = RatingValue.Create(ratingValue);
    if (ratingValueResult.IsFailure)
      return ratingValueResult.Error;

    return new Rating(productId, userId, ratingValueResult.Value, comment);
  }
  public Result Update(int newValue, string newComment)
  {
    var ratingValueResult = RatingValue.Create(newValue);
    if (ratingValueResult.IsFailure)
      return ratingValueResult.Error;

    Value = ratingValueResult.Value;
    Comment = newComment;

    return true;
  }
}