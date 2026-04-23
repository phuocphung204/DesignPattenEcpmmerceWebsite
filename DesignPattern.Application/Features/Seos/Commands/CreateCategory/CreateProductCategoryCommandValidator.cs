using FluentValidation;
using DesignPattern.Application.Helpers;

namespace DesignPattern.Application.Features.Seos.Commands.CreateCategory;

public sealed class CreateProductCategoryCommandValidator : AbstractValidator<CreateProductCategoryCommand>
{
  public CreateProductCategoryCommandValidator()
  {
    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Dữ liệu đầu vào là bắt buộc.");

    When(x => x.dto is not null, () =>
    {
      RuleFor(x => x.dto.Name)
        .NotEmpty()
        .WithMessage("Tên danh mục là bắt buộc.")
        .MaximumLength(200)
        .WithMessage("Tên danh mục không được vượt quá 200 ký tự.");

      RuleFor(x => x.dto.MetaDescription)
        .MaximumLength(200)
        .WithMessage("Mô tả meta không được vượt quá 200 ký tự.");

      RuleFor(x => x.dto.Title)
        .NotEmpty()
        .WithMessage("Tiêu đề meta là bắt buộc.")
        .MaximumLength(200)
        .WithMessage("Tiêu đề meta không được vượt quá 200 ký tự.");

      RuleFor(x => x.dto.ParentCategoryId)
        .IsNullOrValidGuid()
        .WithMessage("ParentCategoryId phải là GUID hợp lệ.");
    });
  }
}
