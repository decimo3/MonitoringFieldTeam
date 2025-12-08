using Serilog;
using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.WebHandler;
namespace MonitoringFieldTeam.WebScraper;

public static class Autenticador
{
  public static void Autenticar(WebHandler.WebHandler handler, Configuration cfg)
  {
    if (handler.GetElements("AUTENTICAR_OFSAUTH", WAITSEC.Medio).Any())
    {
      handler.GetElement("AUTENTICAR_OFSUSER", WAITSEC.Agora).SendKeys(cfg.CONFIGURACAO["USUARIO"]);
      handler.GetElement("AUTENTICAR_OFSPASS", WAITSEC.Agora).SendKeys(cfg.CONFIGURACAO["PALAVRA"]);
      handler.GetElement("AUTENTICAR_OFSLOGIN", WAITSEC.Agora).Click();
    }
    if (handler.GetElements("AUTENTICAR_WFMAUTH", WAITSEC.Medio).Any())
    {
      handler.GetElement("AUTENTICAR_WFMUSER", WAITSEC.Curto).SendKeys(cfg.CONFIGURACAO["USUARIO"]);
      handler.GetElement("AUTENTICAR_WFMBUTTON", WAITSEC.Agora).Click();
      if (handler.GetElements("AUTENTICAR_WFMERROR1", WAITSEC.Curto).Any())
        throw new InvalidOperationException("O usuário informado está incorreto!");
      handler.GetElement("AUTENTICAR_WFMPASS", WAITSEC.Curto).SendKeys(cfg.CONFIGURACAO["PALAVRA"]);
      handler.GetElement("AUTENTICAR_WFMBUTTON", WAITSEC.Agora).Click();
      if (handler.GetElements("AUTENTICAR_WFMERROR2", WAITSEC.Curto).Any())
        throw new InvalidOperationException("A senha informada está incorreta!");
    }
    if (handler.GetElements("AUTENTICAR_WFMLIST", WAITSEC.Medio).Any())
    {
      handler.GetElement("AUTENTICAR_WFMLIST", WAITSEC.Agora).Click();
      handler.GetElement("AUTENTICAR_WFMPASS", WAITSEC.Curto).SendKeys(cfg.CONFIGURACAO["PALAVRA"]);
      handler.GetElement("AUTENTICAR_WFMBUTTON", WAITSEC.Agora).Click();
      if (handler.GetElements("AUTENTICAR_WFMERROR2", WAITSEC.Curto).Any())
        throw new InvalidOperationException("A senha informada está incorreta!");
    }
    if (handler.GetElements("AUTENTICAR_WFMCHECK", WAITSEC.Medio).Any())
    {
      handler.GetElement("AUTENTICAR_WFMCHECK", WAITSEC.Agora).Click();
      handler.GetElement("AUTENTICAR_WFMBUTTON", WAITSEC.Agora).Click();
    }
  }
}
