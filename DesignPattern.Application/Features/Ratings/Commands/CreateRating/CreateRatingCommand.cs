using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.Ratings.Shared;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Ratings.Commands.CreateRating;

public record CreateRatingDto(
  Guid ProductId,
  int RatingValue,
  string Comment
);
public record CreateRatingCommand(
  CreateRatingDto dto
) : IRequest<Result<RatingResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
}