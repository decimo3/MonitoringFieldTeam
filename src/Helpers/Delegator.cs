namespace MonitoringFieldTeam.Helpers;

public static class Delegator
{
  public static void Run()
  {
    // DONE - Get the list of orders
    Log.Information("Verificando a lista de notas...");
    var filepath = System.IO.Path.Combine(
      Configuration.GetString("DATAPATH"),
      "ofs.txt");
    if (!System.IO.File.Exists(filepath))
    {
      Log.Information("O arquivo de lista notas não foi encontrado!");
      return;
    }
    var orders = System.IO.File.ReadAllLines(filepath);
    // TODO - Get the list of workers
    // TODO - Check witch workers are on
    // TODO - Send orders to online workers
    // TODO - Store successful response on DB
    // TODO - Export the report in the end
  }
}
