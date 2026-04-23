namespace DesignPattern.Application.Features.Ratings.Shared;

public record RatingResponse(
    Guid Id,
    Guid ProductId,
    Guid UserId,
    decimal RatingValue,
    string? Comment
);
