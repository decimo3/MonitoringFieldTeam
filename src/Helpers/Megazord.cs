using Serilog;
namespace MonitoringFieldTeam.Helpers;

/// <summary>
/// Class that instace multiples webservers to scalate vertically
/// the power of query on OFS system on each machine.
/// </summary>
public static class Megazord
{
  public static void Run()
  {
    var tasks = new List<Task>();
    var instanceUrls = Configuration.GetArray("WORKERS");
    var handlers = new MonitoringFieldTeam.WebHandler.WebHandler[instanceUrls.Length];
    var servers = new MonitoringFieldTeam.Helpers.WebServer[instanceUrls.Length];
    try
    {
      for (int i = 0; i < instanceUrls.Length; i++)
      {
        var index = i;
        handlers[index] = new MonitoringFieldTeam.WebHandler.WebHandler(index);
        MonitoringFieldTeam.WebScraper.Autenticador.Autenticar(handlers[index]);
        MonitoringFieldTeam.WebScraper.Parametrizador.VerificarPagina(handlers[index]);
        servers[index] = new MonitoringFieldTeam.Helpers.WebServer(handlers[index], instanceUrls[index]);
        tasks.Add(Task.Factory.StartNew(() => servers[index].Run(), TaskCreationOptions.LongRunning));
      }
      Task.WhenAll(tasks).GetAwaiter().GetResult();
    }
    catch (System.Exception error)
    {
      Log.Error("Ocorreu um erro! {erro}\n{trace}", error.Message, error.StackTrace);
    }
    finally
    {
      for (int i = 0; i < instanceUrls.Length; i++)
      {
        servers[i]?.Dispose();
        handlers[i]?.Dispose();
      }
    }
  }
}
