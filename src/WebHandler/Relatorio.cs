using Automation.Persistence;
namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Relatorio()
    {
      if(this.cfg.ENVIRONMENT)
      {
        var conf = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        var json = System.Text.Json.JsonSerializer.Serialize<List<Espelho>>(this.espelhos, conf);
        var filename = $"{this.cfg.DOWNFOLDER}\\{this.agora.ToString("yyyyMMdd_HHmmss")}_{this.balde_nome}.json";
        System.IO.File.WriteAllText(filename, json);
        System.Console.WriteLine($"{DateTime.Now} - Arquivo {filename} exportado!");
      }
      if(relatorios.Length > 0)
      {
        if(!cfg.BOT_CHANNELS.TryGetValue(this.balde_nome, out long channel)) return;
        this.relatorios.Insert(0, $"_Balde de recursos: *{this.balde_nome}*_\n\n");
        this.relatorios.Insert(0, "*MONITORAMENTO DE OFENSORES DO IDG*\n");
        this.relatorios.Append($"\n_Relatório extraído em {this.agora}_");
        Helpers.Telegram.SendMessage(channel, this.relatorios.ToString().Replace("-", "\\-"));
      }
      else
      {
        System.Console.WriteLine($"{DateTime.Now} - Nenhum ofensor ao IDG nesta análise!");
      }
    }
  }
}