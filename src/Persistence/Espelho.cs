using System.Text.RegularExpressions;
namespace MonitoringFieldTeam.Persistence
{
  public class Espelho
  {
    public String recurso { get; set; }
    public Int32 par_pid { get; set; }
    public Int32 style_top { get; set; }
    public Int32 shift_start { get; set; }
    public Int32 shift_dur { get; set; }
    public Int32 shift_left { get; set; }
    public Int32 shift_width { get; set; }
    public Int32 final_dur { get; set; }
    public Boolean tw_alert_display { get; set; }
    public Int32 tw_alert_left { get; set; }
    public Int32 queue_start_left { get; set; }
    public Int32 queue_start_start { get; set; }
    public Int32 queue_reactivated_left { get; set; }
    public Int32 queue_reactivated_start { get; set; }
    public Int32 queue_end_left { get; set; }
    public Int32 queue_end_start { get; set; }
    public List<Servico> servicos { get; set; } = new();
    public List<Roteiro> roteiros { get; set; } = new();
    public Espelho(String recurso, Int32 par_pid, Int32 style_top)
    {
      // Remove spaces before and after text;
      var abreviado = recurso.Trim();
      // Change long-dash for simple dash;
      abreviado = abreviado.Replace('–', '-');
      // Change non-breaking for regular space
      abreviado = abreviado.Replace('\u00A0', '\u0020');
      // Replace double spaces by singles
      abreviado = abreviado.Replace("  ", " ");
      // Abbreviates words that determine the type of activity
      abreviado = abreviado.Replace(" - Corte", "C");
      abreviado = abreviado.Replace(" - Religa", "R");
      abreviado = abreviado.Replace(" - Vistoriador ", "V");
      abreviado = abreviado.Replace(" - Indica", "");
      // Remove the word 'team' to shorten the resource name
      abreviado = abreviado.Replace(" - Equipe ", "");
      // remove any excess word before space character
      var indice = abreviado.IndexOf(' ');
      if(indice > 0)
      {
        abreviado = abreviado[..indice];
      }
      this.recurso = abreviado;
      this.par_pid = par_pid;
      this.style_top = style_top;
    }
  }
}
