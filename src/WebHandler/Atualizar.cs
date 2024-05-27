using OpenQA.Selenium;
namespace Automation.WebScraper;
public partial class Manager
{
  public void Atualizar()
  {
    // Verifica se não foi direcionado a página de logout
    if(this.driver.Url != this.configuration.website) System.Environment.Exit(1);
    var balde_nome = this.configuration.recurso[this.configuration.contador_de_baldes];
    var sub_baldes = balde_nome.Split('>');
    var baldes = this.driver.FindElements(By.ClassName("edt-item"));
    var i = 0;
    var j = 0;
    while(true)
    {
      var texto = baldes[i].GetAttribute("innerText");
      if(String.IsNullOrEmpty(texto)) continue;
      if(texto.Contains(sub_baldes[j]))
      {
        if(j > 0 && texto.Contains(sub_baldes[j - 1]))
        {
          i++;
          continue;
        }
        if(j == (sub_baldes.Count() - 1))
        {
          baldes[i].Click();
          break;
        }
        var tree_arrow = baldes[i].FindElements(By.XPath("./div/button")).First();
        if(tree_arrow.GetAttribute("class").Contains("ptplus")) tree_arrow.Click();
        System.Threading.Thread.Sleep(configuration.ESPERA_CURTA);
        baldes = this.driver.FindElements(By.ClassName("edt-item"));
        i = 0;
        j++;
        continue;
      }
      i++;
    }

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
