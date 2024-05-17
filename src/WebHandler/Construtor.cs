using Automation.Persistence;
using OpenQA.Selenium.Chrome;
namespace Automation.WebScraper;
public partial class Manager : IDisposable
{
  private readonly ChromeDriver driver;
  private readonly ChromeDriverService service;
  private readonly Configuration configuration;
  public DateTime agora { get; set; } = DateTime.Now;
  public List<Espelho> espelhos { get; set; } = new();
  public Dictionary<String, String> relatorios { get; set; } = new();
  public Int32 horario_atual { get; set; }
  public Double pixels_por_hora { get; set; }
  public Double pixels_por_minuto { get; set; }
  public Manager(Configuration configuration)
  {
    this.service = configuration.is_development ?
      ChromeDriverService.CreateDefaultService() :
      ChromeDriverService.CreateDefaultService(System.IO.Directory.GetCurrentDirectory());
    this.driver = new ChromeDriver(this.service, configuration.options);
    this.driver.Manage().Window.Maximize();
    this.configuration = configuration;
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