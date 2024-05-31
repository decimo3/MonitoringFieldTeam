using Automation.Persistence;
using OpenQA.Selenium.DevTools.V119.Network;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    public Double GetTimeOnly(Int32 tempo)
    {
      return (Double)tempo/60/24;
    }
    public void Finalizacao()
    {
      if(!this.espelhos.Any()) return;
      if(!this.espelhos.All(e => e.queue_end_left > 0)) return;
      var relatorios = new List<Relatorio>();
      foreach (var espelho in espelhos)
      {
        var relatorio = new Relatorio();
        relatorio.recurso = espelho.recurso;
        relatorio.login_calendario = GetTimeOnly(espelho.shift_start);
        relatorio.login_horario = GetTimeOnly(espelho.queue_start_start);
        relatorio.logout_calendario = GetTimeOnly(espelho.shift_start + espelho.shift_dur);
        relatorio.logout_horario = GetTimeOnly(espelho.queue_end_start);
        relatorio.login_considerado = relatorio.login_horario < relatorio.login_calendario ? relatorio.login_horario : relatorio.login_calendario;
        relatorio.logout_considerado = relatorio.logout_horario > relatorio.logout_calendario ? relatorio.logout_horario : relatorio.logout_calendario;
        relatorio.tempo_jornada = relatorio.logout_considerado - relatorio.login_considerado;
        foreach (var roteiro in espelho.roteiros)
        {
          switch (roteiro.status)
          {
            case Roteiro.Status.normal:
              relatorio.roteiro_andando += GetTimeOnly(roteiro.dur);
            break;
            case Roteiro.Status.idle:
              relatorio.roteiro_parado += GetTimeOnly(roteiro.dur);
            break;
            case Roteiro.Status.alert:
              relatorio.roteiro_alerta += GetTimeOnly(roteiro.dur);
            break;
          }
        }
        foreach (var servico in espelho.servicos)
        {
          servico.innerText = servico.innerText.Trim();
          if(servico.innerText == "Início de turno")
          {
            relatorio.tempo_checklist += GetTimeOnly(servico.data_activity_duration);
            continue;
          }
          if(servico.innerText == "Intervalo para almoço")
          {
            relatorio.tempo_intervalo += GetTimeOnly(servico.data_activity_duration);
            continue;
          }
          if(servico.innerText == "INDISPONIBILIDADE")
          {
            relatorio.tempo_indisponivel += GetTimeOnly(servico.data_activity_duration);
            continue;
          }
          if(servico.data_activity_status == Servico.Status.complete)
          {
            relatorio.tempo_executando += GetTimeOnly(servico.data_activity_duration);
            relatorio.tempo_deslocando += GetTimeOnly(servico.travel_dur);
            continue;
          }
          if(servico.data_activity_status == Servico.Status.notdone)
          {
            relatorio.tempo_rejeitando += GetTimeOnly(servico.data_activity_duration);
            relatorio.tempo_rejeitando += GetTimeOnly(servico.travel_dur);
            continue;
          }
        }
        relatorio.tempo_eficiencia = relatorio.tempo_executando + relatorio.tempo_deslocando;
        relatorio.tempo_ocupacao = relatorio.tempo_eficiencia + relatorio.tempo_rejeitando;
        relatorio.tempo_ociosidade = relatorio.tempo_jornada - relatorio.tempo_ocupacao;
        relatorio.proporcao_ocupacao = relatorio.tempo_ocupacao / relatorio.tempo_jornada;
        relatorio.proporcao_eficiencia = relatorio.tempo_eficiencia / relatorio.proporcao_ocupacao;
        relatorios.Add(relatorio);
      }
      var balde_nome = this.cfg.PISCINAS[this.contador_de_baldes].Split('>').Last();
      var filename = $"{this.cfg.DOWNFOLDER}/{this.agora.ToString("yyyyMMdd")}_{balde_nome}.csv";
      var csv = TableMaker<Relatorio>.Serialize(relatorios);
      System.IO.File.WriteAllText(filename, csv);
      System.Console.WriteLine($"{DateTime.Now} - Arquivo {filename} exportado!");
    }
  }
}
