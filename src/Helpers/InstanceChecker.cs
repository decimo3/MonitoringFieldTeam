using Serilog;
namespace MonitoringFieldTeam.Helpers;
public static class InstanceChecker
{

  public static void MultipleRun()
  {
    Log.Information("Verificando se há outras instâncias do sistema...");
    var result = Helpers.Executor.Executar("tasklist", $"/NH /FI \"IMAGENAME eq ofs.exe\"");
    if(((result.Length - result.Replace("ofs.exe", "").Length) / "ofs.exe".Length) > 1)
    {
      Log.Error("Já existe uma instância do sistema em execução!");
      System.Environment.Exit(409);
    }
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
