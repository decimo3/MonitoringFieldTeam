namespace automation.schemas;
public class Recurso
{
  public Int32 par_pid { get; set; }
  public Int32 tl_shift_start { get; set; }
  public Int32 tl_shift_dur { get; set; }
  public Int32 td_final_dur { get; set; }
  public Int32 tw_alert_display { get; set; }
  public Int32 tw_alert_left { get; set; }
  public Boolean tw_estimation_alert { get; set; }
  public Int32 queue_start_left { get; set; }
  public Int32 queue_start_start { get; set; }
  public Int32 queue_reactivated_left { get; set; }
  public Int32 queue_reactivated_start { get; set; }
  public Int32 queue_end_left { get; set; }
  public Int32 queue_end_start { get; set; }
  public List<Atividade> atividades { get; set; } = new();
  public List<Marcador> rastreador { get; set; } = new();
}