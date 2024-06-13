using System.Text.RegularExpressions;
using Automation.Persistence;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Comparar()
    {
      foreach (var espelho in this.espelhos)
      {
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
        if(this.horario_atual < espelho.shift_left && espelho.queue_start_start < 0) continue;
        if(this.horario_atual > janela_final && espelho.queue_end_start > 0) continue;
        // DONE - Verificar se a janela do recurso está com horário mínimo
        Int32 minutos_esperados_para_a_janela = 0;
        var prefixo_do_recurso = espelho.recurso.Substring(0, espelho.recurso.Length - 3);
        var numero_do_recurso = Int32.Parse(espelho.recurso.Substring(espelho.recurso.Length - 3));
        minutos_esperados_para_a_janela = EncontrarChave(this.cfg.HORARIOS, prefixo_do_recurso, numero_do_recurso);
        if(minutos_esperados_para_a_janela == 0) continue;
        if(espelho.queue_start_start > 0 && espelho.queue_end_start < 0)
        {
        if(espelho.shift_dur < minutos_esperados_para_a_janela)
        {
          Concatenar(espelho.recurso, "jornada encurtada", espelho.shift_dur);
        }
        if(espelho.shift_dur > minutos_esperados_para_a_janela)
        {
          Concatenar(espelho.recurso, "jornada extendida", espelho.shift_dur);
        }
        }
        // DONE - Verificar se o recurso já finalizou a rota e se ainda está na janela de horário
        if(espelho.queue_end_left > 0 && this.horario_atual < janela_final)
        {
          var diff = this.horario_atual - espelho.queue_end_left;
          Concatenar(espelho.recurso, "deslogou antes", (int)(diff/this.pixels_por_minuto));
          continue;
        }
        // DONE - Verificar se o recurso já está logado ou reativado
        if(espelho.queue_start_left < 0)
        {
          var diff = this.horario_atual - espelho.shift_left;
          if((diff/this.pixels_por_minuto) > cfg.TOLERANCIA)
          {
            Concatenar(espelho.recurso, "ainda não logou", (int)(diff/this.pixels_por_minuto));
            continue;
          }
        }
        if(espelho.queue_start_left > 0)
        {
          if(this.horario_atual < espelho.shift_left)
          {
            var diff = espelho.shift_left - espelho.queue_start_left;
            Concatenar(espelho.recurso, "logou antes", (int)(diff/this.pixels_por_minuto));
          }
        }
        if(espelho.queue_end_left > 0) continue;
        // DONE - Verificar se o recurso ainda tem notas pendentes
        if(this.horario_atual < janela_final)
        {
          if(nota_atual == null)
          {
            Concatenar(espelho.recurso, "equipe sem notas");
            continue;
          }
          if(notas_pendentes.Count() == 1)
          {
            var diff = this.horario_atual - (nota_atual.style_left + nota_atual.style_width);
            Concatenar(espelho.recurso, "na última nota", (int)(diff/this.pixels_por_minuto));
          }
        }
        // DONE - Verificar se o recurso tem alguma nota `enroute` ou `started`
        if(nota_atual == null || nota_atual.data_activity_status == Servico.Status.pending)
        {
          if(nota_anterior == null)
          {
            var diff = this.horario_atual - espelho.shift_left;
            Concatenar(espelho.recurso, "equipe ociosa", (int)(diff/this.pixels_por_minuto));
            continue;
          }
          else
          {
            var diff = this.horario_atual - (nota_anterior.style_left + nota_anterior.style_width);
            if(diff/this.pixels_por_minuto >= cfg.TOLERANCIA)
            {
              Concatenar(espelho.recurso, "equipe ociosa", (int)(diff/this.pixels_por_minuto));
              continue;
            }
          }
        }

        if(nota_atual != null && nota_atual.data_activity_status == Servico.Status.started)
        {
          if(this.horario_atual > nota_atual.style_left + nota_atual.style_width)
          {
            var diff = this.horario_atual - (nota_atual.style_left + nota_atual.style_width);
            Concatenar(espelho.recurso, "atendimento atrasado", (int)(diff/this.pixels_por_minuto));
          }
        }

        if(rota_atual == null)
        {
          var diff = this.horario_atual - espelho.shift_left;
          Concatenar(espelho.recurso, "GPS desligado", (int)(diff/this.pixels_por_minuto));
          continue;
        }
        var distancia_do_inicio_registro_de_rota = this.horario_atual - rota_atual.style_left;
        var distancia_do_final_registro_de_rota = this.horario_atual - (rota_atual.style_left + rota_atual.style_width);
        var minutos_do_inicio_registro_de_rota = Convert.ToInt32(distancia_do_inicio_registro_de_rota / this.pixels_por_minuto);
        var minutos_do_final_registro_de_rota = Convert.ToInt32(distancia_do_final_registro_de_rota / this.pixels_por_minuto);
        if(minutos_do_final_registro_de_rota >= cfg.TOLERANCIA)
        {
          Concatenar(espelho.recurso, "GPS sem registro", minutos_do_final_registro_de_rota);
          continue;
        }
        if(rota_atual.status == Roteiro.Status.idle)
        {
          Concatenar(espelho.recurso, "parada indevida", minutos_do_inicio_registro_de_rota);
          continue;
        }
        if(rota_atual.status == Roteiro.Status.alert)
        {
          if(nota_atual == null)
          {
            Concatenar(espelho.recurso, "encerrou deslocando", minutos_do_inicio_registro_de_rota);
            continue;
          }
          if(nota_atual.data_activity_status == Servico.Status.started)
          {
            Concatenar(espelho.recurso, "deslocamento indevido", minutos_do_inicio_registro_de_rota);
            continue;
          }
          if(nota_atual.data_activity_status == Servico.Status.enroute)
          {
            Concatenar(espelho.recurso, "deslocamento atrasado", minutos_do_inicio_registro_de_rota);
            continue;
          }
        }
      }
    }
    public void Concatenar(String recurso, String aviso, Int32? tempo = null)
    {
      this.relatorios.Append(recurso);
      this.relatorios.Append($" {aviso}!");
      if(tempo != null)
        this.relatorios.Append($" ~{tempo}min");
      this.relatorios.Append('\n');
    }
    public Int32 EncontrarChave(Dictionary<String, Int32> dicionario, String prefixo, Int32 numero)
    {
      if(this.cfg.HORARIOS.TryGetValue(prefixo, out Int32 minutos_esperados_para_a_janela))
        return minutos_esperados_para_a_janela;
      var chaves = dicionario.Keys.ToList();
      var padrao = @"^([A-Z]{3,5})[\[]([0-9]{1,2})\.([0-9]{1,2})[\]]$";
      var regex = new Regex(padrao);
      foreach(var chave in chaves)
      {
        var match = regex.Match(chave);
        if(match != null && match.Groups[1].Value == prefixo)
        {
          if(numero >= Int32.Parse(match.Groups[2].Value))
          {
            if(numero <= Int32.Parse(match.Groups[3].Value))
            {
              return dicionario[chave];
            }
          }
        }
      }
      return 0;
    }
  }
}