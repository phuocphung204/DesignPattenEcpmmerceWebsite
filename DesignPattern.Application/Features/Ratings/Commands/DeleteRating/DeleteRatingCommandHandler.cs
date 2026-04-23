using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Ratings.Commands.DeleteRating;

public class DeleteRatingCommandHandler : IRequestHandler<DeleteRatingCommand, Result>
{
  private readonly IUnitOfWork _unitOfWork;

  public DeleteRatingCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
  {
    var rating = await _unitOfWork.RatingRepository.GetByIdAsync(request.RatingId, cancellationToken);
    if (rating is null)
      return new Error("Rating.NotFound", "Rating not found");

    // Check authorization - user can only delete their own ratings
    if (rating.UserId != request.UserId)
      return Error.Unauthorized("Rating.Unauthorized", "You are not authorized to delete this rating");

    _unitOfWork.RatingRepository.Delete(rating);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }
}
