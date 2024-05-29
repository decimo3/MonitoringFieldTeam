namespace Automation.Persistence
{
  public class Relatorio
  {
    public String recurso { get; set; } = String.Empty;
    public TimeOnly login_calendario { get; set; }
    public TimeOnly logout_calendario { get; set; }
    public TimeOnly login_horario { get; set; }
    public TimeOnly logout_horario { get; set; }
    public Int32 tempo_jornada { get; set; }
    public Int32 tempo_deslocando { get; set; }
    public Int32 tempo_executando { get; set; }
    public Int32 tempo_rejeitando { get; set; }
    public Int32 estima_deslocando { get; set; }
    public Int32 estima_executando { get; set; }
    public Int32 tempo_ocupacao { get; set; }
    // Produto da ocupacao pela jornada
    public Double proporcao_ocupacao { get; set; }
    // Produto do executando pela ocupacao
    public Double proporcao_eficacia { get; set; }
    // Produto da eficacia pelas estimativas
    public Double proporcao_eficiencia { get; set; }
    // ocupacao * eficacia * eficiencia
    public Double proporcao_indice { get; set; }
    public List<String> infracoes { get; set; } = new();
  }
}
