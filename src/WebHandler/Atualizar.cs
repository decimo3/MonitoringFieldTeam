using OpenQA.Selenium;
namespace Automation.WebScraper;
public partial class Manager
{
  public void Atualizar()
  {
    // Selecionar o balde correto conforme parâmetro RECURSO
    var baldes = this.driver.FindElements(By.ClassName("rtl-prov-name"));
    // Verifica se foi encontrado algum elemento 
    if(!baldes.Any()) System.Environment.Exit(1);
    var balde = this.configuration.recurso[this.configuration.contador_de_baldes];
    baldes.Where(e => e.Text == balde).First().Click();
    System.Threading.Thread.Sleep(configuration.ESPERA_MEDIA);
    // Selecionar a visualização do gráfico de Gantt
    this.driver.FindElements(By.ClassName("oj-ux-ico-clock")).First().Click();
    // Abrir menu de seleção de preferências
    this.driver.FindElements(By.ClassName("toolbar-item")).Where(e => e.Text == "Exibir").First().Click();
    // Selecionar para exibir de forma herarquica
    var checkbox_hierarquico = this.driver.FindElement(By.XPath(this.configuration.pathfind["CHECK_TREE"]));
    if(!checkbox_hierarquico.Selected) checkbox_hierarquico.Click();
    // Selecionar para exibir a rota do recurso
    var checkbox_exibir_rota = this.driver.FindElement(By.XPath(this.configuration.pathfind["CHECK_ROUTE"]));
    if(!checkbox_exibir_rota.Selected) checkbox_exibir_rota.Click();
    // Aplicar as preferências de seleções
    this.driver.FindElements(By.ClassName("app-button-title")).Where(e => e.Text == "Aplicar").First().Click();
    System.Threading.Thread.Sleep(configuration.ESPERA_LONGA);
    this.espelhos = new();
    this.relatorios = new();
    this.agora = DateTime.Now;
  }
}
