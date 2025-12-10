using System.Text.Json;
using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.Persistence;
namespace MonitoringFieldTeam.WebScraper
{
  public static class Exportar
  {
    public static void ExportarPropriedadesEspelhos(List<Espelho> espelhos, string balde_nome)
    {
      var conf = new JsonSerializerOptions { WriteIndented = true };
      var json = JsonSerializer.Serialize(espelhos, conf);
      var filename = Path.Combine(Configuration.GetString("DATAPATH"),
        $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}_{balde_nome}.json");
      File.WriteAllText(filename, json);
    }
  }
}
