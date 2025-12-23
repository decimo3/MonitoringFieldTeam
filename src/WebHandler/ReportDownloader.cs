using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.WebHandler;
using Serilog;
namespace MonitoringFieldTeam.WebScraper;

public static class ReportDownloader
{
  public static bool TemRelatorio(String bucketName, DateOnly date)
  {
    var datapath = Configuration.GetString("DATAPATH");
    var expectedFilename = $"Atividades-{bucketName}_{date.ToString("dd_MM_yy")}.csv";
    var expectedFilepath = System.IO.Path.Combine(datapath, expectedFilename);
    return System.IO.File.Exists(expectedFilepath);
  }
  public static string? Download(WebHandler.WebHandler handler, String bucketName, DateOnly date)
  {
    Parametrizador.VerificarPagina(handler);
    if (!handler.GetElements("GANNT_ACTIONBTN").Any())
    {
      Log.Error("Não é possível baixar relatório de um grupo!");
      return null;
    }
    var parent = handler.GetElement("GANNT_TOOLBAR");
    handler.GetNestedElements(parent, "GANNT_ACTIONBTN").First().Click();
    parent = handler.GetElement("GANNT_OPTIONSVIEW", WebHandler.WAITSEC.Curto);
    var download = handler.GetNestedElements(parent, "GANNT_EXPORTBTN").First();
    return handler.DownloadFile(download);
  }
}
