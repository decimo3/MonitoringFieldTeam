using Serilog;
using MonitoringFieldTeam.Helpers;
using MonitoringFieldTeam.WebHandler;
namespace MonitoringFieldTeam.WebScraper;

public static class Autenticador
{
  public static void Autenticar(WebHandler.WebHandler handler)
  {
    var usuario = Configuration.GetString("USUARIO");
    var palavra = Configuration.GetString("PALAVRA");
    Log.Information("Autenticando usuário...");
    if (handler.GetElements("AUTENTICAR_SIGNOUT", WAITSEC.Medio).Any()) // FIXME
    {
      handler.ReloadWebPage();
    }
    if (handler.GetElements("AUTENTICAR_OFSAUTH", WAITSEC.Medio).Any())
    {
      handler.GetElement("AUTENTICAR_OFSUSER", WAITSEC.Agora).SendKeys(usuario);
      handler.GetElement("AUTENTICAR_OFSPASS", WAITSEC.Agora).SendKeys(palavra);
      handler.GetElement("AUTENTICAR_OFSLOGIN", WAITSEC.Agora).Click();
    }
    if (handler.GetElements("AUTENTICAR_WFMAUTH", WAITSEC.Medio).Any())
    {
      handler.GetElement("AUTENTICAR_WFMUSER", WAITSEC.Curto).SendKeys(usuario);
      handler.GetElement("AUTENTICAR_WFMBUTTON", WAITSEC.Total).Click();
      if (handler.GetElements("AUTENTICAR_WFMERROR1", WAITSEC.Curto).Any())
        throw new InvalidOperationException("O usuário informado está incorreto!");
      handler.GetElement("AUTENTICAR_WFMPASS", WAITSEC.Curto).SendKeys(palavra);
      handler.GetElement("AUTENTICAR_WFMBUTTON", WAITSEC.Agora).Click();
      if (handler.GetElements("AUTENTICAR_WFMERROR2", WAITSEC.Curto).Any())
        throw new InvalidOperationException("A senha informada está incorreta!");
    }
    if (handler.GetElements("AUTENTICAR_WFMLIST", WAITSEC.Medio).Any())
    {
      handler.GetElement("AUTENTICAR_WFMLIST", WAITSEC.Agora).Click();
      handler.GetElement("AUTENTICAR_WFMPASS", WAITSEC.Curto).SendKeys(palavra);
      handler.GetElement("AUTENTICAR_WFMBUTTON", WAITSEC.Agora).Click();
      if (handler.GetElements("AUTENTICAR_WFMERROR2", WAITSEC.Curto).Any())
        throw new InvalidOperationException("A senha informada está incorreta!");
    }
    if (handler.GetElements("AUTENTICAR_WFMCHECK", WAITSEC.Medio).Any())
    {
      handler.GetElement("AUTENTICAR_WFMCHECK", WAITSEC.Agora).Click();
      handler.GetElement("AUTENTICAR_WFMBUTTON", WAITSEC.Agora).Click();
    }
    Log.Information("Autenticação realizada!");
  }
}
