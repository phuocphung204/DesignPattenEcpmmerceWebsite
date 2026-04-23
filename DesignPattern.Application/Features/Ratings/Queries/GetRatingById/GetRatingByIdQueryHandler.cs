using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Application.Features.Ratings.Shared;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Ratings.Queries.GetRatingById;

public class GetRatingByIdQueryHandler : IRequestHandler<GetRatingByIdQuery, Result<RatingResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetRatingByIdQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<RatingResponse>> Handle(GetRatingByIdQuery request, CancellationToken cancellationToken)
  {
    var rating = await _unitOfWork.RatingRepository.GetByIdAsync(request.RatingId, cancellationToken);
    if (rating is null)
      return new Error("Rating.NotFound", "Rating not found");

    return Result<RatingResponse>.Success(new RatingResponse(
      Id: rating.Id,
      ProductId: rating.ProductId,
      UserId: rating.UserId,
      RatingValue: rating.Value.Value,
      Comment: rating.Comment
    ));
  }
}
