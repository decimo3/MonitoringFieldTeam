using System.Text;
using Serilog;
using MonitoringFieldTeam.Persistence;
namespace MonitoringFieldTeam.WebScraper
{
  public static class Relator
  {
    public static string? Relatar(List<FeedBack> feedbacks, string balde_nome, DateTime agora)
    {
      if (!feedbacks.Any())
      {
        Log.Information("Nenhum ofensor do IDG nesta análise!");
        return null;
      }
      var relatorios = new StringBuilder();
      relatorios.Insert(0, $"_Balde de recursos: *{balde_nome}*_\n\n");
      relatorios.Insert(0, "*MONITORAMENTO DE OFENSORES DO IDG*\n");
      foreach (var feedback in feedbacks)
        relatorios.AppendLine(feedback.ToString());
      relatorios = relatorios.Replace("-", "");
      relatorios.Append($"\n\n_Relatório extraído em {agora}_");
      var relatorios_texto = relatorios.ToString();
      Log.Information("Lista de ofensores:\n{relatorios}", relatorios_texto);
      return relatorios_texto;
    }
  }
}
