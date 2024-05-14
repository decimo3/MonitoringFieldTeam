using OpenQA.Selenium;
namespace Automation.WebScraper;
public partial class Manager
{
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
    System.Threading.Thread.Sleep(Configuration.ESPERA_LONGA);
  }
}