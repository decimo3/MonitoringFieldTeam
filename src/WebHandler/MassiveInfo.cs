using System.Collections;
using Automation.Persistence;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    private static string ListObjectsToCSV(IList list)
    {
      if (list == null)
        throw new ArgumentException($"A lista está vazia!");
      if (list.Count == 0)
        throw new ArgumentException($"A lista está vazia!");
      var type = list[0]!.GetType();
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
          values.Add(value?.ToString() ?? string.Empty);
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
      var informacoes = new Dictionary<string, IList>
      {
        // ["INF"] = new List<GeneralInfo>(),
        // ["COD"] = new List<FinalizaInfo>(),
        ["MAT"] = new List<MaterialInfo>(),
        // ["APR"] = new List<AnaliseInfo>(),
        // ["JPG"] = new List<FotografiaInfo>(),
        // ["TOI"] = new List<OcorrenciaInfo>(),
        // ["EVD"] = new List<EvidenciaInfo>()
      };
      foreach (var line in lines)
      {
        try
        {
          SearchAndEnterActivity(line);
          if (cfg.EXTRACAO_KEY.Contains("MAT"))
            informacoes["MAT"].Add(GetActivityMaterials(line));
        }
        catch (Exception ex)
        {
          Console.WriteLine($"{DateTime.Now} - Ocorreu um erro ao processar a nota {line}.\n{ex.Message}");
        }
      }
      foreach (var key in informacoes.Keys)
      {
        var lista = informacoes[key];
        if (lista == null || lista.Count == 0)
          continue;
        Type tipo = lista[0]!.GetType();
        System.IO.File.WriteAllText(
            System.IO.Path.Combine(
                cfg.DOWNFOLDER,
                key.ToLower() + ".csv"),
            ListObjectsToCSV(lista));
      }
      System.IO.File.Delete(filepath);
      Console.WriteLine($"{DateTime.Now} - Os relatórios massivos foram exportados!");
      // Abre com o programa padrão do sistema (ex: Bloco de Notas para .txt)
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
      {
        FileName = System.IO.Path.Combine(
            System.AppContext.BaseDirectory,
            "ofs.csv"),
        UseShellExecute = true // importante para usar o programa padrão
      });
    }
  }
}
