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
            this.relatorios.Add(espelho.recurso, "Equipe deslogou antes do horário");
          }
          continue;
        }
        // DONE - Verificar se o recurso já está logado ou reativado
        if(espelho.queue_start_left < 0)
        {
          this.relatorios.Add(espelho.recurso, "Equipe ainda não logou!");
          continue;
        }
        // DONE - Verificar se o recurso ainda tem notas pendentes
        if(espelho.servicos.Where(s => s.data_activity_status == (int)Servico.Status.pending).Count() == 0)
        {
          if(janela_final > this.horario_atual)
            this.relatorios.Add(espelho.recurso, "Equipe sem serviço!");
          continue;
        }
        // TODO - Verificar se o recurso tem alguma nota `enroute` ou `started`
        if(espelho.servicos.Where(s => 
          s.data_activity_status == (int)Servico.Status.enroute || 
          s.data_activity_status == (int)Servico.Status.started).Count() == 0)
        {
          this.relatorios.Add(espelho.recurso, "Equipe está ociosa!");
          continue;
        }
      }
    }
  }
}