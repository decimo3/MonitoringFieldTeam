using Automation.Persistence;
namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Relatorio()
    {
      {
        var conf = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        var json = System.Text.Json.JsonSerializer.Serialize<List<Espelho>>(this.espelhos, conf);
        var filename = $"./ofs/{this.agora.ToString("yyyyMMdd_HHmmss")}_{this.configuration.recurso}.json";
        System.IO.File.WriteAllText(filename, json);
        System.Console.WriteLine($"{DateTime.Now} - Arquivo {filename} exportado!");
      }
      {
        var string_builder = new System.Text.StringBuilder();
        string_builder.Append("*Monitoramento de ofensores do IDG*\n\n");
        foreach (var relatorio in this.relatorios)
        {
          string_builder.Append(relatorio.Key);
          string_builder.Append(" ");
          string_builder.Append(relatorio.Value);
          string_builder.Append('\n');
        }
        string_builder.Append($"\nRelatório extraído em {this.agora}");
        var texto = string_builder.ToString();
        var filename = $"./ofs/{this.agora.ToString("yyyyMMdd_HHmmss")}_{this.configuration.recurso}.txt";
        System.IO.File.WriteAllText(filename, texto);
        System.Console.WriteLine($"{DateTime.Now} - Resultado das análises:");
        System.Console.WriteLine(texto);
      }
      this.espelhos = new();
      this.relatorios = new();
    }
  }
}