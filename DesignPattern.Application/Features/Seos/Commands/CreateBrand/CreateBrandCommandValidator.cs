using FluentValidation;
using DesignPattern.Application.Helpers;

namespace DesignPattern.Application.Features.Seos.Commands.CreateBrand;

public sealed class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
  public CreateBrandCommandValidator()
  {
    RuleFor(x => x.dto)
        .NotNull()
        .WithMessage("Dữ liệu đầu vào thương hiệu là bắt buộc.");

    When(x => x.dto is not null, () =>
    {
      RuleFor(x => x.dto.Name)
              .NotEmpty()
              .WithMessage("Tên thương hiệu là bắt buộc.")
              .MaximumLength(200)
              .WithMessage("Tên thương hiệu không được vượt quá 200 ký tự.");

      RuleFor(x => x.dto.MetaDescription)
              .MaximumLength(200)
              .WithMessage("Mô tả meta không được vượt quá 200 ký tự.");

      RuleFor(x => x.dto.Title)
              .NotEmpty()
              .WithMessage("Tiêu đề meta là bắt buộc.")
              .MaximumLength(200)
              .WithMessage("Tiêu đề meta không được vượt quá 200 ký tự.");

      RuleFor(x => x.dto.ParentBrandId)
              .IsNullOrValidGuid()
              .WithMessage("ParentBrandId phải là GUID hợp lệ.");
    });
  }
}
