using OpenQA.Selenium;
namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Retroativo()
    {
      var dia_now = DateOnly.FromDateTime(DateTime.Now);
      for (var dia_pri = dia_now.AddDays(-15); dia_pri < dia_now; dia_pri=dia_pri.AddDays(1))
      {
        if(dia_pri.DayOfWeek == DayOfWeek.Sunday) continue;
        foreach (var piscina in cfg.PISCINAS)
        {
          this.balde_nome = piscina.Split('>').Last();
          var filename_done = $"{this.cfg.DOWNFOLDER}\\{dia_pri.ToString("yyyyMMdd")}_{this.balde_nome}.done.csv";
          var filename_send = $"{this.cfg.DOWNFOLDER}\\{dia_pri.ToString("yyyyMMdd")}_{this.balde_nome}.send.csv";
          var tem_finalizacao = System.IO.File.Exists(filename_done) || System.IO.File.Exists(filename_send);
          if(tem_finalizacao) continue;
          TrocarData(dia_pri);
          Atualizar(piscina);
          Parametrizar();
          Coletor();
          Finalizacao();
        }
      }
      TrocarData(dia_now);
    }
    public void TrocarData(DateOnly data)
    {
      var data_atual = DateOnly.Parse(this.driver.FindElement(By.ClassName("toolbar-date-picker-button")).Text);
      if(data_atual == data) return;
      var datepicker = this.driver.FindElements(By.ClassName("toolbar-date-range-picker-calendar"));
      if(!datepicker.Any()) this.driver.FindElement(By.ClassName("toolbar-date-picker-button")).Click();
      var calendarios = this.driver.FindElements(By.ClassName("ui-datepicker-group"));
      IWebElement calendario = (calendarios[0].FindElement(By.XPath(".//div/div")).Text.ToLower() == data.ToString("MMMM yyyy")) ? calendarios[0] : calendarios[1];
      foreach (var sem in calendario.FindElements(By.XPath(".//table/tbody/tr")))
      {
        foreach (var dia in sem.FindElements(By.XPath(".//td")))
        {
          if(!Int32.TryParse(dia.Text, out Int32 dia_num)) continue;
          if(dia_num == data.Day)
          {
            dia.Click();
            return;
          }
        }
      }
    }
  }
}