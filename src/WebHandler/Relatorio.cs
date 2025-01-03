using Automation.Persistence;
namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Relatorio()
    {
      if(relatorios.Length > 0)
      {
        this.relatorios = this.relatorios.Replace("-", "");
        this.relatorios.Insert(0, $"_Balde de recursos: *{this.balde_nome}*_\n\n");
        this.relatorios.Insert(0, "*MONITORAMENTO DE OFENSORES DO IDG*\n");
        this.relatorios.Append($"\n_Relatório extraído em {this.agora}_");
        var relatorio_string = this.relatorios.ToString();
        System.Console.WriteLine(relatorio_string);
        if(!cfg.BOT_CHANNELS.TryGetValue(this.balde_nome, out long channel)) return;
        Helpers.Telegram.SendMessage(cfg.CONFIGURACAO["BOT_TOKEN"], channel, relatorio_string.Replace("-", "\\-"));
      }
      else
      {
        System.Console.WriteLine($"{DateTime.Now} - Nenhum ofensor ao IDG nesta análise!");
      }
    }
  }
}
