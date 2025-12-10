using System.Text;
using System.Collections;
using Serilog;
using MonitoringFieldTeam.Persistence;
using MonitoringFieldTeam.Helpers;

namespace MonitoringFieldTeam.WebScraper
{
  public static class MassiveInfo
  {
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
        ["INF"] = new List<GeneralInfo>(),
        ["COD"] = new List<FinalizaInfo>(),
        ["MAT"] = new List<MaterialInfo>(),
        // ["APR"] = new List<AnaliseInfo>(),
        // ["JPG"] = new List<FotografiaInfo>(),
        ["TOI"] = new List<OcorrenciaInfo>(),
        // ["EVD"] = new List<EvidenciaInfo>()
      };
      foreach (var line in lines)
      {
        try
        {
          SearchAndEnterActivity(line);
          if (cfg.EXTRACAO_KEY.Contains("INF"))
            informacoes["INF"].Add(GetActivityGeneralInfo(line));
          if (cfg.EXTRACAO_KEY.Contains("COD"))
            informacoes["COD"].AddRange(GetActivityClosings(line));
          if (cfg.EXTRACAO_KEY.Contains("MAT"))
            informacoes["MAT"].AddRange(GetActivityMaterials(line));
          if (cfg.EXTRACAO_KEY.Contains("TOI"))
            informacoes["TOI"].Add(GetActivityOcorrencias(line));
        }
        catch (Exception ex)
        {
          Console.WriteLine($"{DateTime.Now} - Ocorreu um erro ao processar a nota {line}.\n{ex.Message}");
        }
      }
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      foreach (var key in informacoes.Keys)
      {
        var lista = informacoes[key];
        if (lista == null || lista.Count == 0)
          continue;
        var csv_name = key.ToLower() + ".csv";
        var csv_path = System.IO.Path.Combine(cfg.DOWNFOLDER, csv_name);
        System.IO.File.WriteAllText(csv_path, ListObjectsToCSV(lista), Encoding.GetEncoding(1252));
        // Abre com o programa padrão do sistema (ex: Bloco de Notas para .txt)
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
          FileName = csv_path,
          UseShellExecute = true // importante para usar o programa padrão
        });
      }
      System.IO.File.Delete(filepath);
      Console.WriteLine($"{DateTime.Now} - Os relatórios massivos foram exportados!");
    }
  }
}
