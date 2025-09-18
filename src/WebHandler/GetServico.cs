using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Automation.Persistence;
using System.Collections.ObjectModel;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    private void BackToBlack()
    {
      GetElement(By.ClassName("oj-ux-ico-arrow-up")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    }
    private bool IsFinished()
    {
      return (GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_ESTADO"]))?.Text ?? string.Empty).Contains("concluído");
    }
    private ReadOnlyCollection<IWebElement> GetElements(By by, int timeoutInSeconds = 5)
    {
      var endTime = DateTime.Now.AddSeconds(timeoutInSeconds);
      while (DateTime.Now < endTime)
      {
        try
        {
          var elements = this.driver.FindElements(by);
          if (elements.Count == 0)
            continue;
          var element = elements.First();
          if (element.Displayed && element.Enabled)
            return elements;
        }
        catch (NoSuchElementException)
        {
          // Element not yet in DOM, keep trying
        }
        catch (StaleElementReferenceException)
        {
          // DOM updated, try again
        }
        catch (WebDriverException)
        {
          // Catch all transient WebDriver errors, continue retrying
        }
        Thread.Sleep(200); // Avoid busy waiting
      }

      throw new TimeoutException($"Element not interactable after {timeoutInSeconds} seconds: {by}");
    }
    private IWebElement GetElement(By by)
    {
      var elements = GetElements(by);
      if (elements.Count > 1)
        throw new Exception($"Mais de um elemento encontrado: {by}");
      return elements[0];
    }
    private List<List<string>> GetTableActivity(IWebElement tableElement)
    {
      var resultTable = new List<List<string>>();
      var linhas = tableElement.FindElements(By.XPath(".//tr"));
      if (linhas.Count == 0)
        return resultTable;
      foreach (var linha in linhas)
      {
        var valores = new List<string>();
        if (String.IsNullOrEmpty(linha.Text)) continue;
        var celulas = linha.FindElements(By.XPath(".//td"));
        foreach (var celula in celulas)
        {
            valores.Add(celula.Text.Replace(';',' '));
        }
        resultTable.Add(valores);
      }
      return resultTable;
    }
    private static List<MaterialInfo> GetListMaterialByListString(List<List<string>> listString, string nota, string origem)
    {
      var materiais = new List<MaterialInfo>();
      foreach (var values in listString)
      {
        var material = new MaterialInfo();
        material.Nota = nota;
        material.Tipo = values[0];
        material.Codigo = values[1];
        material.Serie = values[2];
        material.Descricao = values[3];
        material.Quantidade = values[4];
        material.Origem = origem;
        materiais.Add(material);
      }
      return materiais;
    }
    public void SearchAndEnterActivity(String workorder)
    {
      // Click on search bar to focus cursor on
      GetElements(By.ClassName("search-bar-input")).First().Click();
      // Fill search bar with workorder number char by char
      var actions = new Actions(this.driver);
      foreach (var c in workorder)
      {
        actions.KeyDown(c.ToString()).Perform();
        actions.KeyUp(c.ToString()).Perform();
      }
      // Await amount of time and check if there is a response
      if(!GetElements(By.ClassName("found-item-activity")).Any())
      {
        throw new Exception($"A nota de serviço não foi encontrada!");
      }
      // Click on the first workorder on list
      GetElements(By.ClassName("found-item-activity")).First().Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    }
    public String GetActivityGeneralInfo()
    {
      var builder = new System.Text.StringBuilder();
      var estado = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_ESTADO"])).Text;
      if (estado != "concluído" && estado != "não concluído")
      {
        builder.Append("A nota de servico não está finalizada!");
        return builder.ToString();
      }
      builder.Append($"Recurso: {this.driver.FindElement(By.ClassName("page-header-description")).Text}\n");
      builder.Append($"Atividade: {GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_ATIVIDADE"])).Text}\n");
      builder.Append($"Serviço: {GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_SERVICO"])).Text}\n");
      builder.Append($"Estado: {estado}\n");
      if (estado == "não concluído")
        builder.Append($"Código: {GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_REJEICAO"])).Text}\n");
      builder.Append($"Observação: {GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_OBSERVA"])).Text.Replace('\n', ' ')}\n");
      return builder.ToString();
    }
    public void GetActivityUploads()
    {
      GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_ARQUIVOS"])).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      foreach (var download in GetElements(By.ClassName("download-button")))
        download.Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      GetElement(By.ClassName("oj-ux-ico-arrow-up")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    }
    public string GetActivityClosings()
    {
      var builder = new System.Text.StringBuilder();
      GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_FINALIZA"])).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      var frame = GetElement(By.ClassName("content-iframe"));
      this.driver.SwitchTo().Frame(frame);
      var cabecalhos = new string[] { "Código", "Quantidade" };
      var tabela = GetElement(By.TagName("tbody")); // By.Id("itens-selected")
      var tabelaResult = GetTableActivity(tabela);
      foreach (var result in tabelaResult)
      {
        builder.Append("Código: ");
        builder.Append(result[0]);
        builder.Append('\n');
        builder.Append("Quantidade: ");
        builder.Append(result[1]);
        builder.Append('\n');
      }
      this.driver.SwitchTo().DefaultContent();
      GetElement(By.ClassName("oj-ux-ico-arrow-up")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      return builder.ToString();
    }
    public List<MaterialInfo> GetActivityMaterials(string nota)
    {
      var result = new List<MaterialInfo>();
      GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_MATERIAL"])).Click();
      var tabelas = GetElements(By.TagName("tbody"));
      foreach (var tabela in tabelas)
      {
        var origem = tabela.GetDomAttribute("data-ofsc-inventory-pool");
        var conteudoTabela = GetTableActivity(tabela);
        var materialTabela = GetListMaterialByListString(conteudoTabela, nota, origem);
        result.AddRange(materialTabela);
      }
      GetElement(By.ClassName("oj-ux-ico-arrow-up")).Click();
      return result;
    }
    public String GetServico(String arg)
    {
      var builder = new System.Text.StringBuilder();
      try
      {
        SearchAndEnterActivity(arg);
      }
      catch (Exception e)
      {
        builder.Append(e.Message);
        return builder.ToString();
      }
      var estado = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_ESTADO"])).Text;
      if (estado != "concluído" && estado != "não concluído")
      {
        builder.Append("A nota de servico não está finalizada!");
        return builder.ToString();
      }
      try
      {
        builder.Append(GetActivityGeneralInfo());
      }
      catch (Exception e)
      {
        builder.Append(e.Message);
        return builder.ToString();
      }
      try
      {
        GetActivityUploads();
      }
      catch (Exception e)
      {
        builder.Append(e.Message);
        return builder.ToString();
      }
      try
      {
        builder.Append(GetActivityClosings());
        BackToBlack();
      }
      catch (Exception e)
      {
        builder.Append(e.Message);
        return builder.ToString();
      }
      try
      {
        var materiais = GetActivityMaterials(arg);
        foreach (var material in materiais)
        {
          builder.Append(material.ToString());
          builder.Append('\n');
        }
      }
      catch (Exception e)
      {
        builder.Append(e.Message);
        return builder.ToString();
      }
      return builder.ToString();
    }
  }
}
