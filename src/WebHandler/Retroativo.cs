using Serilog;
using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.WebHandler;
namespace MonitoringFieldTeam.WebScraper;

public static class Retroativo
{
  public static void Relatorios(WebHandler.WebHandler handler)
  {
    Parametrizador.VerificarPagina(handler);
    Log.Information("Verificando relatórios retroativos...");
    var now = DateOnly.FromDateTime(DateTime.Now);
    var startdate = DateTime.ParseExact(Configuration.GetString("RETRODAY"),
      "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
    var dias_retroativos = -(DateTime.Now - startdate).Days;
    for (var dia = now.AddDays(dias_retroativos); dia < now; dia = dia.AddDays(1))
    {
      if (dia.DayOfWeek == DayOfWeek.Sunday) continue;
      foreach (var piscina in Configuration.GetArray("RECURSO"))
      {
        var balde = piscina.Split('>').Last();
        if (Finalizador.TemFinalizacao(dia, balde) && ReportDownloader.TemRelatorio(balde, dia))
          continue;
        TrocarData(handler, dia);
        Atualizador.SelecionarBalde(handler, piscina, true);
        if (!Finalizador.TemFinalizacao(dia, balde))
        {
          Log.Information("Finalização retroativa: balde '{balde}', data {data}.", balde, dia);
          var espelhos = Coletor.Coletar(handler);
          var relatorios = Finalizador.Finalizacao(espelhos, dia, false);
          var filename = Finalizador.CreateReport(relatorios, balde, dia);
          #if !DEBUG
            Telegram.SendDocument(balde, filename);
          #endif
        }
        if (!ReportDownloader.TemRelatorio(balde, dia))
        {
          Log.Information("Relatório retroativo: balde '{balde}', data {data}.", balde, dia);
          var reportpath = ReportDownloader.Download(handler, balde, dia);
          #if !DEBUG
            Telegram.SendDocument(balde, reportpath);
          #endif
        }
        Atualizador.SelecionarBalde(handler, piscina, false);
      }
    }
    TrocarData(handler, now);
  }
  public static DateOnly TrocarData(WebHandler.WebHandler handler, DateOnly data)
  {
    const int QUANTIDADE_MAXIMA_DE_PAGINAS_DE_CALENDARIOS = 2;
    var data_atual = DateOnly.Parse(handler.GetElement("GANNT_DATEPICK", WAITSEC.Agora).Text);
    if (data_atual == data) return data_atual;

    var datepicker = handler.GetElements("GANNT_DATERANGE", WAITSEC.Curto);
    if (!datepicker.Any()) handler.GetElement("GANNT_DATEPICK", WAITSEC.Curto).Click();
    for (int i = 0; i < QUANTIDADE_MAXIMA_DE_PAGINAS_DE_CALENDARIOS; i++)
    {
      var calendarios = handler.GetElements("GANNT_DATEGROUP", WAITSEC.Curto);
      foreach (var calendario in calendarios)
      {
        var desired_monthstring = data.ToString("MMMM yyyy").ToLower();
        var current_monthstring = handler.GetNestedElements(calendario, "GANNT_DATETEXT").First().Text.ToLower();
        if (current_monthstring == desired_monthstring)
        {
          foreach (var sem in handler.GetNestedElements(calendario, "GANNT_DATEWEEK"))
          {
            foreach (var dia in handler.GetNestedElements(sem, "GLOBAL_TABLECEL"))
            {
              if (!Int32.TryParse(dia.Text, out Int32 dia_num)) continue;
              if (dia_num == data.Day)
              {
                dia.Click();
                return DateOnly.Parse(handler.GetElement("GANNT_DATEPICK", WAITSEC.Curto).Text);
              }
            }
          }
        }
      }
      handler.GetElement("GANNT_DATEPREV", WAITSEC.Agora).Click();
    }
    throw new InvalidOperationException($"A data '{data}' não pode ser selecionada!");
  }
}
