using System.Text.RegularExpressions;
using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.Persistence;

namespace MonitoringFieldTeam.WebScraper
{
  public static class Comparador
  {
    private const int TOLERANCIA = 3;
    public static List<FeedBack> Comparar(List<Espelho> espelhos, int horario_atual, int pixels_por_minuto)
    {
      var relatorios = new List<FeedBack>();
      foreach (var espelho in espelhos)
      {
        if (string.IsNullOrEmpty(espelho.recurso)) continue;
        var janela_final = espelho.shift_left + espelho.shift_width;
        var janela_tamanho = janela_final - espelho.shift_left;
        var notas_pendentes = espelho.servicos.Where(s =>
          s.data_activity_status == Servico.Status.pending ||
          s.data_activity_status == Servico.Status.enroute ||
          s.data_activity_status == Servico.Status.started
        ).OrderBy(r => r.start).ThenBy(s => s.dur).ToList();
        var nota_anterior = espelho.servicos.Where(s =>
          s.data_activity_status == Servico.Status.complete ||
          s.data_activity_status == Servico.Status.notdone
        ).OrderByDescending(s => s.start).ThenByDescending(s => s.dur).FirstOrDefault();
        var rota_atual = espelho.roteiros.OrderByDescending(r => r.start).ThenByDescending(r => r.dur).FirstOrDefault();
        var nota_atual = notas_pendentes.FirstOrDefault();
        // DONE - Verificar se o recurso já está na janela de horário
        if (horario_atual < espelho.shift_left && espelho.queue_start_start < 0) continue;
        if (horario_atual > janela_final && espelho.queue_end_start > 0) continue;
        // DONE - Verificar se a janela do recurso está com horário mínimo
        var prefixo_do_recurso = espelho.recurso[..^3];
        var numero_do_recurso = Int32.Parse(espelho.recurso[^3..]);
        Int32 minutos_esperados_para_a_janela = EncontrarChave(prefixo_do_recurso, numero_do_recurso);
        if (minutos_esperados_para_a_janela == 0) continue;
        if (espelho.queue_start_start > 0 && espelho.queue_end_start < 0)
        {
          if (espelho.shift_dur < minutos_esperados_para_a_janela)
          {
            relatorios.Add(new FeedBack(espelho.recurso, "jornada encurtada", espelho.shift_dur));
          }
          if (espelho.shift_dur > minutos_esperados_para_a_janela)
          {
            relatorios.Add(new FeedBack(espelho.recurso, "jornada extendida", espelho.shift_dur));
          }
        }
        // DONE - Verificar se o recurso já finalizou a rota e se ainda está na janela de horário
        if (espelho.queue_end_left > 0 && horario_atual < janela_final)
        {
          var diff = horario_atual - espelho.queue_end_left;
          relatorios.Add(new FeedBack(espelho.recurso, "deslogou antes", (int)(diff / pixels_por_minuto)));
          continue;
        }
        // DONE - Verificar se o recurso já está logado ou reativado
        if (espelho.queue_start_left < 0)
        {
          var diff = horario_atual - espelho.shift_left;
          if ((diff / pixels_por_minuto) > TOLERANCIA)
          {
            relatorios.Add(new FeedBack(espelho.recurso, "ainda não logou", (int)(diff / pixels_por_minuto)));
            continue;
          }
        }
        if (espelho.queue_start_left > 0)
        {
          if (horario_atual < espelho.shift_left)
          {
            var diff = espelho.shift_left - espelho.queue_start_left;
            relatorios.Add(new FeedBack(espelho.recurso, "logou antes", (int)(diff / pixels_por_minuto)));
          }
        }
        if (espelho.queue_end_left > 0) continue;
        // DONE - Verificar se o recurso ainda tem notas pendentes
        if (horario_atual < janela_final)
        {
          if (nota_atual == null)
          {
            relatorios.Add(new FeedBack(espelho.recurso, "equipe sem notas", null));
            continue;
          }
          if (notas_pendentes.Count() == 1)
          {
            var diff = horario_atual - (nota_atual.style_left + nota_atual.style_width);
            relatorios.Add(new FeedBack(espelho.recurso, "na última nota", (int)(diff / pixels_por_minuto)));
          }
        }
        // DONE - Verificar se o recurso tem alguma nota `enroute` ou `started`
        if (nota_atual == null || nota_atual.data_activity_status == Servico.Status.pending)
        {
          if (nota_anterior == null)
          {
            var diff = horario_atual - espelho.shift_left;
            relatorios.Add(new FeedBack(espelho.recurso, "equipe ociosa", (int)(diff / pixels_por_minuto)));
            continue;
          }
          else
          {
            var diff = horario_atual - (nota_anterior.style_left + nota_anterior.style_width);
            if (diff / pixels_por_minuto >= TOLERANCIA)
            {
              relatorios.Add(new FeedBack(espelho.recurso, "equipe ociosa", (int)(diff / pixels_por_minuto)));
              continue;
            }
          }
        }

        if (nota_atual != null && nota_atual.data_activity_status == Servico.Status.started)
        {
          if (horario_atual > nota_atual.style_left + nota_atual.style_width)
          {
            var diff = horario_atual - (nota_atual.style_left + nota_atual.style_width);
            relatorios.Add(new FeedBack(espelho.recurso, "atendimento atrasado", (int)(diff / pixels_por_minuto)));
          }
        }

        if (rota_atual == null)
        {
          var diff = horario_atual - espelho.shift_left;
          relatorios.Add(new FeedBack(espelho.recurso, "GPS desligado", (int)(diff / pixels_por_minuto)));
          continue;
        }
        var distancia_do_inicio_registro_de_rota = horario_atual - rota_atual.style_left;
        var distancia_do_final_registro_de_rota = horario_atual - (rota_atual.style_left + rota_atual.style_width);
        var minutos_do_inicio_registro_de_rota = Convert.ToInt32(distancia_do_inicio_registro_de_rota / pixels_por_minuto);
        var minutos_do_final_registro_de_rota = Convert.ToInt32(distancia_do_final_registro_de_rota / pixels_por_minuto);
        if (minutos_do_final_registro_de_rota >= TOLERANCIA)
        {
          relatorios.Add(new FeedBack(espelho.recurso, "GPS sem registro", minutos_do_final_registro_de_rota));
          continue;
        }
        if (rota_atual.status == Roteiro.Status.idle)
        {
          relatorios.Add(new FeedBack(espelho.recurso, "parada indevida", minutos_do_inicio_registro_de_rota));
          continue;
        }
        if (rota_atual.status == Roteiro.Status.alert)
        {
          if (nota_atual == null)
          {
            relatorios.Add(new FeedBack(espelho.recurso, "encerrou deslocando", minutos_do_inicio_registro_de_rota));
            continue;
          }
          if (nota_atual.data_activity_status == Servico.Status.started)
          {
            relatorios.Add(new FeedBack(espelho.recurso, "deslocamento indevido", minutos_do_inicio_registro_de_rota));
            continue;
          }
          if (nota_atual.data_activity_status == Servico.Status.enroute)
          {
            relatorios.Add(new FeedBack(espelho.recurso, "deslocamento atrasado", minutos_do_inicio_registro_de_rota));
            continue;
          }
        }
      }
      return relatorios;
    }
    public static Int32 EncontrarChave(String prefixo, Int32 numero)
    {
      var pairs = Configuration.GetPairs("HORARIO");
      var pair = pairs.FirstOrDefault(c => c.Key == prefixo);
      if (pair.Key is not null)
        return (int)pair.Value;
      var chaves = pairs.Select(p => p.Key).ToList();
      var padrao = @"^([A-Z]{3,5})[\[]([0-9]{1,2})\.([0-9]{1,2})[\]]$";
      var regex = new Regex(padrao);
      foreach (var kv in pairs)
      {
        var match = regex.Match(kv.Key);
        if (match != null && match.Groups[1].Value == prefixo)
        {
          if (numero >= Int32.Parse(match.Groups[2].Value))
          {
            if (numero <= Int32.Parse(match.Groups[3].Value))
            {
              return (int)kv.Value;
            }
          }
        }
      }
      return 0;
    }
  }
}
