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
        // DONE - Verificar se o recurso já está na janela de horário
        if(espelho.shift_left > this.horario_atual) continue;
        // TODO - Verificar se o recurso já finalizou a rota
        if(espelho.queue_end_left > 0)
        {
          if(espelho.queue_end_left < janela_final)
          {
            this.relatorios.Add(espelho.recurso, "deslogou antes do horário");
          }
          continue;
        }
        // DONE - Verificar se o recurso já está logado ou reativado
        if(espelho.queue_start_left < 0)
        {
          this.relatorios.Add(espelho.recurso, "ainda não logou!");
          continue;
        }
        // DONE - Verificar se o recurso ainda tem notas pendentes
        if(espelho.servicos.Where(s => s.data_activity_status == (int)Servico.Status.pending).Count() == 0)
        {
          if(janela_final > this.horario_atual)
            this.relatorios.Add(espelho.recurso, "sem nota de serviço!");
          continue;
        }
        // TODO - Verificar se o recurso tem alguma nota `enroute` ou `started`
        if(espelho.servicos.Where(s => 
          s.data_activity_status == (int)Servico.Status.enroute || 
          s.data_activity_status == (int)Servico.Status.started).Count() == 0)
        {
          this.relatorios.Add(espelho.recurso, "sem atividade acionada!");
          continue;
        }
        var rota_atual = espelho.roteiro.OrderByDescending(r => r.start).FirstOrDefault();
        if(rota_atual == null)
        {
          this.relatorios.Add(espelho.recurso, "sem registro no GPS!");
          continue;
        }
        if(this.horario_atual - (rota_atual.style_left + rota_atual.style_width) > 10)
        {
          this.relatorios.Add(espelho.recurso, "sem registro no GPS!");
          continue;
        }
        if(rota_atual.status == Roteiro.Status.idle)
        {
          this.relatorios.Add(espelho.recurso, "parado com nota em rota!");
          continue;
        }
        if(rota_atual.status == Roteiro.Status.alert)
        {
          this.relatorios.Add(espelho.recurso, "deslocando com nota iniciada!");
          continue;
        }
      }
    }
  }
}