using OpenQA.Selenium;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Refresh()
    {
      this.espelhos = new();
      this.relatorios = new();
      this.driver.Navigate().Refresh();
      System.Threading.Thread.Sleep(this.configuration.ESPERA_LONGA);
    }
    public void Parametrizar()
    {
      // DONE - Coletar a posição do horário atual `toaGantt-time-line`
      var regua_hora_atual = this.driver.FindElement(By.ClassName("toaGantt-time-line"));
      this.horario_atual = ColetarStyle(regua_hora_atual.GetDomAttribute("style"))["left"];
      // TODO - Calcular a quantidade de minutos em um pixel de deslocamento
      var regua_hora_hora = this.driver.FindElements(By.ClassName("toaGantt-hour-line"));
      var espacos_regua_hora = new List<Int32>();
      foreach (var regua_hora in regua_hora_hora)
      {
        espacos_regua_hora.Add(ColetarStyle(regua_hora.GetDomAttribute("style"))["left"]);
      }
    }
  }
}