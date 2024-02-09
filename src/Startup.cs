namespace automation;
public class Startup
{
  public static void Main(string[] args)
  {
    var configuration = new Configuration();
    using var WebHandler = new Manager(configuration);
    WebHandler.Autenticar();
    WebHandler.Inicializar();
    while(true)
    {
      WebHandler.Atualizar();
      System.Threading.Thread.Sleep(Configuration.ESPERA_LONGA);
      Console.Read();
    }
  }
}