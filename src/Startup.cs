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
    Presentation.Show();
    Thread.Sleep(TimeSpan.FromSeconds((int)WAITSEC.Curto));
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
        Configuration.LoadConf("ofs.conf");
        var operacao = Configuration.GetString("OPERACAO");
        if (operacao == "DELEGADOR")
        {
          Delegator.Run();
          break;
        }
        Updater.Update();
        InstanceChecker.MultipleRun();
        InstanceChecker.ChromeKiller();
        if (operacao == "MEGAZORD")
        {
          Megazord.Run();
          break;
        }
        using var handler = new WebHandler.WebHandler();
        Autenticador.Autenticar(handler);
        if (operacao == "RETRODAY")
        {
          Retroativo.Relatorios(handler);
          break;
        }
        if (operacao == "EXTRACAO")
        {
          MassiveInfo.GetMassiveInfo(handler);
          break;
        }
        if (operacao == "MONITORA")
        {
          Monitorador.Monitorar(handler);
          break;
        }
        if (operacao == "SERVIDOR")
        {
          var server = new WebServer(handler);
          server.Run();
          break;
        }
        throw new InvalidOperationException(
          $"O modo de operação '{operacao}' é inválido!");
      }
      catch (System.Exception erro)
      {
        Log.Error("Houve um problema crítico!");
        Log.Error(erro.Message);
        if (erro.StackTrace is not null)
          Log.Debug(erro.StackTrace);
        Executor.Reiniciar();
        break;
      }
    }
  }
}
