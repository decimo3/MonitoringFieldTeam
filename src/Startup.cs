namespace automation;
public class Startup
{
  public static void Main(string[] args)
  {
    var configuration = new Configuration();
    using var program = new Program(configuration);
    program.Autenticar();
    program.Inicializar();
    Console.Read();
  }
}