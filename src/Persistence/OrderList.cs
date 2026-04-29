namespace MonitoringFieldTeam.Persistence;

public class OrderInfo
{
  public long Identifier { get; set; } = 0;
  public long ActivityId { get; set; } = 0;
  public long OrderNumber { get; set; } = 0;
  public int StatusCode { get; set; } = 0;
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
  public string? Observation { get; set; }
}
