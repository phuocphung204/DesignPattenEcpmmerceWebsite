using MediatR;
using DesignPattern.Domain.Common;

namespace DesignPattern.Application.Features.Carts.Commands.SyncCart;

public class SyncCartCommandHandler : IRequestHandler<SyncCartCommand, Result<bool>>
{
  public Task<Result<bool>> Handle(SyncCartCommand request, CancellationToken cancellationToken)
  {
    return Task.FromResult(Result<bool>.Success(true));
  }
}