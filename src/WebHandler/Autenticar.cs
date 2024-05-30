using OpenQA.Selenium;
namespace Automation.WebScraper;
public partial class Manager
{
  public void Autenticar()
  {
    System.Threading.Thread.Sleep(this.cfg.ESPERAS["LONGA"]);
    if (this.driver.FindElements(By.Id("SignOutStatusMessage")).Any())
    {
      this.driver.Navigate().GoToUrl(this.cfg.CONFIGURACAO["WEBSITE"]);
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    }
    if (this.driver.FindElements(By.Id("welcome-message")).Any())
    {
      this.driver.FindElement(By.Id("username")).SendKeys(this.cfg.CONFIGURACAO["USUARIO"]);
      this.driver.FindElement(By.Id("password")).SendKeys(this.cfg.CONFIGURACAO["PALAVRA"]);
      this.driver.FindElement(By.Id("sign-in")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    }
    if (this.driver.FindElements(By.Name("loginfmt")).Any())
    {
      this.driver.FindElements(By.Name("loginfmt")).Single().SendKeys(this.cfg.CONFIGURACAO["USUARIO"]);
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      this.driver.FindElements(By.Name("passwd")).Single().SendKeys(this.cfg.CONFIGURACAO["PALAVRA"]);
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    }
    var xpath_account = "//*[@id='tilesHolder']/div[1]/div/div[1]/div/div[1]/img";
    if (this.driver.FindElements(By.XPath(xpath_account)).Any())
    {
      this.driver.FindElement(By.XPath(xpath_account)).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      this.driver.FindElements(By.Name("passwd")).Single().SendKeys(this.cfg.CONFIGURACAO["PALAVRA"]);
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    }
    if (this.driver.FindElements(By.Name("DontShowAgain")).Any())
    {
      this.driver.FindElements(By.Name("DontShowAgain")).Single().Click();
      this.driver.FindElement(By.Id("idSIButton9")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    }
    System.Threading.Thread.Sleep(this.cfg.ESPERAS["LONGA"]);
  }
}