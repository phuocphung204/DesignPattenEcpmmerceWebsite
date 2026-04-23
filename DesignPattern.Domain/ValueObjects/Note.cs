using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;

namespace DesignPattern.Domain.ValueObjects;

public sealed record class Note
{
  public string Value { get; init; }

  private Note(string value)
  {
    Value = value;
  }
  public static Result<Note> Create(string value)
  {
    if (value.Length > 500)
    {
      return Error.Validation(
        "Domain.Note.TooLong",
        "The note cannot exceed 500 characters.");
    }
    if (value.Length == 0)
    {
      return new Note("null");
    }
    return new Note(value);
  }
  public static Note Rehydrate(string value)
  {
    return new Note(value);
  }
}