using Automation.Persistence;
using OpenQA.Selenium.Chrome;
namespace Automation.WebScraper;
public partial class Manager : IDisposable
{
  private readonly ChromeDriver driver;
  private readonly Configuration configuration;
  public DateTime agora { get; set; } = DateTime.Now;
  public List<Espelho> espelhos { get; set; } = new();
  public Dictionary<String, String> relatorios { get; set; } = new();
  public Int32 horario_atual { get; set; }
  public Int32 pixels_por_hora { get; set; }
  public Manager(Configuration configuration)
  {
    this.configuration = configuration;
    this.driver = new ChromeDriver(this.configuration.options);
    this.driver.Manage().Window.Maximize();
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