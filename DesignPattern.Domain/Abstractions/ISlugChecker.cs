namespace DesignPattern.Domain.Abstractions;

public interface ISlugChecker
{
  Task<bool> CheckSlugExistence(string slug, CancellationToken cancellationToken);
}