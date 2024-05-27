using Automation.Persistence;
namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Relatorio()
    {
      if(this.configuration.is_development)
      {
        var conf = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        var json = System.Text.Json.JsonSerializer.Serialize<List<Espelho>>(this.espelhos, conf);
        var filename = $"./tmp/{this.agora.ToString("yyyyMMdd_HHmmss")}_{this.configuration.recurso}.json";
        System.IO.File.WriteAllText(filename, json);
        System.Console.WriteLine($"{DateTime.Now} - Arquivo {filename} exportado!");
      }
      if(relatorios.Length > 0)
      {
        var balde_nome = this.configuration.recurso[this.configuration.contador_de_baldes];
        this.relatorios.Insert(0, $"_Balde de recursos: *{balde_nome}*_\n\n");
        this.relatorios.Insert(0, "*MONITORAMENTO DE OFENSORES DO IDG*\n");
        this.relatorios.Append($"\n_Relatório extraído em {this.agora}_");
        var texto = this.relatorios.ToString();
        var filename = this.configuration.is_development ? $"./tmp/{this.agora.ToString("yyyyMMdd_HHmmss")}_{this.configuration.recurso}.txt" : "relatorio_ofs.txt";
        System.IO.File.WriteAllText(filename, texto);
        System.Console.WriteLine($"{DateTime.Now} - Resultado das análises:");
        System.Console.WriteLine(texto);
      }
      else
      {
        System.Console.WriteLine($"{DateTime.Now} - Nenhum ofensor ao IDG nesta análise!");
      }
      this.configuration.contador_de_baldes = (this.configuration.contador_de_baldes + 1) % this.configuration.recurso.Count;
    }
  }
}