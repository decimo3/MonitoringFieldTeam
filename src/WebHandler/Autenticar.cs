using OpenQA.Selenium;
namespace Automation.WebScraper;
public partial class Manager
{
  public void Autenticar()
  {
    System.Threading.Thread.Sleep(configuration.ESPERA_LONGA);
    if (this.driver.FindElements(By.Id("SignOutStatusMessage")).Any())
    {
      this.driver.Navigate().GoToUrl(this.configuration.website);
      System.Threading.Thread.Sleep(configuration.ESPERA_CURTA);
    }
    if (this.driver.FindElements(By.Id("welcome-message")).Any())
    {
      this.driver.FindElement(By.Id("username")).SendKeys(configuration.usuario);
      this.driver.FindElement(By.Id("password")).SendKeys(configuration.palavra);
      this.driver.FindElement(By.Id("sign-in")).Click();
      System.Threading.Thread.Sleep(configuration.ESPERA_CURTA);
    }
    if (this.driver.FindElements(By.Name("loginfmt")).Any())
    {
      this.driver.FindElements(By.Name("loginfmt")).Single().SendKeys(configuration.usuario);
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(configuration.ESPERA_CURTA);
      this.driver.FindElements(By.Name("passwd")).Single().SendKeys(configuration.palavra);
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(configuration.ESPERA_CURTA);
    }
    if (this.driver.FindElements(By.Id("lightbox")).Any())
    {
      var xpath_account = "//*[@id='tilesHolder']/div[1]/div/div[1]/div/div[1]/img";
      this.driver.FindElement(By.XPath(xpath_account)).Click();
      System.Threading.Thread.Sleep(configuration.ESPERA_CURTA);
      this.driver.FindElements(By.Name("passwd")).Single().SendKeys(configuration.palavra);
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(configuration.ESPERA_CURTA);
    }
    if (this.driver.FindElements(By.Name("DontShowAgain")).Any())
    {
      this.driver.FindElements(By.Name("DontShowAgain")).Single().Click();
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(configuration.ESPERA_CURTA);
    }
    System.Threading.Thread.Sleep(configuration.ESPERA_LONGA);
  }
}