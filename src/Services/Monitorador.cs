using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.Persistence;
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
      if (Finalizador.TemFinalizacao(DateOnly.FromDateTime(DateTime.Now), piscina))
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
      var texto = Relatar(relatorios, piscina, timestamp);
      #if !DEBUG
        if (texto is not null)
          Telegram.SendMessage(balde, texto);
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
      contador_de_baldes = (contador_de_baldes + 1) % piscinas.Length;
    }
  }
  public static string? Relatar(List<FeedBack> feedbacks, string balde_nome, DateTime agora)
  {
    if (!feedbacks.Any())
    {
      Log.Information("Nenhum ofensor do IDG nesta análise!");
      return null;
    }
    var relatorios = new System.Text.StringBuilder();
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
