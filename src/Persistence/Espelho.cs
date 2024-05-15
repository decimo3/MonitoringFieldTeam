using System.Text.RegularExpressions;
namespace Automation.Persistence
{
  public class Espelho
  {
    public String recurso { get; set; }
    public Int32 par_pid { get; set; }
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
    public List<Roteiro> roteiro { get; set; } = new();
    public Espelho(String recurso, Int32 par_pid)
    {
      this.recurso = recurso.Trim();
      this.par_pid = par_pid;
    }
  }
}
