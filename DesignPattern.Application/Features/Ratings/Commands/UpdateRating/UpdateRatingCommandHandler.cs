using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Application.Features.Ratings.Shared;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Ratings.Commands.UpdateRating;

public class UpdateRatingCommandHandler : IRequestHandler<UpdateRatingCommand, Result<RatingResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public UpdateRatingCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<RatingResponse>> Handle(UpdateRatingCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var rating = await _unitOfWork.RatingRepository.GetByIdAsync(request.id, cancellationToken);
    if (rating is null)
      return new Error("Rating.NotFound", "Rating not found");

    var updateResult = rating.Update(dto.RatingValue, dto.Comment);
    if (updateResult.IsFailure)
      return Result<RatingResponse>.Failure(updateResult.Error);

    _unitOfWork.RatingRepository.Update(rating);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result<RatingResponse>.Success(new RatingResponse(
      Id: rating.Id,
      ProductId: rating.ProductId,
      UserId: rating.UserId,
      RatingValue: rating.Value.Value,
      Comment: rating.Comment
    ));
  }
}
