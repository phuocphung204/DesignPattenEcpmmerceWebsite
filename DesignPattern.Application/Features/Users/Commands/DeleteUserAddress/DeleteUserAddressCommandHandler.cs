using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Users.Commands.DeleteUserAddress;

public sealed class DeleteUserAddressCommandHandler
  : IRequestHandler<DeleteUserAddressCommand, Result>
{
  private readonly IUnitOfWork _unitOfWork;

  public DeleteUserAddressCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> Handle(DeleteUserAddressCommand request, CancellationToken cancellationToken)
  {
    var user = await _unitOfWork.UserRepository.GetAddressesByUserIdAsync(request.UserId, cancellationToken);
    if (user is null)
      return UserErrors.UserNotFound;
    // Lưu snapshot trong order rồi nên không cần kiểm tra address có bị tham chiếu không
    var removeResult = user.RemoveAddress(request.AddressId);
    if (removeResult.IsFailure)
      return removeResult.Error;

    _unitOfWork.UserRepository.Update(user);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }
}
