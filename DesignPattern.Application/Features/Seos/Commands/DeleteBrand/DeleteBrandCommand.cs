using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.DeleteBrand;

public record DeleteBrandCommand(Guid Id) : IRequest<Result>;
