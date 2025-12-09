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
public class ElementNotEnabledException : Exception
{
  public ElementNotEnabledException() { }
  public ElementNotEnabledException(string message) : base(message) { }
  public ElementNotEnabledException(string message, Exception innerException) : base(message, innerException) { }
}
public class ElementNotDisplayedException : Exception
{
  public ElementNotDisplayedException() { }
  public ElementNotDisplayedException(string message) : base(message) { }
  public ElementNotDisplayedException(string message, Exception innerException) : base(message, innerException) { }
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
  public WebHandler()
  {
    this.url = Configuration.GetString("WEBSITE");
    string temppath = System.IO.Path.Combine(
      System.AppContext.BaseDirectory, "temp");
    var chromedriverpath = System.IO.Path.Combine(
      System.AppContext.BaseDirectory,
      "chromedriver-win64", "chromedriver.exe");
    this.service = ChromeDriverService.CreateDefaultService(chromedriverpath);
    this.options.AddArgument($@"--user-data-dir={temppath}");
    this.options.AddArgument($@"--app={url}");
    this.options.BinaryLocation = Configuration.GetString("GCHROME");
    this.options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
    this.options.AddUserProfilePreference("download.default_directory", Configuration.GetString("DATAPATH"));
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
      pathvalue = pathvalue.Replace("?", replaceText1.ToString());
    if (replaceText2 is not null)
      pathvalue = pathvalue.Replace("¿", replaceText2.ToString());
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
    if (!elements.Any()) throw new ElementNotFoundException();
    var first = elements[0];
    if (!first.Enabled) throw new ElementNotEnabledException();
    if (first.GetAttribute("type") == "checkbox") return elements;
    if (first.Displayed) return elements;
    throw new ElementNotDisplayedException();
  }
  public List<List<string>> GetTableData(IWebElement tableElement)
  {
    var resultTable = new List<List<string>>();
    var linhas = GetNestedElements(tableElement, "GLOBAL_TABLEROW");
    if (!linhas.Any()) throw new ElementNotFoundException();
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
  public bool HasClassElement
  (
    IWebElement element,
    string pathname
  )
  {
    if (!WAYPATH.TryGetValue(pathname, out string? pathvalue) || pathvalue is null)
      throw new MissingValueException($"Não foi possível obter o caminho a partir do valor `{pathname}`");
    if (!pathvalue.StartsWith('.'))
      throw new ArgumentException($"O valor passado não é um nome de classe!");
    var classes = element.GetAttribute("class").Split(" ");
    return classes.Contains(pathvalue[1..]);
  }
  public void GetScreenshot(string filename)
  {
    var largura_script = "return document.body.parentNode.scrollWidth";
    var tamanho_script = "return document.body.parentNode.scrollHeight";
    var largura_retorno = (Int64)driver.ExecuteScript(largura_script);
    var tamanho_retorno = (Int64)driver.ExecuteScript(tamanho_script);
    var largura_necessario = Int32.Parse(largura_retorno.ToString());
    var tamanho_necessario = Int32.Parse(tamanho_retorno.ToString());
    var tamanho_janela = new System.Drawing.Size(largura_necessario, tamanho_necessario);
    driver.Manage().Window.Size = tamanho_janela;
    driver.GetScreenshot().SaveAsFile(filename);
    driver.Manage().Window.Maximize();
  }
  public void SendKeyByKey(string texto)
  {
    var actions = new Actions(this.driver);
    foreach (var c in texto)
    {
      actions.KeyDown(c.ToString()).Perform();
      actions.KeyUp(c.ToString()).Perform();
    }
  }
  public void ExchangeContext(string? pathname = null)
  {
    if (pathname is null)
    {
      this.driver.SwitchTo().DefaultContent();
      return;
    }
    if (!WAYPATH.TryGetValue(pathname, out string? pathvalue) || pathvalue is null)
      throw new MissingValueException($"Não foi possível obter o caminho a partir do valor `{pathname}`");
    this.driver.SwitchTo().Frame(GetElement(pathname));
  }
  public string GetElementAttribute
  (
    IWebElement element,
    string pathname
  )
  {
    if (!WAYPATH.TryGetValue(pathname, out string? pathvalue) || pathvalue is null)
      throw new MissingValueException($"Não foi possível obter o caminho a partir do valor `{pathname}`");
    return element.GetAttribute(pathvalue);
  }
  public bool IsElementCovered(IWebElement element)
  {
    var js = @"
        var el = arguments[0];
        if (!el) return true;
        var rect = el.getBoundingClientRect();
        var points = [
          [rect.left + rect.width/2, rect.top + rect.height/2],
          [rect.left + 1, rect.top + 1],
          [rect.right - 1, rect.top + 1],
          [rect.left + 1, rect.bottom - 1],
          [rect.right - 1, rect.bottom - 1]
        ];
        var w = window.innerWidth || document.documentElement.clientWidth;
        var h = window.innerHeight || document.documentElement.clientHeight;
        for (var i = 0; i < points.length; i++) {
          var x = points[i][0], y = points[i][1];
          if (x < 0 || y < 0 || x > w || y > h) continue;
          var top = document.elementFromPoint(x, y);
          if (!top) continue;
          var node = top;
          while (node) { if (node === el) return false; node = node.parentNode; }
        }
        return true;
    ";
    var jsExecutor = (IJavaScriptExecutor)driver;
    return (bool)jsExecutor.ExecuteScript(js, element);
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
