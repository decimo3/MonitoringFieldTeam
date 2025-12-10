using MonitoringFieldTeam.Helpers;
using Serilog;
namespace MonitoringFieldTeam.WebScraper;

public static class Monitorador
{
  private static int contador_de_baldes = 0;
  public static void Monitorar(WebHandler.WebHandler handler)
  {
    var piscinas = Configuration.GetArray("RECURSO");
    var datapath = Configuration.GetString("DATAPATH");
    while (true)
    {
      var piscina = piscinas[contador_de_baldes];
      if (Finalizador.TemFinalizacao(datapath, DateOnly.FromDateTime(DateTime.Now), piscina))
      {
        contador_de_baldes = (contador_de_baldes + 1) % piscinas.Length;
        continue;
      }
      Parametrizador.VerificarPagina(handler);
      Log.Information("Atualizando a página...");
      var date = Atualizador.Atualizar(handler);
      Log.Information("Selecionando o balde {balde}...", piscina);
      var balde = Atualizador.SelecionarBalde(handler, piscina, true);
      Log.Information("Atualizando os parâmetros...");
      var hora_atual = Parametrizador.ObterHoraAtual(handler);
      var px_por_min = Parametrizador.ObterPixelsPorMinuto(handler);
      Log.Information("Obtendo a etiqueta de tempo...");
      var timestamp = DateTime.Now;
      Log.Information("Coletando as informações...");
      var espelhos = Coletor.Coletar(handler);
      Log.Information("Comparando os resultados...");
      var relatorios = Comparador.Comparar(espelhos, hora_atual, px_por_min);
      Log.Information("Exportando as análises...");
      var texto = Relator.Relatar(relatorios, piscina, timestamp);
      #if !DEBUG
      if (texto is not null) Telegram.SendMessage(balde, texto);
      #endif
      Log.Information("Realizando análise final...");
      var finalizacoes = Finalizador.Finalizacao(espelhos, date, true);
      if (!finalizacoes.Any())
      {
        contador_de_baldes = (contador_de_baldes + 1) % piscinas.Length;
        continue;
      }
      Log.Information("Serviços finalizados!");
      Log.Information("Criando relatório final...");
      var report_path = Finalizador.CreateReport(finalizacoes, balde, date);
      Log.Information("Enviando o relatório...");
      Telegram.SendDocument(balde, report_path);
      Log.Information("Realizando a captura de tela...");
      var print_path = System.IO.Path.Combine(datapath,
        $"{timestamp.ToString("yyyyMMdd_HHmmss")}_{balde}.png");
      handler.GetScreenshot(print_path);
      contador_de_baldes = (contador_de_baldes + 1) % piscinas.Length;
    }
  }
}
