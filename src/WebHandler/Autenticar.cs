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
    var matricula = Configuration.GetString("MATRICULA");
    Log.Information("Autenticando usuário...");
    Log.Information("Verificando se o usuário foi desconectado...");
    if (handler.GetElements("AUTENTICAR_SIGNOUT", WAITSEC.Medio).Any()) // FIXME
    {
      handler.ReloadWebPage();
    }
    Log.Information("Verificando a tela de login do OFS...");
    if (handler.GetElements("AUTENTICAR_OFSAUTH", WAITSEC.Medio).Any())
    {
      Log.Information("Autenticando no portal de login do OFS...");
      handler.GetElement("AUTENTICAR_OFSUSER", WAITSEC.Agora).SendKeys(usuario);
      handler.GetElement("AUTENTICAR_OFSPASS", WAITSEC.Agora).SendKeys(palavra);
      handler.GetElement("AUTENTICAR_OFSLOGIN", WAITSEC.Agora).Click();
      if(handler.GetElements("AUTENTICAR_OFSERROR", WAITSEC.Curto).FirstOrDefault()?.Displayed ?? false)
          throw new InvalidOperationException("A credencial informada está incorreta!");
      Log.Information("Autenticação no portal login do OFS bem sucedida!");
    }
    Log.Information("Verificando a tela de login do cliente...");
    if (handler.GetElements("AUTENTICAR_WFMAUTH", WAITSEC.Medio).Any())
    {
      Log.Information("Autenticando no portal de login do cliente...");
      handler.GetElement("AUTENTICAR_WFMUSER", WAITSEC.Curto).SendKeys(matricula);
      handler.GetElement("AUTENTICAR_WFMPASS", WAITSEC.Curto).SendKeys(palavra);
      handler.GetElement("AUTENTICAR_WFMBUTTON", WAITSEC.Agora).Click();
      if (handler.GetElements("AUTENTICAR_WFMERROR", WAITSEC.Curto).FirstOrDefault()?.Displayed ?? false)
          throw new InvalidOperationException("A credencial informada está incorreta!");
      Log.Information("Autenticação no portal login do cliente bem sucedida!");
    }
    Log.Information("Verificando a tela de consentimento de dados...");
    if (handler.GetElements("CONFEDERACAO_CONSENT_PAGE", WAITSEC.Medio).Any())
    {
      handler.GetElement("CONFEDERACAO_CONSENT_AGREE", WAITSEC.Curto).Click();
      Log.Information("Consentimento de dados permitido!");
    }
    Log.Information("Autenticação realizada!");
  }
}
