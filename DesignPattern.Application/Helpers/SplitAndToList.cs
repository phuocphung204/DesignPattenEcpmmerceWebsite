namespace DesignPattern.Application.Helpers;

public class SemicolonSeparatedListParser
{
  public static List<string> Parse(string input)
  {
    return input.Split("; ", StringSplitOptions.RemoveEmptyEntries)
      .Select(s => s.Trim())
      .ToList();
  }
}