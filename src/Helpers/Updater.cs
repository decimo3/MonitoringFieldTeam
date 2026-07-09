using System.Net.Http.Json;
using System.Text.Json;
using Serilog;
namespace MonitoringFieldTeam.Helpers;

public static class Updater
{
  private const String DRIVER_ZIPFILE = "chromedriver-win64.zip";
  private const String DRIVER_MILESTONES = "https://googlechromelabs.github.io/chrome-for-testing/latest-versions-per-milestone.json";
  private const String DRIVER_DOWNLOAD = "https://storage.googleapis.com/chrome-for-testing-public/{0}/win64/chromedriver-win64.zip";

  private static Int32 GetVersionAplicationOutput(String aplication, String arguments)
  {
      var regex = new System.Text.RegularExpressions.Regex(@"\d+");
      var result = Helpers.Executor.Executar(aplication, arguments);
      var match = regex.Match(result);
      if(!match.Success)
        throw new InvalidOperationException($"Não foi encontrada a versão da aplicação `{aplication}` nas propriedades do arquivo!");
      return Int32.Parse(match.Value);
  }

  private static String CheckNewerChromeDriverVersion(String version)
  {
    using(var client = new HttpClient())
    {
      var request = new System.Net.Http.HttpRequestMessage(HttpMethod.Get, DRIVER_MILESTONES);
      var response = client.Send(request);
      response.EnsureSuccessStatusCode();
      using var document = response.Content.ReadFromJsonAsync<JsonDocument>().GetAwaiter().GetResult() ??
          throw new InvalidOperationException("A responsta da requisição para `DRIVER_MILESTONES` recebida é nula");
      return document.RootElement.GetProperty("milestones").GetProperty(version).GetProperty("version").GetString()!;
    }
  }

  private static void DownloadNewerChromeDriver(String driver_version)
  {
    using (var client = new HttpClient())
    {
      var last_version_url = String.Format(DRIVER_DOWNLOAD, driver_version);
      var request = new System.Net.Http.HttpRequestMessage(HttpMethod.Get, last_version_url);
      var response = client.Send(request);
      response.EnsureSuccessStatusCode();
      using(var stream = response.Content.ReadAsStream())
      {
        using(var file = System.IO.File.Create(
          System.IO.Path.Combine(
            System.AppContext.BaseDirectory,
            DRIVER_ZIPFILE)))
        {
          stream.CopyTo(file);
          file.Flush();
        }
      }
    }
  }

  private static void DeleteOlderDriverFile()
  {
    var drivefolderpath = System.IO.Path.Combine(
      System.AppContext.BaseDirectory, "chromedriver-win64");
    if (!Directory.Exists(drivefolderpath)) return;
    var files = System.IO.Directory.GetFiles(drivefolderpath);
    foreach (var file in files) System.IO.File.Delete(file);
  }

  private static void UnzipChromeDriverFile()
  {
    var zipfilepath = System.IO.Path.Combine(
      System.AppContext.BaseDirectory, DRIVER_ZIPFILE);
    System.IO.Compression.ZipFile.ExtractToDirectory(
      zipfilepath, System.AppContext.BaseDirectory);
  }

  public static void Update()
  {
    try
    {
      var driverpath = System.IO.Path.Combine(
        System.AppContext.BaseDirectory, "chromedriver-win64", "chromedriver.exe");
      Log.Information("Verificando as versões do browser e do driver...");
      var argumento = $"-c \"(Get-Item '{Configuration.GetString("GCHROME")}').VersionInfo.ProductVersion.ToString()\"";
      var chrome_version = GetVersionAplicationOutput("powershell", argumento);
      Log.Information("Chrome major version: {chrome_version}.", chrome_version);
      if (System.IO.File.Exists(driverpath))
      {
        var driver_version = GetVersionAplicationOutput(driverpath, "--version");
        Log.Information("Driver major version: {driver_version}.", driver_version);
        if (driver_version == chrome_version) return;
      }
      Log.Information("Buscando as novas versões do chromedriver...");
      var newer_version = CheckNewerChromeDriverVersion(chrome_version.ToString());
      Log.Information("Versão do chromedriver: {newer_version}", newer_version);
      Log.Information("Baixando a nova versão do chromedriver...");
      DownloadNewerChromeDriver(newer_version);
      Log.Information("Download concluído da nova versão do chromedriver!");
      if (System.IO.File.Exists(driverpath))
      {
        Log.Information("Removendo a versão antiga do chromedriver...");
        DeleteOlderDriverFile();
      }
      Log.Information("Remoção concluída da versão antiga do chromedriver");
      Log.Information("Descompactando atualização...");
      UnzipChromeDriverFile();
      Log.Information("Atualização concluída com sucesso!");
    }
    catch (System.Exception erro)
    {
      Log.Error(erro.Message);
      if (erro.StackTrace is not null)
        Log.Error(erro.StackTrace);
    }
  }
}
