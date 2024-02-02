using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace automation;
public class Program : IDisposable
{
  private readonly ChromeDriver driver;
  private readonly Configuration configuration;
  public Program(Configuration configuration)
  {
    this.configuration = configuration;
    this.driver = new ChromeDriver(this.configuration.options);
    this.driver.Manage().Window.Maximize();
  }
  public void Autenticar()
  {
    System.Threading.Thread.Sleep(Configuration.ESPERA_LONGA);
    if (this.driver.FindElements(By.Id("SignOutStatusMessage")).Any())
    {
      this.driver.Navigate().GoToUrl(this.configuration.website);
    }
    if (this.driver.FindElements(By.Id("welcome-message")).Any())
    {
      this.driver.FindElement(By.Id("username")).SendKeys(configuration.usuario);
      this.driver.FindElement(By.Id("password")).SendKeys(configuration.palavra);
      this.driver.FindElement(By.Id("sign-in")).Click();
      System.Threading.Thread.Sleep(Configuration.ESPERA_CURTA);
    }
    if (this.driver.FindElements(By.Name("loginfmt")).Any())
    {
      this.driver.FindElements(By.Name("loginfmt")).Single().SendKeys(configuration.usuario);
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(Configuration.ESPERA_CURTA);
      this.driver.FindElements(By.Name("passwd")).Single().SendKeys(configuration.palavra);
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(Configuration.ESPERA_CURTA);
    }
    if (this.driver.FindElements(By.Id("lightbox")).Any())
    {
      var xpath_account = "//*[@id='tilesHolder']/div[1]/div/div[1]/div/div[1]/img";
      this.driver.FindElement(By.XPath(xpath_account)).Click();
      System.Threading.Thread.Sleep(Configuration.ESPERA_CURTA);
      this.driver.FindElements(By.Name("passwd")).Single().SendKeys(configuration.palavra);
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(Configuration.ESPERA_CURTA);
    }
    if (this.driver.FindElements(By.Name("DontShowAgain")).Any())
    {
      this.driver.FindElements(By.Name("DontShowAgain")).Single().Click();
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(Configuration.ESPERA_CURTA);
    }
    System.Threading.Thread.Sleep(Configuration.ESPERA_LONGA);
  }
  public void Inicializar()
  {
    // Selecionar a visualização do gráfico de Gantt
    this.driver.FindElements(By.ClassName("oj-ux-ico-clock")).First().Click();
    // Abrir menu de seleção de preferências
    this.driver.FindElements(By.ClassName("toolbar-item")).Where(e => e.Text == "Exibir").First().Click();
    // Selecionar para exibir de forma herarquica
    this.driver.FindElements(By.ClassName("oj-complete")).Where(e => e.Text == "Aplicar de forma hierárquica").First().Click();
    // Selecionar para exibir a rota do recurso
    this.driver.FindElements(By.ClassName("oj-complete")).Where(e => e.Text == "Exibir rota do recurso").First().Click();
    // Aplicar as preferências de seleções
    this.driver.FindElements(By.ClassName("app-button-title")).Where(e => e.Text == "Aplicar").First().Click();
    System.Threading.Thread.Sleep(Configuration.ESPERA_MEDIA);
  }
  public void Atualizar()
  {
    // Selecionar o balde correto conforme parâmetro RECURSO
    this.driver.FindElements(By.ClassName("rtl-prov-name")).Where(e => e.Text == this.configuration.recurso).First().Click();
    System.Threading.Thread.Sleep(Configuration.ESPERA_LONGA);
    var ordens = this.driver.FindElements(By.ClassName("toaGantt-tl")).Where(e => e.GetAttribute("aid") == "9147181");
    // TODO: Coletar todas as informações necessárias
    // TODO: Verificar desvios de comportamento
    // TODO: Comparar com as informações anteriores
    // TODO: Notificar se houver os novos desvios 
  }
  private void Procurar(By query)
  {
    var elementos = this.driver.FindElements(query);
    foreach (var elemento in elementos)
    {
      System.Console.WriteLine(elemento.GetDomProperty("innerHTML"));
      var json_elemento = System.Text.Json.JsonSerializer.Serialize<IWebElement>(elemento);
      System.Console.WriteLine(json_elemento);
    }
    System.Console.WriteLine();
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