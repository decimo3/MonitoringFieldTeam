using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    public String GetServico(String arg)
    {
      var builder = new StringBuilder();
      this.driver.FindElement(By.ClassName("search-bar-input")).Click();
      var actions = new Actions(this.driver);
      foreach (var letra in arg)
      {
        actions.KeyDown(letra.ToString()).Perform();
        actions.KeyUp(letra.ToString()).Perform();
      }
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      if(!this.driver.FindElements(By.ClassName("found-item-activity")).Any())
      {
        builder.Append("A nota de servico `");
        builder.Append(arg);
        builder.Append("` não foi encontrada!");
        return builder.ToString();
      }
      // Clicar na primeira ordem de serviço
      this.driver.FindElement(By.ClassName("found-item-activity")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      var estado = GetActivityInformation(this.cfg.CAMINHOS["ACTIVITY_ESTADO"]);
      if(estado != "concluído" && estado != "não concluído")
      {
        builder.Append("A nota de servico `");
        builder.Append(arg);
        builder.Append("` não está finalizada!");
        return builder.ToString();
      }
      builder.Append($"Recurso: {this.driver.FindElement(By.ClassName("page-header-description")).Text}\n");
      builder.Append($"Atividade: {GetActivityInformation(this.cfg.CAMINHOS["ACTIVITY_ATIVIDADE"])}\n");
      builder.Append($"Serviço: {GetActivityInformation(this.cfg.CAMINHOS["ACTIVITY_SERVICO"])}\n");
      builder.Append($"Estado: {estado}\n");
      if(estado == "não concluído")
        builder.Append($"Código: {GetActivityInformation(this.cfg.CAMINHOS["ACTIVITY_REJEICAO"])}\n");
      builder.Append($"Observação: {GetActivityInformation(this.cfg.CAMINHOS["ACTIVITY_OBSERVA"])}\n");
      {
        this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_ARQUIVOS"])).Click();
        System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
        foreach(var download in this.driver.FindElements(By.ClassName("download-button")))
          download.Click();
        System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
        this.driver.FindElement(By.ClassName("oj-ux-ico-arrow-up")).Click();
        System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      }
      if(estado == "não concluído") return builder.ToString();
      {
        this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_FINALIZA"])).Click();
        System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
        var frame = this.driver.FindElement(By.ClassName("content-iframe"));
        this.driver.SwitchTo().Frame(frame);
        var cabecalhos = new string[] {"Código", "Quantidade"};
        var tabela = this.driver.FindElement(By.Id("itens-selected"));
        builder.Append(GetActivityInformation(tabela, By.XPath(".//tbody/tr"), "Código", cabecalhos));
        this.driver.SwitchTo().DefaultContent();
        this.driver.FindElement(By.ClassName("oj-ux-ico-arrow-up")).Click();
        System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      }
      {
        this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_MATERIAL"])).Click();
        System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
        var cabecalhos = new string[] {"Tipo", "Código", "Nº série", "Descrição", "Quantidade"};
        var tabelas = this.driver.FindElements(By.TagName("tbody"));
        for(var i = 0; i < tabelas.Count; i++)
        {
          var tipo_tabela = tabelas[i].GetDomAttribute("data-ofsc-inventory-pool");
          switch(tipo_tabela)
          {
            case "customer":
              builder.Append(GetActivityInformation(tabelas[i], By.XPath(".//tr"), "Material existente", cabecalhos));
            break;
            case "install":
              builder.Append(GetActivityInformation(tabelas[i], By.XPath(".//tr"), "Material instalado", cabecalhos));
            break;
            case "deinstall":
              builder.Append(GetActivityInformation(tabelas[i], By.XPath(".//tr"), "Material retirado", cabecalhos));
            break;
            default:
              builder.Append($"A tabela encontrada {tipo_tabela} não foi programada!");
            break;
          }
        }
        this.driver.FindElement(By.ClassName("oj-ux-ico-arrow-up")).Click();
        System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      }
      this.driver.FindElement(By.ClassName("oj-ux-ico-arrow-up")).Click();
      return builder.ToString();
    }
    private String GetActivityInformation(String xpath)
    {
      return this.driver.FindElement(By.XPath(xpath)).Text.Replace('\n', ' ');
    }
    private String GetActivityInformation(IWebElement tabela, By linhas_by, String prefixo, String[] cabecalhos)
    {
      var builder = new StringBuilder();
      var linhas = tabela.FindElements(linhas_by);
      if(linhas.Count == 0)
        builder.Append($"{prefixo}: SEM INFORMAÇÃO!\n");
      else if(linhas.Count == 1)
        builder.Append($"{prefixo}: {linhas.First().Text}\n");
      else
      {
        builder.Append($"Lista de {prefixo.ToLower()}s:\n");
        for(var i = 0; i < linhas.Count; i++)
        {
          if(String.IsNullOrEmpty(linhas[i].Text)) continue;
          var celulas = linhas[i].FindElements(By.XPath(".//td"));
          for(var j = 0; j < celulas.Count; j++)
          {
            builder.Append($"{cabecalhos[j]}: {celulas[j].Text}\n");
          }
        }
      }
      return builder.ToString();
    }
  }
}