using OpenQA.Selenium.Chrome;

namespace automation;
public class Configuration
{
  public readonly string usuario;
  public readonly string palavra;
  public readonly ChromeOptions options;
  public readonly string caminho;
  public readonly string website;
  public readonly string gchrome;
  public readonly string recurso;
  public Configuration()
  {
    var file = System.IO.File.ReadAllLines(".env");
    foreach (var line in file)
    {
      if (line == null) continue;
      if (line == String.Empty) continue;
      var args = line.Split('=');
      if (args.Length < 2) throw new IndexOutOfRangeException("O arquivo contém valores inválidos!");
      var cfg = args[0];
      var val = args[1];
      switch(cfg)
      {
        case "WEBSITE": this.website = val; break;
        case "USUARIO": this.usuario = val; break;
        case "PALAVRA": this.palavra = val; break;
        case "GCHROME": this.gchrome = val; break;
        case "RECURSO": this.recurso = val; break;
        default: throw new InvalidOperationException("O arquivo de configuração é inválido!");
      }
    }
    if (this.website == null) throw new InvalidOperationException("O parâmetro WEBSITE não foi encontrado!");
    if (this.usuario == null) throw new InvalidOperationException("O parâmetro USUARIO não foi encontrado!");
    if (this.palavra == null) throw new InvalidOperationException("O parâmetro PALAVRA não foi encontrado!");
    if (this.gchrome == null) throw new InvalidOperationException("O parâmetro GCHROME não foi encontrado!");
    if (this.recurso == null) throw new InvalidOperationException("O parâmetro RECURSO não foi encontrado!");
    this.caminho = @$"{System.IO.Directory.GetCurrentDirectory()}\www";
    this.options = new ChromeOptions();
    this.options.AddArgument($@"--user-data-dir={this.caminho}");
    this.options.AddArgument($@"--app={this.website}");
    this.options.BinaryLocation = this.gchrome;
  }
}
