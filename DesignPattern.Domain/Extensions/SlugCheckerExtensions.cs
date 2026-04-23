using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects.Seo;

namespace DesignPattern.Domain.Extensions;

public static class SlugCheckerExtensions
{
  public static async Task<Result<string>> GenerateUniqueSlugAsync(this ISlugChecker slugChecker, string name, CancellationToken cancellationToken = default)
  {
    var baseSlug = Slug.GenerateFromName(name);
    if (baseSlug.IsFailure)
    {
      return Result<string>.Failure(baseSlug.Error);
    }

    const int maxAttempts = 50;
    for (var index = 0; index <= maxAttempts; index++)
    {
      // candidate = ứng viên
      var candidate = index == 0 ? baseSlug.Value : $"{baseSlug.Value}-{index}";
      var exists = await slugChecker.CheckSlugExistence(candidate, cancellationToken);
      if (!exists)
      {
        return Result<string>.Success(candidate);
      }
    }

    return Result<string>.Failure(Error.Failure(
      "Slug.GenerationFailed",
      "Failed to generate a unique slug."));
  }
}
