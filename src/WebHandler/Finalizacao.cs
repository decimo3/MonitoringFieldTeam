using Automation.Persistence;
using OpenQA.Selenium.DevTools.V119.Network;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    public Double GetTimeOnly(Int32 tempo)
    {
      return tempo > 0 ? (Double)tempo/60/24 : 0;
    }
    private Boolean HasInfo2FinalReport()
    {
      if(this.espelhos.Count == 0) return false;
      var pendentes = this.espelhos.Where(e => e.queue_end_left < 0).ToList();
      foreach (var pendente in pendentes)
      {
        if(pendente.servicos.Where(s => s.data_activity_status == Servico.Status.pending).Count() > 0) return false;
        var ultimo_servico = pendente.servicos.OrderBy(s => s.start).FirstOrDefault();
        if(ultimo_servico == null) continue;
        var hora_encerramento_ultima_nota = TimeSpan.FromMinutes(ultimo_servico.start + ultimo_servico.dur);
        var hora_limite_para_encerramento = hora_encerramento_ultima_nota.Add(new TimeSpan(hours: 2, minutes: 0, seconds: 0));
        if(hora_encerramento_ultima_nota > hora_limite_para_encerramento) return false;
        pendente.queue_end_start = (pendente.queue_start_start < pendente.shift_start) ? pendente.queue_start_start + 1440 : pendente.shift_start + 1440;
      }
      return true;
    }
    private void CreateVoidReport()
    {
      var filename = $"{this.datalabel.ToString("yyyyMMdd")}_{this.balde_nome}.void.csv";
      var filepath = System.IO.Path.Combine(this.cfg.DOWNFOLDER, filename);
      System.IO.File.Create(filepath).Close();
      System.Console.WriteLine($"{DateTime.Now} - Relatório vazio {filename} exportado!");
    }
    public void Finalizacao(Boolean check_pending = true)
    {
      if(check_pending)
      {
        if(!HasInfo2FinalReport()) return;
      }
      else
      {
        ExportarPropriedadesEspelhos();
      }
      var relatorios = new List<Relatorio_DTO>();
      foreach (var espelho in espelhos)
      {
        if(espelho.queue_start_start < 0) continue;
        var relatorio = new Relatorio();
        relatorio.recurso = espelho.recurso;
        relatorio.data_referencia = this.datalabel;
        relatorio.login_calendario = GetTimeOnly(espelho.shift_start);
        relatorio.login_horario = GetTimeOnly(espelho.queue_start_start);
        relatorio.logout_calendario = GetTimeOnly(espelho.shift_start + espelho.shift_dur);
        relatorio.logout_horario = GetTimeOnly(espelho.queue_end_start);
        relatorio.login_considerado = relatorio.login_horario < relatorio.login_calendario ? relatorio.login_horario : relatorio.login_calendario;
        relatorio.logout_considerado = relatorio.logout_horario > relatorio.logout_calendario ? relatorio.logout_horario : relatorio.logout_calendario;
        relatorio.jornada_tempo = relatorio.logout_considerado - relatorio.login_considerado;
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
        var roteiro_registrando = relatorio.roteiro_andando + relatorio.roteiro_parado + relatorio.roteiro_alerta;
        if(roteiro_registrando > relatorio.jornada_tempo) relatorio.roteiro_desligado = 0;
        else relatorio.roteiro_desligado = relatorio.jornada_tempo - roteiro_registrando;
        foreach (var servico in espelho.servicos)
        {
          servico.innerText = servico.innerText.Trim();
          if(servico.innerText == "Início de turno")
          {
            var checklist_tolerancia = GetTimeOnly(30);
            relatorio.checklist_tempo += GetTimeOnly(servico.data_activity_duration);
            relatorio.checklist_considerado = relatorio.checklist_tempo > checklist_tolerancia ? checklist_tolerancia : relatorio.checklist_tempo;
            continue;
          }
          if(servico.innerText == "Intervalo para almoço")
          {
            var intervalo_tolerancia = GetTimeOnly(60);
            relatorio.intervalo_tempo += GetTimeOnly(servico.data_activity_duration);
            relatorio.intervalo_considerado = relatorio.intervalo_tempo > intervalo_tolerancia ? intervalo_tolerancia : relatorio.intervalo_tempo;
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
        relatorio.jornada_considerado = relatorio.jornada_tempo - (relatorio.intervalo_considerado + relatorio.checklist_considerado);
        relatorio.tempo_produtivo = relatorio.tempo_executando + relatorio.tempo_deslocando;
        relatorio.tempo_ocupacao = relatorio.tempo_produtivo + relatorio.tempo_rejeitando;
        relatorio.tempo_ociosidade = relatorio.jornada_considerado - relatorio.tempo_ocupacao;
        relatorio.proporcao_ocupacao = relatorio.tempo_ocupacao / relatorio.jornada_considerado;
        relatorio.proporcao_produtivo = relatorio.tempo_produtivo / relatorio.tempo_ocupacao;
        relatorio.proporcao_eficiencia = 1; // TODO - Coletar o valor correto de acordo com o relatório do IDG
        relatorio.proporcao_indice = relatorio.proporcao_ocupacao * relatorio.proporcao_produtivo * relatorio.proporcao_eficiencia;
        var relatorio_dto = new Relatorio_DTO(relatorio);
        relatorios.Add(relatorio_dto);
      }
      if(!relatorios.Any())
      {
        ExportarPropriedadesEspelhos();
        CreateVoidReport();
        return;
      }
      var filename = $"{this.datalabel.ToString("yyyyMMdd")}_{this.balde_nome}.done.csv";
      var filepath = System.IO.Path.Combine(this.cfg.DOWNFOLDER, filename);
      var csv = Helpers.TableMaker<Relatorio_DTO>.Serialize(relatorios);
      System.IO.File.WriteAllText(filepath, csv);
      foreach (var line in csv.Split('\n')) System.Console.WriteLine($"{DateTime.Now} - {line}");
      if(!cfg.BOT_CHANNELS.TryGetValue(this.balde_nome, out long channel)) return;
      using(var arquivo = new FileStream(filepath, FileMode.Open))
      {
        Helpers.Telegram.sendDocument(channel, arquivo, filename);
      }
      System.Console.WriteLine($"{DateTime.Now} - Arquivo {filename} exportado e transmitido!");
    }
  }
}
