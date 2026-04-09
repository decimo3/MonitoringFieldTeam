namespace MonitoringFieldTeam.Persistence;

public class RequestInfo
{
  public string[] info { get; set; }
  public long nota { get; set; }
  public RequestInfo(string[] info, long nota)
  {
    this.info = info;
    this.nota = nota;
  }
}
