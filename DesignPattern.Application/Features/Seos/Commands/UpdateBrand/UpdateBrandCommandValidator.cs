using FluentValidation;
using DesignPattern.Application.Helpers;

namespace DesignPattern.Application.Features.Seos.Commands.UpdateBrand;

public sealed class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
{
  public UpdateBrandCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.dto).NotNull();

    When(x => x.dto is not null, () =>
    {
      RuleFor(x => x.dto.Name)
              .NotEmpty()
              .WithMessage("Tên thương hiệu là bắt buộc.")
              .MaximumLength(200);

      RuleFor(x => x.dto.Title)
              .NotEmpty()
              .WithMessage("Tiêu đề meta là bắt buộc.")
              .MaximumLength(200);

      RuleFor(x => x.dto.ParentBrandId)
              .IsNullOrValidGuid();
    });
  }
}
