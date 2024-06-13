using OpenQA.Selenium;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Refresh()
    {
      this.driver.Navigate().Refresh();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["LONGA"]);
    }
    public void Parametrizar()
    {
      // DONE - Coletar a posição do horário atual `toaGantt-time-line`
      var regua_hora_atual = this.driver.FindElement(By.ClassName("toaGantt-time-line"));
      this.horario_atual = ColetarStyle(regua_hora_atual.GetDomAttribute("style"))["left"];
      // TODO - Calcular a quantidade de minutos em um pixel de deslocamento
      var regua_hora_hora = this.driver.FindElements(By.ClassName("toaGantt-hour-line"));
      var pixel_1th_hora = ColetarStyle(regua_hora_hora[1].GetDomAttribute("style"))["left"];
      var pixel_2th_hora = ColetarStyle(regua_hora_hora[0].GetDomAttribute("style"))["left"];
      this.pixels_por_hora = pixel_1th_hora - pixel_2th_hora;
      this.pixels_por_minuto = this.pixels_por_hora / 60;
    }
    public void ProximoBalde()
    {
      this.contador_de_baldes = (this.contador_de_baldes + 1) % this.cfg.PISCINAS.Count;
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["LONGA"]);
    }
    public bool TemFinalizacao()
    {
      this.balde_nome = this.cfg.PISCINAS[this.contador_de_baldes].Split('>').Last();
      var filename_done = $"{this.cfg.DOWNFOLDER}\\{this.datalabel.ToString("yyyyMMdd")}_{this.balde_nome}.done.csv";
      var filename_send = $"{this.cfg.DOWNFOLDER}\\{this.datalabel.ToString("yyyyMMdd")}_{this.balde_nome}.send.csv";
      var tem_finalizacao = System.IO.File.Exists(filename_done) || System.IO.File.Exists(filename_send);
      return tem_finalizacao;
    }
    public void SimpleProgressBar(Int32 atual, Int32 maximo, String prefixo)
    {
      var i = (Int32)Math.Ceiling((Double)(atual + 1) / (Double)maximo * 100);
      var s = i < 10 ? 2 : i < 100 ? 1 : i < 1000 ? 0 : 0;
      var j = atual < 10 ? 2 : atual < 100 ? 1 : 0;
      var k = maximo < 10 ? 2 : maximo < 100 ? 1 : 0;
      if(i < 100)
      {
        Console.Write($"{prefixo} {new String(' ', s)}{i}% [{new String('#', i)}{new String(' ', 100 - i)}] {new String(' ', j)}{atual}/{new String(' ', k)}{maximo}\r");
      }
      else
      {
        Console.Write($"{prefixo} {new String(' ', s)}{i}% [{new String('#', i)}{new String(' ', 100 - i)}] {new String(' ', j)}{atual}/{new String(' ', k)}{maximo}\n");
      }
    }
  }
}