using OpenQA.Selenium;
namespace Automation.WebScraper;
public partial class Manager
{
  public void Atualizar(String piscina, Boolean direcao)
  {
    var sub_baldes = direcao ? piscina.Split('>') : piscina.Split('>').Reverse().ToArray();
    for (var i = 0; i < sub_baldes.Length; i++)
    {
      var baldes = this.driver.FindElements(By.ClassName("edt-item"));
      for (var j = 0; j < baldes.Count; j++)
      {
        if(j == baldes.Count - 1)
        {
          ProximoBalde();
          throw new InvalidOperationException($"O balde {sub_baldes[j]} não foi encontrado!");
        }
        var texto = baldes[j].GetAttribute("innerText");
        if(String.IsNullOrEmpty(texto)) continue;
        if(texto.Contains(sub_baldes[i]))
        {
          if(direcao)
          {
            if(i > 0)
            {
              if(texto.Contains(sub_baldes[i - 1]))
              {
                continue;
              }
            }
            if(i == (sub_baldes.Length - 1))
            {
              if(baldes[j].Displayed)
              {
                baldes[j].Click();
                break;
              }
              else
              {
                // TODO - Solucionar ElementNotInteractableException()
                throw new NotImplementedException();
              }
            }
          }
          else
          {
            if(i < sub_baldes.Length - 1)
            {
              if(texto.Contains(sub_baldes[i + 1]))
              {
                continue;
              }
            }
            if(i == (sub_baldes.Length))
            {
              if(baldes[j].Displayed)
              {
                baldes[j].Click();
                break;
              }
            }
          }
          var tree_arrow = baldes[j].FindElements(By.XPath("./div/button")).First();
          var arrow_class = direcao ? "ptplus" : "ptminus";
          if(tree_arrow.GetAttribute("class").Contains(arrow_class)) tree_arrow.Click();
          System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
          break;
        }
      }
    }
    if(!direcao) return;
    System.Threading.Thread.Sleep(this.cfg.ESPERAS["MEDIA"]);
    // Selecionar a visualização do gráfico de Gantt
    this.driver.FindElements(By.ClassName("oj-ux-ico-clock")).First().Click();
    // Abrir menu de seleção de preferências
    this.driver.FindElements(By.ClassName("toolbar-item")).Where(e => e.Text == "Exibir").First().Click();
    System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    // Selecionar para exibir de forma herarquica
    var checkbox_hierarquico = this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["CHECK_TREE"]));
    if(!checkbox_hierarquico.Selected) checkbox_hierarquico.Click();
    // Selecionar para exibir a rota do recurso
    var checkbox_exibir_rota = this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["CHECK_ROUTE"]));
    if(!checkbox_exibir_rota.Selected) checkbox_exibir_rota.Click();
    // Ajusta o zoom da página para visualizar toda linha do tempo
    // this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["ZOOM_FIT"])).Click();
    // Aplicar as preferências de seleções
    this.driver.FindElements(By.ClassName("app-button-title")).Where(e => e.Text == "Aplicar").First().Click();
    System.Threading.Thread.Sleep(this.cfg.ESPERAS["LONGA"]);
    this.datalabel = DateOnly.Parse(this.driver.FindElement(By.ClassName("toolbar-date-picker-button")).Text);
    this.espelhos = new();
    this.relatorios = new();
    this.agora = DateTime.Now;
  }
}
