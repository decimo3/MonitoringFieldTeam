namespace automation;
public class Startup
{
  public static void Main(string[] args)
  {
    var configuration = new Configuration();
    using var program = new Program(configuration);
    program.Autenticar();
    program.Inicializar();
    while(true)
    {
      program.Atualizar();
      System.Threading.Thread.Sleep(Configuration.ESPERA_LONGA);
      Console.Read();
    }
  }
}