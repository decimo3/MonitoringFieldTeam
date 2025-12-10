using Serilog;
using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.Persistence;
namespace MonitoringFieldTeam.WebScraper
{
  public static class Finalizador
  {
    private static Double GetTimeOnly(Int32 tempo)
    {
      return tempo > 0 ? (Double)tempo/60/24 : 0;
    }
    private static Boolean HasInfo2FinalReport(List<Espelho> espelhos)
    {
      if (espelhos.Count == 0) return false;
      var pendentes = espelhos.Where(e => e.queue_end_left < 0).ToList();
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
    public static string CreateReport(List<Relatorio_DTO> relatorios, String bucketName, DateOnly dateLabel)
    {
      var tailpath = relatorios.Any() ? "done" : "void";
      var filename = $"{dateLabel.ToString("yyyyMMdd")}_{bucketName}.{tailpath}.csv";
      var filepath = System.IO.Path.Combine(Configuration.GetString("DATAPATH"), filename);
      var csv = relatorios.Any() ? TableMaker.ListObjectsToCSV(relatorios) : string.Empty;
      System.IO.File.WriteAllText(filepath, csv);
      Log.Information("Relatório '{filename}' exportado!\n{csv}", filename, csv);
      return filepath;
    }
    public static List<Relatorio_DTO> Finalizacao(List<Espelho> espelhos, DateOnly datalabel, Boolean check_pending = true)
    {
      var relatorios = new List<Relatorio_DTO>();
      if (check_pending || !HasInfo2FinalReport(espelhos)) return relatorios;
      foreach (var espelho in espelhos)
      {
        if(espelho.queue_start_start < 0) continue;
        var relatorio = new Relatorio();
        relatorio.recurso = espelho.recurso;
        relatorio.data_referencia = datalabel;
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
      return relatorios;
    }
    public static bool TemFinalizacao(String downfolder, DateOnly data, String piscina)
    {
      var balde = piscina.Split('>').Last();
      var filename_done = Path.Combine(downfolder, $"{data.ToString("yyyyMMdd")}_{balde}.done.csv");
      var filename_send = Path.Combine(downfolder, $"{data.ToString("yyyyMMdd")}_{balde}.send.csv");
      var filename_void = Path.Combine(downfolder, $"{data.ToString("yyyyMMdd")}_{balde}.void.csv");
      return File.Exists(filename_done) || File.Exists(filename_send) || File.Exists(filename_void);
    }
  }
}
