namespace Automation.Persistence
{
  public class Roteiro
  {
    public Int32 par_pid { get; set; }
    public Status status { get; set; }
    public Int32 start { get; set; }
    public Int32 dur { get; set; }
    public Int32 style_left { get; set; }
    public Int32 style_width { get; set; }
    public enum Status {normal, idle, alert}
  }
}