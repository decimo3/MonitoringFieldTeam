using OpenQA.Selenium.Chrome;
namespace Automation;
public class Configuration
{
  public readonly String DATAFOLDER;
  public readonly String DOWNFOLDER;
  public readonly String TEMPFOLDER;
  public readonly String LOCKFILE = "ofs.lock";
  public readonly Int32 TOLERANCIA = 3;
  public readonly List<String> PISCINAS;
  public readonly Boolean ENVIRONMENT = false;
  public readonly Dictionary<String, String> CONFIGURACAO = new();
  public readonly Dictionary<String, String> CAMINHOS = new();
  public readonly Dictionary<String, Int32> HORARIOS = new();
  public readonly Dictionary<String, Int32> ESPERAS = new()
  {
    {"TOTAL", 60_000},
    {"LONGA", 10_000},
    {"MEDIA", 6_000},
    {"CURTA", 3_000},
  };
  public Configuration()
  {
    this.ENVIRONMENT = System.Environment.GetCommandLineArgs().Contains("debug");

    this.DATAFOLDER = $"{System.IO.Directory.GetCurrentDirectory()}\\www";
    if(!System.IO.Directory.Exists(this.DATAFOLDER)) System.IO.Directory.CreateDirectory(this.DATAFOLDER);
    this.DOWNFOLDER = $"{System.IO.Directory.GetCurrentDirectory()}\\odl";
    if(!System.IO.Directory.Exists(this.DOWNFOLDER)) System.IO.Directory.CreateDirectory(this.DOWNFOLDER);
    this.TEMPFOLDER = $"{System.IO.Directory.GetCurrentDirectory()}\\tmp";
    if(!System.IO.Directory.Exists(this.TEMPFOLDER)) System.IO.Directory.CreateDirectory(this.TEMPFOLDER);

    if(System.Environment.GetCommandLineArgs().Contains("slower"))
      foreach(var key in this.ESPERAS.Keys.ToList()) this.ESPERAS[key] *= 2;
    if(System.Environment.GetCommandLineArgs().Contains("faster"))
      foreach(var key in this.ESPERAS.Keys.ToList()) this.ESPERAS[key] /= 2;
    
    this.CONFIGURACAO = ArquivoConfiguracao("ofs.conf");
    this.PISCINAS = this.CONFIGURACAO["RECURSO"].Split(",").ToList();
    this.CAMINHOS = ArquivoConfiguracao("ofs.path");
    foreach(var horario in this.CONFIGURACAO["HORARIO"].Split(",").ToList())
    {
      var horario_string = horario.Split('|');
      if(horario_string.Length != 2) continue;
      this.HORARIOS.Add(horario_string.First(), Int32.Parse(horario_string.Last()));
    }
  }
  private Dictionary<String,String> ArquivoConfiguracao(String filename, char delimiter = '=')
  {
    var parametros = new Dictionary<string,string>();
    var file = System.IO.File.ReadAllLines(filename);
    foreach (var line in file)
    {
      if(String.IsNullOrEmpty(line)) continue;
      var args = line.Split(delimiter);
      if(args.Length != 2) continue;
      parametros.Add(args[0], args[1]);
    }
    return parametros;
  }
}
