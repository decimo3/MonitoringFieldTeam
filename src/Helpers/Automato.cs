using Serilog;
using MonitoringFieldTeam.WebScraper;
namespace MonitoringFieldTeam.Helpers;

public static class Automato
{
  public const int MAX_SERVER_CONNECT_RETRY = 3;
  public static void Run(WebHandler.WebHandler handler)
  {
    try
    {
      var isReportAlreadyDownloaded = false;
      Parametrizador.VerificarPagina(handler);

      Log.Information("Verificando se já foi realizada a importação de relatórios...");
      using (var database = new Database())
      {
        var last = database.GetOrderList()
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault();
        if (last != null && last.CreatedAt.Date == DateTime.Today)
        {
          isReportAlreadyDownloaded = true;
          Log.Information("Os relatórios foram importados recentemente, pulando...");
        }
      }

      if (!isReportAlreadyDownloaded)
      {
        var date = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);
        if (date.DayOfWeek != DayOfWeek.Sunday)
        {
          Retroativo.TrocarData(handler, date);
          var buckets = Configuration.GetArray("RECURSO");
          Log.Information("Realizando o download dos baldes {baldes}...", buckets);
          foreach (var bucket in buckets)
          {
            Atualizador.SelecionarBalde(handler, bucket, true);
            Atualizador.Atualizar(handler);
            ReportDownloader.Download(handler, bucket, date);
          }
        }
      }

      Log.Information("Iniciando o servidor de coleta de informações...");
      using var server = new WebServer(handler);
      var task = Task.Run(() => server.Run());
      var fails = 0;
      Log.Information("Verificando se os servidores estão online...");
      while (fails <= MAX_SERVER_CONNECT_RETRY)
      {
        try { Delegator.GetOnlineWorkers(); break; }
        catch (Exception ex)
        {
          fails++;
          Log.Error("ERROR {count}: {error}. ", fails, ex.Message);
        }
      }
      Delegator.Run();
    }
    catch (Exception ex)
    {
      Log.Error("Ocorreu um erro durante a coleta: {errorMessage}", ex.Message);
      throw;
    }
  }
}
