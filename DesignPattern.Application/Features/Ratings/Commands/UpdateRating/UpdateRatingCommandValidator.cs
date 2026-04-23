using FluentValidation;

namespace DesignPattern.Application.Features.Ratings.Commands.UpdateRating;

public sealed class UpdateRatingCommandValidator : AbstractValidator<UpdateRatingCommand>
{
  public UpdateRatingCommandValidator()
  {
    RuleFor(x => x.id)
      .NotEmpty()
      .WithMessage("Id đánh giá là bắt buộc.");

    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin cập nhật đánh giá là bắt buộc.");

    RuleFor(x => x.dto)
      .SetValidator(new UpdateRatingDtoValidator())
      .When(x => x.dto is not null);
  }
}

public sealed class UpdateRatingDtoValidator : AbstractValidator<UpdateRatingDTO>
{
  public UpdateRatingDtoValidator()
  {
    RuleFor(x => x.RatingValue)
      .InclusiveBetween(1, 5)
      .WithMessage("Điểm đánh giá phải từ 1 đến 5.");
  }
}
