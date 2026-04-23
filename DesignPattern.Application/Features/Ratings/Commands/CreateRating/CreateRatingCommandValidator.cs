using FluentValidation;

namespace DesignPattern.Application.Features.Ratings.Commands.CreateRating;

public sealed class CreateRatingCommandValidator : AbstractValidator<CreateRatingCommand>
{
  public CreateRatingCommandValidator()
  {
    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin đánh giá là bắt buộc.");

    RuleFor(x => x.dto)
      .SetValidator(new CreateRatingDtoValidator())
      .When(x => x.dto is not null);
  }
}

public sealed class CreateRatingDtoValidator : AbstractValidator<CreateRatingDto>
{
  public CreateRatingDtoValidator()
  {
    RuleFor(x => x.ProductId)
      .NotEmpty()
      .WithMessage("ProductId là bắt buộc.");

    RuleFor(x => x.RatingValue)
      .InclusiveBetween(1, 5)
      .WithMessage("Điểm đánh giá phải từ 1 đến 5.");

    RuleFor(x => x.Comment)
      .NotNull()
      .WithMessage("Nội dung đánh giá không được null.");
  }
}
