using Serilog;
using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.WebHandler;
using MonitoringFieldTeam.WebScraper;
namespace MonitoringFieldTeam;

public class Startup
{
  public static void Main(string[] args)
  {
    #region
    var loglevel = Serilog.Events.LogEventLevel.Information;
#if DEBUG
    loglevel = Serilog.Events.LogEventLevel.Verbose;
#endif
    Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Verbose()
      .WriteTo.Console(loglevel)
      .WriteTo.File(System.IO.Path.Combine(
        System.AppContext.BaseDirectory, "logs", "ofs_.log"
        ), loglevel,
        rollingInterval: RollingInterval.Day)
      .CreateLogger();
      #endregion
    while (true)
    {
    try
    {
    Verificador.Verificar();
    var cfg = new Configuration();
    Updater.Update(cfg);
    using var WebHandler = new WebScraper.Manager(cfg);
    WebHandler.Autenticar();
    WebHandler.VerificarPagina();
    WebHandler.Retroativo();
    WebHandler.MassiveInfo();
      }
      catch (System.Exception erro)
      {
        Log.Error("Houve um problema crítico!");
        Log.Error(erro.Message);
        if (erro.StackTrace is not null)
          Log.Debug(erro.StackTrace);
#if !DEBUG
        Log.Information("Tentando reiniciar o sistema...");
        Executor.Reiniciar();
#endif
      }
    }
  }
}
