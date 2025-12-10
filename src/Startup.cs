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
    while(true)
    {
      try
      {
        var piscina_atual = cfg.PISCINAS[WebHandler.contador_de_baldes];
        Console.WriteLine($"{DateTime.Now} - Verificando solicitações...");
        if(WebHandler.Solicitacoes())
        {
          Console.WriteLine($"{DateTime.Now} - Solicitação respondida!");
          continue;
        }
        if(!WebHandler.TemFinalizacao(WebHandler.datalabel, piscina_atual))
        {
        WebHandler.VerificarPagina();
        Console.WriteLine($"{DateTime.Now} - Atualizando a página...");
        WebHandler.Atualizar(piscina_atual, true);
        Console.WriteLine($"{DateTime.Now} - Atualizando os parâmetros...");
        WebHandler.Parametrizar();
        if(WebHandler.Solicitacoes())
        {
          Console.WriteLine($"{DateTime.Now} - Solicitação respondida!");
          continue;
        }
        Console.WriteLine($"{DateTime.Now} - Coletando as informações...");
        if(WebHandler.Coletor())
        {
        Console.WriteLine($"{DateTime.Now} - Comparando os resultados...");
        WebHandler.Comparar();
        Console.WriteLine($"{DateTime.Now} - Exportando as análises...");
        WebHandler.Relatorio();
        Console.WriteLine($"{DateTime.Now} - Realizando análise final...");
        WebHandler.Finalizacao();
        if(cfg.ENVIRONMENT)
        {
          Console.WriteLine($"{DateTime.Now} - Realizando a captura de tela...");
          WebHandler.Fotografo();
        }
        }
        }
        WebHandler.Atualizar(piscina_atual, false);
        WebHandler.ProximoBalde();
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
