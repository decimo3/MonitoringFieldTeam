namespace MonitoringFieldTeam.Persistence;

public class RequestInfo
{
  public string[] info { get; set; }
  public long nota { get; set; }
  public long aid { get; set; }
  public RequestInfo(string[] info, long nota, long aid = 0)
  {
    this.info = info;
    this.nota = nota;
    this.aid = aid;
  }
}
