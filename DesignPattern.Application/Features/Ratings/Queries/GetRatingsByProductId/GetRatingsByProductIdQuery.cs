using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Ratings.Shared;

namespace DesignPattern.Application.Features.Ratings.Queries.GetRatingsByProductId;

public record GetRatingsByProductIdQuery(
    Guid ProductId
) : IRequest<Result<List<RatingResponse>?>>;
