using OpenQA.Selenium;
namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Retroativo()
    {
      var dia_now = DateOnly.FromDateTime(DateTime.Now);
      for (var dia_pri = new DateOnly(agora.Year, agora.Month, 1); dia_pri < dia_now; dia_pri.AddDays(1))
      {
        foreach (var piscina in cfg.PISCINAS)
        {
          var balde_nome = piscina.Split('>').Last();
          var filename_done = $"{this.cfg.DOWNFOLDER}\\{dia_pri.ToString("yyyyMMdd")}_{balde_nome}.done.csv";
          var filename_send = $"{this.cfg.DOWNFOLDER}\\{dia_pri.ToString("yyyyMMdd")}_{balde_nome}.send.csv";
          var tem_finalizacao = System.IO.File.Exists(filename_done) || System.IO.File.Exists(filename_send);
          if(tem_finalizacao) continue;
          TrocarData(dia_pri);
          Atualizar(piscina);
          Parametrizar();
          Coletor();
          Finalizacao();
        }
      }
    }
    public void TrocarData(DateOnly data)
    {
      this.driver.FindElement(By.ClassName("toolbar-date-picker-button")).Click();
      var calendarios = this.driver.FindElements(By.ClassName("ui-datepicker-group"));
      IWebElement calendario = (calendarios[0].FindElement(By.XPath(".//span")).Text.ToLower() == data.ToString("MMMM yyyy")) ? calendarios[0] : calendarios[1];
      foreach (var sem in calendario.FindElements(By.XPath(".//table/tbody/tr")))
      {
        foreach (var dia in sem.FindElements(By.XPath(".//td")))
        {
          if(Int32.Parse(dia.Text) == data.Day) dia.Click();
        }
      }
    }
  }
}