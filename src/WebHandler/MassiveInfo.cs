using Automation.Persistence;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    private static string ListObjectsToCSV<T>(List<T> list)
    {
      if (list.Count == 0)
      {
        throw new ArgumentException($"A lista está vazia!");
      }
      var type = typeof(T);
      var properties = type.GetProperties();
      var table = new System.Text.StringBuilder();
      var values = new List<string>();
      foreach (var property in properties)
      {
        values.Add(property.Name);
      }
      table.AppendLine(string.Join(";", values));
      values.Clear();
      foreach (var item in list)
      {
        foreach (var property in properties)
        {
          var value = property.GetValue(item);
          if (value == null)
          {
            values.Add("null");
          }
          else
          {
            values.Add(value.ToString());
          }
        }
        table.AppendLine(string.Join(";", values));
        values.Clear();
      }
      return table.ToString();
    }
    public void MassiveInfo()
    {
      Console.WriteLine($"{DateTime.Now} - Verificando a lista de notas...");
      var filepath = System.IO.Path.Combine(
          System.AppContext.BaseDirectory,
          "ofs.txt");
      if (!System.IO.File.Exists(filepath))
      {
        Console.WriteLine($"{DateTime.Now} - O arquivo de lista notas não existe!");
        return;
      }
      var lines = System.IO.File.ReadAllLines(filepath);
      if (!lines.All(l => Int64.TryParse(l, out var _)))
      {
        throw new ArgumentException($"Há caracteres inválidos na lista de notas!");
      }
      var materiais = new List<MaterialInfo>();
      foreach (var line in lines)
      {
        try
        {
          SearchAndEnterActivity(line);
          materiais.AddRange(GetActivityMaterials(line));

        }
        catch (Exception ex)
        {
          Console.WriteLine($"{DateTime.Now} - Ocorreu um erro ao processar a nota {line}.\n{ex.Message}");
        }
      }
      System.IO.File.WriteAllText(
          System.IO.Path.Combine(
              System.AppContext.BaseDirectory,
              "ofs.csv"),
          ListObjectsToCSV(materiais));
      System.IO.File.Delete(filepath);
      Console.WriteLine($"{DateTime.Now} - O relatório de material foi exportado!");
    }
  }
}
