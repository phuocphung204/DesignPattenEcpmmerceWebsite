using FluentValidation;
using DesignPattern.Application.Helpers;

namespace DesignPattern.Application.Features.Seos.Commands.UpdateProductCategory;

public sealed class UpdateProductCategoryCommandValidator : AbstractValidator<UpdateProductCategoryCommand>
{
  public UpdateProductCategoryCommandValidator()
  {
    RuleFor(x => x.Id)
      .NotEmpty()
      .WithMessage("Id danh mục là bắt buộc.");

    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Dữ liệu đầu vào là bắt buộc.");

    When(x => x.dto is not null, () =>
    {
      RuleFor(x => x.dto.Name)
        .NotEmpty()
        .WithMessage("Tên danh mục là bắt buộc.")
        .MaximumLength(200)
        .WithMessage("Tên danh mục không được vượt quá 200 ký tự.")
        .When(x => x.dto.Name != null);

      RuleFor(x => x.dto.ParentCategoryId)
        .IsNullOrValidGuid()
        .WithMessage("ParentCategoryId phải là GUID hợp lệ.");

      RuleFor(x => x.dto.MetaDescription)
        .MaximumLength(200)
        .WithMessage("Mô tả meta không được vượt quá 200 ký tự.")
        .When(x => x.dto.MetaDescription != null);

      RuleFor(x => x.dto.Title)
        .NotEmpty()
        .WithMessage("Tiêu đề meta là bắt buộc.")
        .MaximumLength(200)
        .WithMessage("Tiêu đề meta không được vượt quá 200 ký tự.")
        .When(x => x.dto.Title != null);

    });
  }
}
