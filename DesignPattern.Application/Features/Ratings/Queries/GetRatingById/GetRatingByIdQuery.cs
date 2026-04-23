using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Ratings.Shared;

namespace DesignPattern.Application.Features.Ratings.Queries.GetRatingById;

public record GetRatingByIdQuery(
    Guid RatingId
) : IRequest<Result<RatingResponse>>;
