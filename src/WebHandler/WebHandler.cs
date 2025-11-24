using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Automation.Helpers;
namespace MonitoringFieldTeam.WebHandler;
public class MissingValueException : Exception
{
  public MissingValueException() { }
  public MissingValueException(string message) : base(message) { }
  public MissingValueException(string message, Exception innerException) : base(message, innerException) { }
}
public class ElementNotFoundException : Exception
{
  public ElementNotFoundException() { }
  public ElementNotFoundException(string message) : base(message) { }
  public ElementNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
public enum WAITSEC : int { Agora = 0, Curto = 3, Medio = 7, Longo = 15, Total = 30 };
public sealed class WebHandler : IDisposable
{
  private readonly string url = "";
  private readonly ChromeDriver driver;
  private readonly ChromeDriverService service;
  private readonly ChromeOptions options = new();
  private readonly Dictionary<string, string> WAYPATH;
  private readonly Dictionary<char, Func<string, By>> BY = new()
  {
    {'#', By.Id},
    {'/', By.XPath},
    {'.', By.ClassName}
  };
  private const int MILISECONDS_TIMECHECK_INTERVAL = 200;
  public WebHandler(Configuration cfg)
  {
    this.url = cfg.CONFIGURACAO["WEBSITE"];
    var chromedriverpath = System.IO.Path.Combine(
      System.AppContext.BaseDirectory,
      "chromedriver-win64",
      "chromedriver.exe");
    this.service = cfg.ENVIRONMENT ?
      ChromeDriverService.CreateDefaultService() :
      ChromeDriverService.CreateDefaultService(chromedriverpath);
    this.options.AddArgument($@"--user-data-dir={cfg.TEMPFOLDER}");
    this.options.AddArgument($@"--app={cfg.CONFIGURACAO["WEBSITE"]}");
    this.options.BinaryLocation = cfg.CONFIGURACAO["GCHROME"];
    this.options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
    this.options.AddUserProfilePreference("download.default_directory", cfg.DATAFOLDER);
    this.driver = new ChromeDriver(this.service, options);
    this.driver.Manage().Window.Maximize();
    var pathfind_filepath = System.IO.Path.Combine(
      System.AppContext.BaseDirectory, "ofs.path");
    this.WAYPATH = Configuration.ArquivoConfiguracao(pathfind_filepath);
  }
  public IList<IWebElement> GetElements
  (
    string pathname,
    WAITSEC timeout = WAITSEC.Agora,
    int? replaceText1 = null,
    int? replaceText2 = null
  )
  {
    if (!WAYPATH.TryGetValue(pathname, out string? pathvalue) || pathvalue is null)
      throw new MissingValueException($"Não foi possível obter o caminho a partir do valor `{pathname}`");
    if (replaceText1 is not null)
      pathvalue = pathvalue.Replace('?', (char)(replaceText1 + '0'));
    if (replaceText2 is not null)
      pathvalue = pathvalue.Replace('¿', (char)(replaceText2 + '0'));
    var bytype = pathvalue[0];
    if (!BY.TryGetValue(bytype, out Func<string, By>? byfunc) || byfunc is null)
      throw new MissingValueException($"Não foi possível obter o meio a partir do caminho `{bytype}`!");
    var byvalue = (byfunc == By.XPath) ? pathvalue : pathvalue[1..];
    var expiration = DateTime.Now + TimeSpan.FromSeconds((int)timeout);
    do
    {
      var elements = this.driver.FindElements(byfunc(byvalue));
      if (elements.Any() && elements[0].Displayed && elements[0].Enabled)
      {
        return elements;
      }
      System.Threading.Thread.Sleep(MILISECONDS_TIMECHECK_INTERVAL);
    } while (DateTime.Now < expiration);
    return new List<IWebElement>();
  }
  public IWebElement GetElement
  (
    string pathname,
    WAITSEC timeout = WAITSEC.Agora,
    int? replaceText1 = null,
    int? replaceText2 = null,
    object? setValue = null
  )
  {
    var elements = this.GetElements(pathname, timeout, replaceText1, replaceText2);
    if (!elements.Any())
      throw new ElementNotFoundException();
    var element = elements[0];
    if (setValue is not null)
    {
      element.Click();
      element.Clear();
      switch (setValue)
      {
        case string s:
          element.SendKeys(s);
          break;
        case int i:
          element.SendKeys(i.ToString());
          break;
        case IEnumerable<string> l:
          element.SendKeys(string.Join(System.Environment.NewLine, l));
          break;
        default:
          throw new ArgumentException($"O valor informado `{setValue}` é inválido!");
      }
    }
    return element;
  }
  public void ReloadWebPage()
  {
    // Erase cached data to a clean run
    this.driver.ExecuteCdpCommand("Network.enable", new Dictionary<string, object>());
    this.driver.ExecuteCdpCommand("Network.clearBrowserCookies", new Dictionary<string, object>());
    this.driver.ExecuteCdpCommand("Network.clearBrowserCache", new Dictionary<string, object>());
    this.driver.ExecuteScript("window.localStorage.clear()");
    this.driver.ExecuteScript("window.sessionStorage.clear()");
    this.driver.Navigate().GoToUrl(this.url);
  }
  public IList<IWebElement> GetNestedElements
  (
    IWebElement parentElement,
    string pathname
  )
  {
    if (!WAYPATH.TryGetValue(pathname, out string? pathvalue) || pathvalue is null)
      throw new MissingValueException($"Não foi possível obter o caminho a partir do valor `{pathname}`");
    var elements = parentElement.FindElements(By.XPath('.' + pathvalue));
    if (elements.Any() && elements[0].Displayed && elements[0].Enabled)
    {
      return elements;
    }
    return new List<IWebElement>();
  }
  private void Dispose(bool disposing)
  {
    if (disposing)
    {
      driver?.Quit();
      driver?.Dispose();
      service?.Dispose();
    }
  }
  public void Dispose()
  {
      Dispose(true);
      GC.SuppressFinalize(this);
  }
}
