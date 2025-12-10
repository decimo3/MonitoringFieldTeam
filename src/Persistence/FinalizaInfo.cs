namespace MonitoringFieldTeam.Persistence
{
  public class FinalizaInfo
  {
    public string? NotaServico { get; set; }
    public string? Codigo { get; set; }
    public string? Quantidade { get; set; }
    public override string ToString()
    {
      var type = this.GetType();
      var properties = type.GetProperties();
      var result = new System.Text.StringBuilder();
      foreach (var property in properties)
      {
        var value = property.GetValue(this);
        if (value is null) continue;
        result.Append(property.Name);
        result.Append(": ");
        result.Append(value);
        result.Append('\n');
      }
      return result.ToString();
    }
  }
}
