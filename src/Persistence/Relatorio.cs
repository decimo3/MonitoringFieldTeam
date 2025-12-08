namespace MonitoringFieldTeam.Persistence
{
  public class Relatorio
  {
    public DateOnly data_referencia { get; set; }
    public String recurso { get; set; } = String.Empty;
    public Double login_calendario { get; set; }
    public Double logout_calendario { get; set; }
    public Double login_horario { get; set; }
    public Double logout_horario { get; set; }
    public Double login_considerado { get; set; }
    public Double logout_considerado { get; set; }
    public Double checklist_tempo { get; set; }
    public Double checklist_considerado { get; set; }
    public Double intervalo_tempo { get; set; }
    public Double intervalo_considerado { get; set; }
    public Double tempo_indisponivel { get; set; }
    public Double jornada_tempo { get; set; }
    public Double jornada_considerado { get; set; }
    public Double tempo_deslocando { get; set; }
    public Double tempo_executando { get; set; }
    public Double tempo_rejeitando { get; set; }
    public Double estima_deslocando { get; set; }
    public Double estima_executando { get; set; }
    public Double tempo_ocupacao { get; set; }
    public Double tempo_ociosidade { get; set; }
    public Double tempo_produtivo { get; set; }
    public Double roteiro_alerta { get; set; }
    public Double roteiro_parado { get; set; }
    public Double roteiro_andando { get; set; }
    public Double roteiro_desligado { get; set; }
    // Produto da ocupacao pela jornada
    public Double proporcao_ocupacao { get; set; }
    // Produto do executando pela ocupacao
    public Double proporcao_produtivo { get; set; }
    // Produto da produtivo pelas estimativas
    public Double proporcao_eficiencia { get; set; }
    // ocupacao * produtivo * eficiencia
    public Double proporcao_indice { get; set; }
  }
}
