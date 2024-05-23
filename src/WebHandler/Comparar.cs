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
        var nota_atual = espelho.servicos.Where(s => s.data_activity_status != (int)Servico.Status.pending).OrderByDescending(r => r.start).FirstOrDefault();
        var rota_atual = espelho.roteiro.OrderByDescending(r => r.start).ThenByDescending(r => r.dur).FirstOrDefault();
        // DONE - Verificar se o recurso já está na janela de horário
        if(espelho.shift_left > this.horario_atual) continue;
        // DONE - Verificar se o recurso já finalizou a rota e se ainda está na janela de horário
        if(espelho.queue_end_left > 0 && this.horario_atual < janela_final)
        {
          var diff = this.horario_atual - espelho.queue_end_left;
          this.relatorios.Add(espelho.recurso, $"deslogou antes! ~{(int)(diff/this.pixels_por_minuto)}min");
          continue;
        }
        // DONE - Verificar se o recurso já está logado ou reativado
        if(espelho.queue_start_left < 0)
        {
          var diff = this.horario_atual - espelho.shift_left;
          if((diff/this.pixels_por_minuto) > configuration.TOLERANCIA)
          {
          this.relatorios.Add(espelho.recurso, $"ainda não logou! ~{(int)(diff/this.pixels_por_minuto)}min");
          continue;
          }
        }
        // DONE - Verificar se o recurso ainda tem notas pendentes
        if(espelho.servicos.Where(s => s.data_activity_status == (int)Servico.Status.pending).Count() == 0)
        {
          if(janela_final > this.horario_atual)
          {
            if(nota_atual == null)
            {
              this.relatorios.Add(espelho.recurso, "equipe sem notas!");
              continue;
            }
            var diff = this.horario_atual - (nota_atual.style_left + nota_atual.style_width);
            this.relatorios.Add(espelho.recurso, $"equipe sem notas! ~{(int)(diff/this.pixels_por_minuto)}min");
          }
          continue;
        }
        {
        // DONE - Verificar se o recurso tem alguma nota `enroute` ou `started`
        if(espelho.servicos.Where(s => 
          s.data_activity_status == (int)Servico.Status.enroute || 
          s.data_activity_status == (int)Servico.Status.started).Count() == 0)
        {
          var diff = nota_atual == null ? 0 : this.horario_atual - (nota_atual.style_left + nota_atual.style_width);
          if(diff/this.pixels_por_minuto > configuration.TOLERANCIA)
          {
            this.relatorios.Add(espelho.recurso, $"equipe ociosa! ~{(int)(diff/this.pixels_por_minuto)}min");
            continue;
          }
        }
        // DONE
        if(rota_atual == null)
        {
          var diff = this.horario_atual - espelho.shift_left;
          this.relatorios.Add(espelho.recurso, $"GPS desligado! ~{(int)(diff/this.pixels_por_minuto)}min");
          continue;
        }
        var distancia_do_inicio_registro_de_rota = this.horario_atual - rota_atual.style_left;
        var distancia_do_final_registro_de_rota = this.horario_atual - (rota_atual.style_left + rota_atual.style_width);
        var minutos_do_inicio_registro_de_rota = Convert.ToInt32(distancia_do_inicio_registro_de_rota / this.pixels_por_minuto);
        var minutos_do_final_registro_de_rota = Convert.ToInt32(distancia_do_final_registro_de_rota / this.pixels_por_minuto);
        if(minutos_do_final_registro_de_rota >= configuration.TOLERANCIA)
        {
          this.relatorios.Add(espelho.recurso, $"GPS sem registro! ~{minutos_do_final_registro_de_rota}min");
          continue;
        }
        if(rota_atual.status == Roteiro.Status.idle)
        {
          this.relatorios.Add(espelho.recurso, $"parada indevida! ~{minutos_do_inicio_registro_de_rota}min");
          continue;
        }
        if(rota_atual.status == Roteiro.Status.alert)
        {
          if(nota_atual == null)
          {
            this.relatorios.Add(espelho.recurso, $"encerrou deslocando! ~{minutos_do_inicio_registro_de_rota}min");
            continue;
          }
          if(nota_atual.data_activity_status == (int)Servico.Status.started)
          {
            this.relatorios.Add(espelho.recurso, $"deslocamento indevido! ~{minutos_do_inicio_registro_de_rota}min");
            continue;
          }
          if(nota_atual.data_activity_status == (int)Servico.Status.enroute)
          {
            this.relatorios.Add(espelho.recurso, $"deslocamento atrasado! ~{minutos_do_inicio_registro_de_rota}min");
            continue;
          }
          continue;
        }
        }
      }
    }
  }
}