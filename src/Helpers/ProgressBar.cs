namespace MonitoringFieldTeam.Helpers;
public static class ProgressBar
{
  public static void SimpleProgressBar(Int32 atual, Int32 maximo, String prefixo)
  {
    var i = (Int32)Math.Ceiling((Double)atual / (Double)maximo * 100);
    var s = i < 10 ? "  " : i < 100 ? " " : "";
    var j = atual < 10 ? 2 : atual < 100 ? 1 : 0;
    var k = maximo < 10 ? 2 : maximo < 100 ? 1 : 0;
    var x = $"{prefixo} {s}{i}% " +
        $"[{new String('#', i)}{new String(' ', 100 - i)}] " +
        $"{new String(' ', j)}{atual}/{new String(' ', k)}{maximo}";
    Console.Write(atual == maximo ? x + '\n' : x + '\r');
  }
}
