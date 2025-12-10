using Serilog;
namespace MonitoringFieldTeam.WebScraper;
public partial class Manager
{
  public void Atualizar(String piscina, Boolean direcao)
  {
    this.balde_nome = piscina.Split('>').Last();
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
                // The page cover elements after the date is changed
                // Discover how to uncover elements to interact
                // "Solucionado" atualizando a página a cada troca de data
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
  public static DateOnly Atualizar(WebHandler.WebHandler handler)
  {
    Log.Information("Atualizando o gráfico...");
    // Selecionar a visualização do gráfico de Gantt
    handler.GetElement("GANNT_CLOCKICON", WebHandler.WAITSEC.Total).Click();
    // Abrir menu de seleção de preferências
    var element = handler.GetElements("GANNT_TOOLBAR").First();
    handler.GetNestedElements(element, "GANNT_SHOWBTN").First().Click();
    // Selecionar para exibir de forma herarquica
    element = handler.GetElements("GANNT_FILTERVIEW", WebHandler.WAITSEC.Curto).First();
    element = handler.GetNestedElements(element, "GANNT_TREEGROUP").First();
    element = handler.GetNestedElements(element, "GANNT_FILTERCHECK").First();
    if (!element.Selected) element.Click();
    // Selecionar para exibir a rota do recurso
    element = handler.GetElements("GANNT_FILTERVIEW", WebHandler.WAITSEC.Curto).First();
    element = handler.GetNestedElements(element, "GANNT_ROUTEGROUP").First();
    element = handler.GetNestedElements(element, "GANNT_FILTERCHECK").First();
    if (!element.Selected) element.Click();
    // Ajusta o zoom da página para visualizar toda linha do tempo
    //parent = handler.GetElements("GANNT_FILTERVIEW", WebHandler.WAITSEC.Curto).First();
    //handler.GetNestedElements(parent, "ZOOM_FIT").First().Click();
    // Aplicar as preferências de seleções
    element = handler.GetElements("GANNT_FILTERVIEW").First();
    handler.GetNestedElements(element, "GANNT_APPLYBTN").First().Click();
    Log.Information("Gráfico atualizado!");
    return DateOnly.Parse(handler.GetElement("GANNT_DATEPICK", WebHandler.WAITSEC.Curto).Text);
  }
}
