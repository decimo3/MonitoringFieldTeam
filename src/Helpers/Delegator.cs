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
    // DONE - Get the list of workers
    var workers = Configuration.GetArray("WEBSITE");
    // DONE - Check witch workers are on
    using var client = new HttpClient();
    var online_workers = Array.Empty<string>();
    foreach (var worker in workers)
    {
      try
      {
        if (string.IsNullOrEmpty(worker)) continue;
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new System.Uri(worker);
        var response = client.Send(request);
        response.EnsureSuccessStatusCode();
        online_workers = online_workers.Append(worker).ToArray();
      }
      catch (HttpRequestException)
      {
        Log.Error("O worker {worker} está offline!", worker);
      }
    }
    if (online_workers.Length == 0)
    {
      Log.Error("Não foram encontrado workers online!");
      return;
    }
    Log.Information("{qtd} workers online!", online_workers.Length);
    // TODO - Send orders to online workers
    // TODO - Store successful response on DB
    // TODO - Export the report in the end
  }
}
