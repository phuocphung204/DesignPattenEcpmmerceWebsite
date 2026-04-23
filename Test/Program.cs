namespace Test;

public class Program
{
  List<Func<int, Task>> tasks = new List<Func<int, Task>>();

  public static async Task Main(string[] args)
  {
    await Task.WhenAll(
      Task1(),
      Task2()
    );
  }

  private static async Task Task1()
  {
    for (int i = 0; i < 5; i++)
    {
      Console.WriteLine($"Task1 - Iteration {i}");
      // await Task.Delay(1000);
    }
  }

  private static async Task Task2()
  {
    for (int i = 0; i < 5; i++)
    {
      Console.WriteLine($"Task2 - Iteration {i}");
      // await Task.Delay(1500);
    }
  }

}