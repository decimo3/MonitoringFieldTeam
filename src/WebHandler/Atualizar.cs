using OpenQA.Selenium;
namespace Automation.WebScraper;
public partial class Manager
{
  public void Atualizar()
  {
    // Selecionar o balde correto conforme parâmetro RECURSO
    this.driver.FindElements(By.ClassName("rtl-prov-name")).Where(e => e.Text == this.configuration.recurso).First().Click();
    System.Threading.Thread.Sleep(configuration.ESPERA_LONGA);
    // TODO - Calcular a quantidade de minutos em um pixel de deslocamento
  }
}
