namespace MonitoringFieldTeam.Helpers;

public static class Presentation
{
  private const char BANNER_CHAR = '#';
  private const int MAX_LENGTH = 100;
  public static void LineRow()
  {
    Console.WriteLine(new string('#', MAX_LENGTH));
  }
  public static void Center(string text)
  {
    var numblk = ((MAX_LENGTH - text.Length) / 2) - 2;
    var spaces = new string(' ', numblk);
    Console.WriteLine(BANNER_CHAR + spaces + text + spaces + BANNER_CHAR);
  }
  public static void Show()
  {
    LineRow();
    Center("Programa de automação do OFS do MestreRuan!");
    Center("Repositório: https://github.com/decimo3/MonitoringFieldTeam");
    LineRow();
  }
}
