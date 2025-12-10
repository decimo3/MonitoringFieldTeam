using System.Collections;
namespace MonitoringFieldTeam.Helpers;

public static class TableMaker
{
  public const char SEPARADOR = ';';
  public static string ListObjectsToCSV(IList list)
  {
    if (list == null || list.Count == 0)
      throw new ArgumentException($"A lista está vazia!");
    var type = list[0]!.GetType();
    var properties = type.GetProperties();
    var table = new System.Text.StringBuilder();
    var values = new List<string>();
    foreach (var property in properties)
    {
      values.Add(property.Name);
    }
    table.AppendLine(string.Join(SEPARADOR, values));
    values.Clear();
    foreach (var item in list)
    {
      foreach (var property in properties)
      {
        var value = property.GetValue(item);
        values.Add(value?.ToString() ?? string.Empty);
      }
      table.AppendLine(string.Join(SEPARADOR, values));
      values.Clear();
    }
    return table.ToString();
  }
}
