namespace Automation.Helpers;
public class Configuration
{
  public readonly String DATAFOLDER;
  public readonly String DOWNFOLDER;
  public readonly String TEMPFOLDER;
  public readonly String LOCKFILE;
  public readonly Int32 TOLERANCIA = 3;
  public readonly List<String> PISCINAS;
  public readonly List<String> EXTRACAO_KEY;
  public readonly Boolean ENVIRONMENT = false;
  public readonly Dictionary<String, String> CONFIGURACAO = new();
  public readonly Dictionary<String, String> CAMINHOS = new();
  public readonly Dictionary<String, Int32> HORARIOS = new();
  public readonly Dictionary<String, Int64> BOT_CHANNELS = new();
  public readonly Dictionary<String, Int32> ESPERAS = new()
  {
    {"TOTAL", 60_000},
    {"LONGA", 10_000},
    {"MEDIA", 6_000},
    {"CURTA", 3_000},
  };
  public Configuration()
  {
    if(System.Environment.GetCommandLineArgs().Contains("debug"))
    {
      this.ENVIRONMENT = true;
    }
    this.LOCKFILE = System.IO.Path.Combine(System.AppContext.BaseDirectory, "ofs.lock");
    this.DATAFOLDER = System.IO.Path.Combine(System.AppContext.BaseDirectory, "www");
    if(!System.IO.Directory.Exists(this.DATAFOLDER)) System.IO.Directory.CreateDirectory(this.DATAFOLDER);
    this.DOWNFOLDER = System.IO.Path.Combine(System.AppContext.BaseDirectory, "odl");
    if(!System.IO.Directory.Exists(this.DOWNFOLDER)) System.IO.Directory.CreateDirectory(this.DOWNFOLDER);
    this.TEMPFOLDER = System.IO.Path.Combine(System.AppContext.BaseDirectory, "tmp");
    if(!System.IO.Directory.Exists(this.TEMPFOLDER)) System.IO.Directory.CreateDirectory(this.TEMPFOLDER);

    if(System.Environment.GetCommandLineArgs().Contains("slower"))
      foreach(var key in this.ESPERAS.Keys.ToList()) this.ESPERAS[key] *= 2;
    if(System.Environment.GetCommandLineArgs().Contains("faster"))
      foreach(var key in this.ESPERAS.Keys.ToList()) this.ESPERAS[key] /= 2;

    this.CONFIGURACAO = ArquivoConfiguracao(
      System.IO.Path.Combine(System.AppContext.BaseDirectory, "ofs.conf"));

    if(this.CONFIGURACAO.TryGetValue("ODLPATH", out String? odl_path))
    {
      if(!String.IsNullOrWhiteSpace(odl_path) && System.IO.Directory.Exists(odl_path))
      {
        this.DOWNFOLDER = odl_path;
      }
    }

    if(this.CONFIGURACAO.TryGetValue("TMPPATH", out String? tmp_path))
    {
      if(!String.IsNullOrWhiteSpace(tmp_path) && System.IO.Directory.Exists(tmp_path))
      {
        this.TEMPFOLDER = tmp_path;
      }
    }

    this.PISCINAS = this.CONFIGURACAO["RECURSO"].Split(",").ToList();
    foreach(var channel in this.CONFIGURACAO["BOT_CHANNEL"].Split(",").ToList())
    {
      var channel_args = channel.Split('|');
      if(channel_args.Length != 2) continue;
      this.BOT_CHANNELS.Add(channel_args.First(), Int64.Parse(channel_args.Last()));
    }

    this.CAMINHOS = ArquivoConfiguracao("ofs.path");
    foreach(var horario in this.CONFIGURACAO["HORARIO"].Split(",").ToList())
    {
      var horario_string = horario.Split('|');
      if(horario_string.Length != 2) continue;
      this.HORARIOS.Add(horario_string.First(), Int32.Parse(horario_string.Last()));
    }
    this.EXTRACAO_KEY = this.CONFIGURACAO["EXTRACAO"].Split(',').ToList();
  }
  private Dictionary<String,String> ArquivoConfiguracao(String filename, char delimiter = '=')
  {
    var parametros = new Dictionary<string,string>();
    var file = System.IO.File.ReadAllLines(filename);
    foreach (var line in file)
    {
      if(String.IsNullOrEmpty(line)) continue;
      if(line.StartsWith('#')) continue;
      var args = line.Split(delimiter);
      if(args.Length != 2) continue;
      parametros.Add(args[0], args[1]);
    }
    return parametros;
  }
}
