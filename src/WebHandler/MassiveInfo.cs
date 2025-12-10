using System.Text;
using System.Collections;
using Serilog;
using MonitoringFieldTeam.Persistence;
using MonitoringFieldTeam.Helpers;

namespace MonitoringFieldTeam.WebScraper
{
  public static class MassiveInfo
  {
    public static void GetMassiveInfo(WebHandler.WebHandler handler)
    {
      Parametrizador.VerificarPagina(handler);
      Log.Information("Verificando a lista de notas...");
      var filepath = System.IO.Path.Combine(
        Configuration.GetString("DATAPATH"),
          "ofs.txt");
      if (!System.IO.File.Exists(filepath))
      {
        Log.Information("O arquivo de lista notas não existe!");
        return;
      }
      var lines = System.IO.File.ReadAllLines(filepath);
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
      var extracao = Configuration.GetArray("EXTRACAO");
      foreach (var line in lines)
      {
        if (!long.TryParse(line, out long service))
        {
          throw new InvalidOperationException($"Há caracteres inválidos na nota {line}!");
        }
        var workHandler = new ServicoHandler(handler, service);
        try
        {
          workHandler.SearchAndEnterActivity();
          if (extracao.Contains("INF"))
            informacoes["INF"].Add(workHandler.GetActivityGeneralInfo());
          if (extracao.Contains("COD"))
            informacoes["COD"].AddRange(workHandler.GetActivityClosings());
          if (extracao.Contains("MAT"))
            informacoes["MAT"].AddRange(workHandler.GetActivityMaterials());
          //if (extracao.Contains("APR"))
          //  informacoes["APR"].Add(workHandler.GetActivityAnaliseInfo());
          //if (extracao.Contains("JPG"))
          //  informacoes["JPG"].Add(workHandler.GetActivityFotografiaFiles());
          //if (extracao.Contains("EVD"))
          //  informacoes["EVD"].Add(workHandler.GetActivityEvidenciaInfo());
          if (extracao.Contains("TOI"))
            informacoes["TOI"].Add(workHandler.GetActivityOcorrencias());
        }
        catch (Exception ex)
        {
          Log.Information("Ocorreu um erro ao processar a nota {line}.\n{ex.Message}", line, ex.Message);
        }
      }
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      foreach (var key in informacoes.Keys)
      {
        var lista = informacoes[key];
        if (lista == null || lista.Count == 0)
          continue;
        var csv_path = System.IO.Path.Combine(
          Configuration.GetString("DATAPATH"), key.ToLower() + ".csv");
        System.IO.File.WriteAllText(csv_path,
          TableMaker.ListObjectsToCSV(lista), Encoding.GetEncoding(1252));
        // Abre com o programa padrão do sistema (ex: Bloco de Notas para .txt)
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
          FileName = csv_path,
          UseShellExecute = true // importante para usar o programa padrão
        });
      }
      System.IO.File.Delete(filepath);
      Log.Information("Os relatórios massivos foram exportados!");
    }
  }
}
