using Automation.Persistence;
using OpenQA.Selenium.Chrome;
namespace Automation.WebScraper;
public partial class Manager : IDisposable
{
  private readonly ChromeDriver driver;
  private readonly Configuration configuration;
  public List<Espelho> velho { get; set; } = new();
  public List<Espelho> atual { get; set; } = new();
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