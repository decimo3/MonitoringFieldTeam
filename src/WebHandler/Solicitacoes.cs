using Serilog;
using MonitoringFieldTeam.Helpers;
namespace MonitoringFieldTeam.WebScraper
{
  public static class WorkGetterHandler
  {
    private const string ERROR_MESSAGE = "A solicitação enviada é inválida, verifique e envie novamente!";
    private static readonly string LOCKFILE = System.IO.Path.Combine(System.AppContext.BaseDirectory, "ofs.lock");
    private static void WriteLockFile(string message)
    {
      File.WriteAllText(LOCKFILE, message, System.Text.Encoding.UTF8);
      Log.Information(message);
    }
    public static void Solicitacoes(WebHandler.WebHandler handler)
    {
      try
      {
        if (!System.IO.File.Exists(LOCKFILE)) return;
        var solicitacao = System.IO.File.ReadAllText(LOCKFILE, System.Text.Encoding.UTF8);
        if (solicitacao.Length == 0 || solicitacao.Length > 50) return;
        Log.Information("Solicitação recebida: {solicitacao}.", solicitacao);
        var args = solicitacao.Split(' ');
        if (args.Length != 2) throw new ArgumentException(ERROR_MESSAGE);
        var aplicacao = args[0];
        var informacao = long.Parse(args[1]);
        var workHandler = new ServicoHandler(handler, informacao);
        switch (aplicacao)
        {
          case "evidencia":
            WriteLockFile(workHandler.GetServico());
            break;
          default:
            throw new InvalidOperationException(ERROR_MESSAGE);
            break;
        }
      }
      catch (System.Exception erro)
      {
        WriteLockFile(erro.Message);
      }
    }
  }
}
