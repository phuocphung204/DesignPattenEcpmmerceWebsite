using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.DeleteProductCategory;

public record DeleteProductCategoryCommand(Guid Id) : IRequest<Result>;
