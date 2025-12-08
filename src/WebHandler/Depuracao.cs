using System.Text.Json;
using MonitoringFieldTeam.Persistence;
namespace MonitoringFieldTeam.WebScraper
{
  public partial class Manager
  {
    public void ExportarPropriedadesEspelhos()
    {
      var conf = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
      var json = System.Text.Json.JsonSerializer.Serialize<List<Espelho>>(this.espelhos, conf);
      var filename = $"{this.cfg.DOWNFOLDER}\\{this.agora.ToString("yyyyMMdd_HHmmss")}_{this.balde_nome}.json";
      System.IO.File.WriteAllText(filename, json);
      System.Console.WriteLine($"{DateTime.Now} - Arquivo {filename} exportado!");
    }
  }
}
