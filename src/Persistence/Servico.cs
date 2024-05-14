namespace Automation.Persistence
{
  public class Servico
  {
    public Int32 par_pid { get; set; }
    public Int32 aid { get; set; }
    public DateOnly par_date { get; set; }
    public Boolean ordered { get; set; }
    public Int32 start { get; set; }
    public Int32 dur { get; set; }
    public Boolean movable { get; set; }
    public Boolean multiday { get; set; }
    public Int32 data_activity_eta { get; set; }
    public Int32 data_activity_status { get; set; }
    public String data_activity_type { get; set; } = String.Empty;
    public Int32 data_activity_worktype { get; set; }
    public Int32 data_activity_duration { get; set; }
    public Int32 data_master_id { get; set; }
    public Int32 style_top { get; set; }
    public Int32 style_left { get; set; }
    public Int32 style_width { get; set; }
    public Int32 style_height { get; set; }
    public Int32 travel_dur { get; set; }
    public Int32 travel_style_width { get; set; }
    public String innerText { get; set; } = String.Empty;
    public enum Status {cancelled, pending, enroute, started, complete, notdone}
  }
}