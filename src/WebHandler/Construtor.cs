using System.Text;
using Automation.Persistence;
using OpenQA.Selenium.Chrome;
namespace Automation.WebScraper;
public partial class Manager : IDisposable
{
  private readonly Configuration cfg;
  private readonly ChromeDriver driver;
  private readonly ChromeDriverService service;
  private readonly ChromeOptions options = new();
  public DateTime agora { get; set; } = DateTime.Now;
  public List<Espelho> espelhos { get; set; } = new();
  public StringBuilder relatorios { get; set; } = new();
  public Int32 horario_atual { get; set; }
  public Double pixels_por_hora { get; set; }
  public Double pixels_por_minuto { get; set; }
  public Int32 contador_de_baldes { get; set; }
  public String balde_nome { get; set; } = String.Empty;
  public DateOnly datalabel { get; set; }
  public Manager(Configuration cfg)
  {
    this.service = cfg.ENVIRONMENT ?
      ChromeDriverService.CreateDefaultService() :
      ChromeDriverService.CreateDefaultService(System.IO.Directory.GetCurrentDirectory());
    this.options.AddArgument($@"--user-data-dir={cfg.DATAFOLDER}");
    this.options.AddArgument($@"--app={cfg.CONFIGURACAO["WEBSITE"]}");
    this.options.BinaryLocation = cfg.CONFIGURACAO["GCHROME"];
    this.options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
    this.options.AddUserProfilePreference("download.default_directory", cfg.TEMPFOLDER);
    this.driver = new ChromeDriver(this.service, options);
    this.driver.Manage().Window.Maximize();
    this.cfg = cfg;
  }
  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      // Dispose managed resources here.
      this.driver.Quit();
    }
    // Dispose unmanaged resources here.
  }
  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}