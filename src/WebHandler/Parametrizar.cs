using Serilog;
using MonitoringFieldTeam.WebHandler;
using MonitoringFieldTeam.Helpers;

namespace MonitoringFieldTeam.WebScraper
{
  public static class Parametrizador
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
    public static void VerificarPagina(WebHandler.WebHandler handler)
    {
      Log.Information("Aguardando o carregamento da página...");
      handler.GetElement("SEARCHBAR_LOGO", WebHandler.WAITSEC.Total);
      Log.Information("Página '{pagina}' carregada!", handler.GetTitle());
      var operacao = Configuration.GetString("OPERACAO");
      if (new String[] {"RETRODAY", "MONITORA", "AUTOMATO"}.Contains(operacao))
      {
        if(!handler.GetTitle().Contains("Console de Alocação"))
        {
          if(!handler.GetElement("MENU_CONSOLE_BUTTON", WAITSEC.Curto).Displayed)
              handler.GetElement("MENU_HAMBURGER_BUTTON", WAITSEC.Agora).Click();
          handler.GetElement("MENU_CONSOLE_BUTTON", WAITSEC.Agora).Click();
          VerificarPagina(handler);
        }
      }
    }
  }
}
