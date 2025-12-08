using Serilog;
using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.WebHandler;
namespace MonitoringFieldTeam.WebScraper;
{
  public partial class Manager
  {
    private Int32 DIAS_RETROATIVOS = -30;
    public void Retroativo()
    {
      if (cfg.CONFIGURACAO.TryGetValue("RETRODAY", out String start_day))
      {
        var startdate = DateTime.ParseExact(start_day, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        DIAS_RETROATIVOS = -(DateTime.Now - startdate).Days;
      }
      var dia_now = DateOnly.FromDateTime(DateTime.Now);
      for (var dia_pri = dia_now.AddDays(DIAS_RETROATIVOS); dia_pri < dia_now; dia_pri=dia_pri.AddDays(1))
      {
        if(dia_pri.DayOfWeek == DayOfWeek.Sunday) continue;
        foreach (var piscina in cfg.PISCINAS)
        {
          if(this.TemFinalizacao(dia_pri, piscina)) continue;
          TrocarData(dia_pri);
          Atualizar(piscina, true);
          Parametrizar();
          Coletor();
          Finalizacao(false);
          Atualizar(piscina, false);
          Refresh();
        }
      }
      TrocarData(dia_now);
    }
    public void TrocarData(DateOnly data)
    {
      var data_atual = DateOnly.Parse(this.driver.FindElement(By.ClassName("toolbar-date-picker-button")).Text);
      if(data_atual == data) return;
      IWebElement calendario = TrocarData(data.ToString("MMMM yyyy").ToLower());
      foreach (var sem in calendario.FindElements(By.XPath(".//table/tbody/tr")))
      {
        foreach (var dia in sem.FindElements(By.XPath(".//td")))
        {
          if(!Int32.TryParse(dia.Text, out Int32 dia_num)) continue;
          if(dia_num == data.Day)
          {
            dia.Click();
            this.datalabel = DateOnly.Parse(this.driver.FindElement(By.ClassName("toolbar-date-picker-button")).Text);
            System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
            return;
          }
        }
      }
    }
    public IWebElement TrocarData(String dataonly)
    {
      var datepicker = this.driver.FindElements(By.ClassName("toolbar-date-range-picker-calendar"));
      if(!datepicker.Any()) this.driver.FindElement(By.ClassName("toolbar-date-picker-button")).Click();
      var calendarios = this.driver.FindElements(By.ClassName("ui-datepicker-group"));
      foreach (var elemento in calendarios)
        if(elemento.FindElement(By.XPath(".//div/div")).Text.ToLower() == dataonly) return elemento;
      this.driver.FindElement(By.ClassName("ui-datepicker-prev")).Click();
      calendarios = this.driver.FindElements(By.ClassName("ui-datepicker-group"));
      foreach (var elemento in calendarios)
        if(elemento.FindElement(By.XPath(".//div/div")).Text.ToLower() == dataonly) return elemento;
      throw new IndexOutOfRangeException("O mês solicitado não foi encontrado!");
    }
  }
}
