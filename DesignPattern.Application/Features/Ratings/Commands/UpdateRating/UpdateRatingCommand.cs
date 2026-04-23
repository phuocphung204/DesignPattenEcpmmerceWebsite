using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.Ratings.Shared;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Ratings.Commands.UpdateRating;

public record UpdateRatingDTO(
    int RatingValue,
    string Comment
);
public record UpdateRatingCommand(
    Guid id,
    UpdateRatingDTO dto
) : IRequest<Result<RatingResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
}
