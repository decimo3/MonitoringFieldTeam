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