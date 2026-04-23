using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Entities;
using DesignPattern.Application.Features.Ratings.Shared;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Ratings.Commands.CreateRating;

public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, Result<RatingResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public CreateRatingCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<RatingResponse>> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId, cancellationToken);
    if (product is null)
      return ProductErrors.ProductNotFound;

    var rating = Rating.Create(
        dto.ProductId,
        request.UserId,
        dto.RatingValue,
        dto.Comment
    );
    if (rating.IsFailure)
      return rating.Error;

    var ratingValue = rating.Value;
    _unitOfWork.RatingRepository.Create(ratingValue);

    // Update rating in Product entity
    var ratings = await _unitOfWork.RatingRepository.GetByProductIdAsync(dto.ProductId, cancellationToken);
    ratings.Add(ratingValue); // Include the newly added rating in the calculation
    double averageRating = 0.0;
    if (ratings.Count == 0)
      averageRating = 0.0;
    else
      averageRating = ratings.Average(r => r.Value.Value);

    product.UpdateRating((decimal)averageRating);
    _unitOfWork.ProductRepository.Update(product);

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result<RatingResponse>.Success(new RatingResponse(
        Id: ratingValue.Id,
        ProductId: ratingValue.ProductId,
        UserId: ratingValue.UserId,
        RatingValue: ratingValue.Value.Value,
        Comment: ratingValue.Comment
    ));
  }

}