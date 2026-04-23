using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Application.Features.Ratings.Shared;
using DesignPattern.Domain.Entities;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Ratings.Queries.GetRatingsByProductId;

public class GetRatingsByProductIdQueryHandler : IRequestHandler<GetRatingsByProductIdQuery, Result<List<RatingResponse>?>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetRatingsByProductIdQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<List<RatingResponse>?>> Handle(GetRatingsByProductIdQuery request, CancellationToken cancellationToken)
  {
    var ratings = await _unitOfWork.RatingRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
    if (ratings is null)
      return RatingErrors.RatingNotFound;
    var ratingResponses = ratings
    .Select(r => new RatingResponse(
      Id: r.Id,
      ProductId: r.ProductId,
      UserId: r.UserId,
      RatingValue: r.Value.Value,
      Comment: r.Comment
    )).ToList();

    return Result<List<RatingResponse>?>.Success(ratingResponses);
  }
}
