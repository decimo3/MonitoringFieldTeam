using automation.schemas;

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
      var rotas = WebHandler.Coletar();
      Decoder.Analisador(rotas);
      Console.Read();
    }
  }
}