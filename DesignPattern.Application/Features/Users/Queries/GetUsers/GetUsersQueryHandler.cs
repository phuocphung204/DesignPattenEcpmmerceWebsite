using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<UserResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;
  public GetUsersQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<List<UserResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
  {
    var users = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);
    return users.Select(UserResponse.FromDomain).ToList();
  }
}
