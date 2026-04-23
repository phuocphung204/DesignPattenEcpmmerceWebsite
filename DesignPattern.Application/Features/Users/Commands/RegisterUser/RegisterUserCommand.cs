using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Users.Commands.RegisterUser;

public record RegisterUserCommand(RegisterUserDTO dto) : IRequest<Result>;
