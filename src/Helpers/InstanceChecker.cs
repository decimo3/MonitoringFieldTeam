using Serilog;
namespace MonitoringFieldTeam.Helpers;
public static class Verificador
{
  public static void Verificar()
  {
    System.Threading.Thread.Sleep(5_000);
    Console.WriteLine($"{DateTime.Now} - Verificando se há outras instâncias do sistema...");
    var result = Helpers.Executor.Executar("tasklist", $"/NH /FI \"IMAGENAME eq ofs.exe\"");
    if(((result.Length - result.Replace("ofs.exe", "").Length) / "ofs.exe".Length) > 1)
    {
      Console.WriteLine($"{DateTime.Now} - Já existe uma instância do sistema em execução!");
      System.Environment.Exit(409);
    }
  public static void ChromeKiller()
  {
    Log.Information("Finalizando processos residuais (se houver)...");
    var exec2kill = new List<String> {"chrome.exe", "chromedriver.exe"};
    foreach (var exec in exec2kill)
    {
      try
      {
        Helpers.Executor.Executar("taskkill", $"/F /T /IM {exec}");
      }
      catch (System.Exception erro)
      {
        Log.Error(erro.Message);
      }
    }
  }
}
