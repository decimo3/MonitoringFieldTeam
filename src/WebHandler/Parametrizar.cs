using Serilog;
using MonitoringFieldTeam.WebHandler;

namespace MonitoringFieldTeam.WebScraper
{
  public partial class Manager
  {
    public static int ObterHoraAtual(WebHandler.WebHandler handler)
    {
      var regua_hora_atual = handler.GetElement("PARAMETRO_HORAATUAL");
      return handler.GetElementStyle(regua_hora_atual)["left"];
    }
    public static int ObterPixelsPorMinuto(WebHandler.WebHandler handler)
    {
      var regua_hora_hora = handler.GetElements("PARAMETRO_HORALINHA", WAITSEC.Medio);
      if (!regua_hora_hora.Any())
        throw new ElementNotFoundException("não foi encontrado nenhum elemento pelo caminho `PARAMETRO_HORALINHA`!");
      var pixel_1th_hora = handler.GetElementStyle(regua_hora_hora[0])["left"];
      var pixel_2th_hora = handler.GetElementStyle(regua_hora_hora[1])["left"];
      return (pixel_2th_hora - pixel_1th_hora) / 60;
    }
    public void ProximoBalde()
    {
      this.contador_de_baldes = (this.contador_de_baldes + 1) % this.cfg.PISCINAS.Count;
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["LONGA"]);
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
    public static void VerificarPagina(WebHandler.WebHandler handler)
    {
      Log.Information("Verificando a página...");
      handler.GetElement("GANNT_DISPLAY", WebHandler.WAITSEC.Total);
    }
  }
}
