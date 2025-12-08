using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using MonitoringFieldTeam.Helpers;
using OpenQA.Selenium.Interactions;
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
  public List<List<string>> GetTableData(IWebElement tableElement)
  {
    var resultTable = new List<List<string>>();
    var linhas = GetNestedElements(tableElement, "GLOBAL_TABLEROW");
    if (!linhas.Any()) return resultTable;
    foreach (var linha in linhas)
    {
      var valores = new List<string>();
      if (String.IsNullOrEmpty(linha.Text)) continue;
      var celulas = GetNestedElements(linha, "GLOBAL_TABLECEL");
      foreach (var celula in celulas)
      {
        if (celula is null)
          valores.Add(string.Empty);
        else
          valores.Add(celula.Text.Replace(';', ' '));
      }
      resultTable.Add(valores);
    }
    return resultTable;
  }
  public static Dictionary<String, Int32> GetElementStyle(IWebElement element)
  {
    var texto_estilo = element.GetDomAttribute("style");
    var resposta = new Dictionary<String, Int32>();
    var estilos = texto_estilo.Trim().Split(";");
    foreach (var estilo in estilos)
    {
      if (String.IsNullOrEmpty(estilo)) continue;
      var key_val = estilo.Replace(" ", "").Split(":");
      if (key_val.Length != 2) continue;
      var valor_sanitizado = key_val[1].Replace("px", "");
      if (Int32.TryParse(valor_sanitizado, out Int32 valor_numero))
      {
        resposta.Add(key_val[0], valor_numero);
        continue;
      }
      if (key_val[1] == "none") resposta.Add(key_val[0], 0);
      if (key_val[1] == "block") resposta.Add(key_val[0], 1);
    }
    return resposta;
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
