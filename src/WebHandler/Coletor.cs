using Serilog;
using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.Persistence;
namespace MonitoringFieldTeam.WebScraper;

public static class Coletor
{
  public static List<Espelho> Coletar(WebHandler.WebHandler handler)
  {
    var espelhos = new List<Espelho>();
    Log.Information("Coletando espelhos...");
    handler.GetElement("GANNT_CANVAS", WebHandler.WAITSEC.Total);
    // Coleta de nome de recurso e par_pid
    // var recursos = handler.GetElements("RECURSOS", WebHandler.WAITSEC.Agora);
    for(var i = 1; true; i++)
    {
      var recurso = handler.GetElements("RECURSO", WebHandler.WAITSEC.Agora, i).SingleOrDefault();
      if (recurso is null) break;
      var texto = handler.GetElementAttribute(recurso, "GLOBAL_TEXT");
      var par_pid = handler.GetElementAttribute(recurso, "COLETOR_PARPID");
      var style_top = handler.GetElementStyle(recurso)["top"];
      espelhos.Add(new Espelho(texto, Int32.Parse(par_pid), style_top));
    }
    if (!espelhos.Any())
    {
      Log.Error("O balde selecionado está vazio!");
      return new List<Espelho>();
    }
    // Pecorre a lista de linhas do gráfico Gantt
    Log.Information("Coletando atividades...");
    var timelines = handler.GetElements("ESPELHOS", WebHandler.WAITSEC.Agora);
    foreach (var timeline in timelines)
    {
      var style_top = handler.GetElementStyle(timeline)["top"];
      var espelho = espelhos.Single(s => s.style_top == style_top);
      if (handler.HasClassElement(timeline, "COLETOR_HORALINHA")) continue;
      if (handler.HasClassElement(timeline, "COLETOR_TIMELINE"))
      {
        var par_pid = Int32.Parse(handler.GetElementAttribute(timeline, "COLETOR_PARPID"));
        var atividades = handler.GetNestedElements(timeline, "GLOBAL_DIVCHILD");
        // Pecorre a lista de atividades filhos do elemento
        var j = 0;
        foreach (var atividade in atividades)
        {
          ProgressBar.SimpleProgressBar(j, atividades.Count, espelho.recurso);
          // Verifica se é uma ordem de atividade
          if (handler.HasClassElement(atividade, "COLETOR_SERVICO"))
          {
            if (handler.HasClassElement(atividade, "COLETOR_FINALROUTE"))
            {
              espelho.final_dur = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_DURATION"));
              j++;
              continue;
            }
            var servico = new Servico();
            var estilos = handler.GetElementStyle(atividade);
            servico.par_pid = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_PARPID"));
            servico.aid = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_IDENTIFIER").Split('|').Last());
            servico.par_date = DateOnly.Parse(handler.GetElementAttribute(atividade, "COLETOR_DATEONLY"));
            servico.ordered = Boolean.Parse(handler.GetElementAttribute(atividade, "COLETOR_ORDENADO"));
            servico.start = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_TIMESTART"));
            servico.dur = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_DURATION"));
            servico.movable = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_ISMOVABLE"));
            servico.multiday = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_ISMULTIDAY"));
            servico.data_activity_eta = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_ESTIMATE"));
            servico.data_activity_status = Enum.Parse<Servico.Status>(handler.GetElementAttribute(atividade, "COLETOR_STATUSES"));
            servico.data_activity_type = handler.GetElementAttribute(atividade, "COLETOR_TIPAGEM");
            servico.data_activity_worktype = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_WORKTYPE"));
            servico.data_activity_duration = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_DURATION2"));
            servico.style_left = estilos["left"];
            servico.style_width = estilos["width"];
            // Obtém informações do trajeto da nota
            var trajeto = handler.GetNestedElements(atividade, "GLOBAL_DIVCHILD").First();
            servico.travel_dur = Int32.Parse(handler.GetElementAttribute(trajeto, "COLETOR_DURATION"));
            servico.travel_style_width = handler.GetElementStyle(trajeto)["width"];
            servico.innerText = atividade.GetAttribute("innerText");
            espelho.servicos.Add(servico);
            j++;
            continue;
          }
          // Verifica se é uma janela de tempo
          if (handler.HasClassElement(atividade, "COLETOR_JANELATEMPO"))
          {
            espelho.shift_start = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_TIMESTART"));
            espelho.shift_dur = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_DURATION"));
            var estilos = handler.GetElementStyle(atividade);
            espelho.shift_left = estilos["left"];
            espelho.shift_width = estilos["width"];
            j++;
            continue;
          }
          // verifica se é uma alteração da jornada
          if (handler.HasClassElement(atividade, "COLETOR_GLOBALJOURNEY"))
          {
            if (handler.HasClassElement(atividade, "COLETOR_STARTJOURNEY"))
            {
              espelho.queue_start_start = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_TIMESTART"));
              espelho.queue_start_left = handler.GetElementStyle(atividade)["left"];
              j++;
              continue;
            }
            if (handler.HasClassElement(atividade, "COLETOR_RESTARTJOURNEY"))
            {
              espelho.queue_reactivated_start = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_TIMESTART"));
              espelho.queue_reactivated_left = handler.GetElementStyle(atividade)["left"];
              j++;
              continue;
            }
            if (handler.HasClassElement(atividade, "COLETOR_FINALJOURNEY"))
            {
              espelho.queue_end_start = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_TIMESTART"));
              espelho.queue_end_left = handler.GetElementStyle(atividade)["left"];
              j++;
              continue;
            }
          }
          // Verifica se é uma alteração na tempo
          if (handler.HasClassElement(atividade, "COLETOR_GLOBALTRACE"))
          {
            var roteiros = new Roteiro();
            roteiros.start = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_TIMESTART"));
            roteiros.dur = Int32.Parse(handler.GetElementAttribute(atividade, "COLETOR_DURATION"));
            if (handler.HasClassElement(atividade, "COLETOR_NORMALTRACE"))
              roteiros.status = Roteiro.Status.normal;
            if (handler.HasClassElement(atividade, "COLETOR_STOPEDTRACE"))
              roteiros.status = Roteiro.Status.idle;
            if (handler.HasClassElement(atividade, "COLETOR_ALERTTRACE"))
              roteiros.status = Roteiro.Status.alert;
            var estilos = handler.GetElementStyle(atividade);
            roteiros.style_width = estilos["width"];
            roteiros.style_left = estilos["left"];
            espelho.roteiros.Add(roteiros);
            j++;
            continue;
          }
          if (handler.HasClassElement(atividade, "COLETOR_GLOBALALERT"))
          {
            var estilos = handler.GetElementStyle(atividade);
            espelho.tw_alert_display = estilos["display"] != 0;
            espelho.tw_alert_left = estilos["left"];
            j++;
            continue;
          }
          j++;
        }
      }
    }
    return espelhos;
  }
}
