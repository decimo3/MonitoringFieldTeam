namespace Automation;
public class Startup
{
  public static void Main(string[] args)
  {
    var configuration = new Configuration();
    using var WebHandler = new WebScraper.Manager(configuration);
    WebHandler.Autenticar();
    WebHandler.Inicializar();
    while(true)
    {
      WebHandler.Atualizar();
      WebHandler.Coletor();
      System.Threading.Thread.Sleep(configuration.ESPERA_TOTAL);
    }
  }
}