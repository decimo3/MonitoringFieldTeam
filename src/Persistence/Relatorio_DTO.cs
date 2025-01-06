namespace Automation.Persistence
{
  public class Relatorio_DTO
  {
    public DateOnly data_referencia { get; set; }
    public String recurso { get; set; } = String.Empty;
    public TimeOnly login_horario { get; set; }
    public TimeOnly logout_horario { get; set; }
    public TimeOnly checklist_tempo { get; set; }
    public TimeOnly intervalo_tempo { get; set; }
    public TimeOnly tempo_indisponivel { get; set; }
    public TimeOnly jornada_tempo { get; set; }
    public TimeOnly tempo_executando { get; set; }
    public TimeOnly tempo_deslocando { get; set; }
    public TimeOnly tempo_rejeitando { get; set; }
    public TimeOnly estima_deslocando { get; set; }
    public TimeOnly estima_executando { get; set; }
    public TimeOnly tempo_ocupacao { get; set; }
    public TimeOnly tempo_ociosidade { get; set; }
    public TimeOnly tempo_produtivo { get; set; }
    public TimeOnly tempo_eficiencia { get; set; }
    public TimeOnly roteiro_alerta { get; set; }
    public TimeOnly roteiro_parado { get; set; }
    public TimeOnly roteiro_andando { get; set; }
    public TimeOnly roteiro_desligado { get; set; }
    public Double proporcao_ocupacao { get; set; }
    public Double proporcao_produtivo { get; set; }
    public Double proporcao_eficiencia { get; set; }
    public Double proporcao_indice { get; set; }
    public Relatorio_DTO(Relatorio relatorio)
    {
      this.data_referencia = relatorio.data_referencia;
      this.recurso = relatorio.recurso;
      this.login_horario = Double2TimeOnly(relatorio.login_horario);
      this.logout_horario = Double2TimeOnly(relatorio.logout_horario);
      this.checklist_tempo = Double2TimeOnly(relatorio.checklist_tempo);
      this.intervalo_tempo = Double2TimeOnly(relatorio.intervalo_tempo);
      this.tempo_indisponivel = Double2TimeOnly(relatorio.tempo_indisponivel);
      this.jornada_tempo = Double2TimeOnly(relatorio.jornada_tempo);
      this.tempo_deslocando = Double2TimeOnly(relatorio.tempo_deslocando);
      this.tempo_executando = Double2TimeOnly(relatorio.tempo_executando);
      this.tempo_rejeitando = Double2TimeOnly(relatorio.tempo_rejeitando);
      this.estima_deslocando = Double2TimeOnly(relatorio.estima_deslocando);
      this.estima_executando = Double2TimeOnly(relatorio.estima_executando);
      this.tempo_ocupacao = Double2TimeOnly(relatorio.tempo_ocupacao);
      this.tempo_ociosidade = Double2TimeOnly(relatorio.tempo_ociosidade);
      this.tempo_produtivo = Double2TimeOnly(relatorio.tempo_produtivo);
      this.roteiro_alerta = Double2TimeOnly(relatorio.roteiro_alerta);
      this.roteiro_parado = Double2TimeOnly(relatorio.roteiro_parado);
      this.roteiro_andando = Double2TimeOnly(relatorio.roteiro_andando);
      this.roteiro_desligado = Double2TimeOnly(relatorio.roteiro_desligado);
      this.proporcao_ocupacao = relatorio.proporcao_ocupacao;
      this.proporcao_produtivo = relatorio.proporcao_produtivo;
      this.proporcao_eficiencia = relatorio.proporcao_eficiencia;
      this.proporcao_indice = relatorio.proporcao_indice;
    }
    public TimeOnly Double2TimeOnly(Double tempo)
    {
      if(tempo < 0) return new TimeOnly();
      var tempo_em_minutos = tempo * 24 * 60;
      var dias_de_diferenca = (int)Math.Floor(tempo_em_minutos / 1440);
      if(dias_de_diferenca > 0) tempo_em_minutos = tempo_em_minutos - (dias_de_diferenca * 1440);
      return TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(tempo_em_minutos));
    }
  }
}
