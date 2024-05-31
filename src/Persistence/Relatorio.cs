namespace Automation.Persistence
{
  public class Relatorio
  {
    public String recurso { get; set; } = String.Empty;
    public Double login_calendario { get; set; }
    public Double logout_calendario { get; set; }
    public Double login_horario { get; set; }
    public Double logout_horario { get; set; }
    public Double login_considerado { get; set; }
    public Double logout_considerado { get; set; }
    public Double tempo_checklist { get; set; }
    public Double tempo_intervalo { get; set; }
    public Double tempo_indisponivel { get; set; }
    public Double tempo_jornada { get; set; }
    public Double tempo_deslocando { get; set; }
    public Double tempo_executando { get; set; }
    public Double tempo_rejeitando { get; set; }
    public Double estima_deslocando { get; set; }
    public Double estima_executando { get; set; }
    public Double tempo_ocupacao { get; set; }
    public Double tempo_ociosidade { get; set; }
    public Double tempo_eficiencia { get; set; }
    public Double roteiro_alerta { get; set; }
    public Double roteiro_parado { get; set; }
    public Double roteiro_andando { get; set; }
    // Produto da ocupacao pela jornada
    public Double proporcao_ocupacao { get; set; }
    // Produto do executando pela ocupacao
    public Double proporcao_eficacia { get; set; }
    // Produto da eficacia pelas estimativas
    public Double proporcao_eficiencia { get; set; }
    // ocupacao * eficacia * eficiencia
    public Double proporcao_indice { get; set; }
  }
}
