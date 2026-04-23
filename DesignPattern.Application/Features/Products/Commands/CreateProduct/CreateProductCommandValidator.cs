using DesignPattern.Domain.Enums;
using FluentValidation;

namespace DesignPattern.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
  public CreateProductCommandValidator()
  {
    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin sản phẩm là bắt buộc.");

    RuleFor(x => x.dto)
      .SetValidator(new CreateProductDtoValidator())
      .When(x => x.dto is not null);
  }
}

public sealed class CreateProductDtoValidator : AbstractValidator<CreateProductDTO>
{
  public CreateProductDtoValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Tên sản phẩm là bắt buộc.")
      .MaximumLength(100).WithMessage("Tên sản phẩm không được vượt quá 100 ký tự.");

    RuleFor(x => x.BrandId)
      .NotEmpty().WithMessage("BrandId là bắt buộc.");

    RuleFor(x => x.CategoryId)
      .NotEmpty().WithMessage("CategoryId là bắt buộc.");

    RuleFor(x => x.BrandName)
      .NotEmpty().WithMessage("Tên thương hiệu là bắt buộc.")
      .MaximumLength(100).WithMessage("Tên thương hiệu không được vượt quá 100 ký tự.");

    RuleFor(x => x.CategoryName)
      .NotEmpty().WithMessage("Tên danh mục là bắt buộc.")
      .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự.");

    RuleFor(x => x.VariantGroupId)
      .NotEmpty().WithMessage("VariantGroupId là bắt buộc.")
      .MaximumLength(64).WithMessage("VariantGroupId không được vượt quá 64 ký tự.");

    RuleFor(x => x.Sku)
      .NotEmpty().WithMessage("Sku là bắt buộc.")
      .MaximumLength(64).WithMessage("Sku không được vượt quá 64 ký tự.");

    RuleFor(x => x.SellingPrice)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Giá bán phải lớn hơn hoặc bằng 0.");

    RuleFor(x => x.PurchasePrice)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Giá nhập phải lớn hơn hoặc bằng 0.");

    RuleFor(x => x.StockQuantity)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Số lượng tồn kho phải lớn hơn hoặc bằng 0.");

    RuleFor(x => x.ShortDescription)
      .NotEmpty().WithMessage("Mô tả ngắn là bắt buộc.");

    RuleFor(x => x.DetailDescription)
      .NotEmpty().WithMessage("Mô tả chi tiết là bắt buộc.");

    RuleFor(x => x.Images)
      .NotNull().WithMessage("Danh sách ảnh là bắt buộc.");

    RuleForEach(x => x.Images)
      .NotEmpty().WithMessage("Link ảnh không được để trống.")
      .MaximumLength(2048).WithMessage("Link ảnh không được vượt quá 2048 ký tự.")
      .When(x => x.Images is not null);

    RuleFor(x => x.Attributes)
      .NotNull().WithMessage("Danh sách thuộc tính là bắt buộc.");

    RuleForEach(x => x.Attributes)
      .SetValidator(new CreateProductAttributeDtoValidator())
      .When(x => x.Attributes is not null);

    RuleFor(x => x.Title)
      .NotEmpty().WithMessage("Seo title không được để trống.")
      .MaximumLength(255).WithMessage("Seo title không được vượt quá 255 ký tự.")
      .When(x => x.Title is not null);

    RuleFor(x => x.MetaDescription)
      .MaximumLength(200).WithMessage("Seo description không được vượt quá 200 ký tự.")
      .When(x => x.MetaDescription is not null);
  }
}

public sealed class CreateProductAttributeDtoValidator : AbstractValidator<ProductAttributeDto>
{
  public CreateProductAttributeDtoValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Tên thuộc tính là bắt buộc.")
      .MaximumLength(100).WithMessage("Tên thuộc tính không được vượt quá 100 ký tự.");

    RuleFor(x => x.Value)
      .NotEmpty().WithMessage("Giá trị thuộc tính là bắt buộc.")
      .MaximumLength(64).WithMessage("Giá trị thuộc tính không được vượt quá 64 ký tự.");

    RuleFor(x => x.Type)
      .IsInEnum()
      .WithMessage("Loại thuộc tính không hợp lệ.");
  }
}
