using System.Net.Http.Json;
using System.Text.Json;
using MonitoringFieldTeam.Persistence;
using Serilog;
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
    // DONE - Send orders to online workers
    var tasks = new List<Task>();
    var semaphore = new SemaphoreSlim(online_workers.Length);
    var extracao = Configuration.GetArray("EXTRACAO");
    for(var i = 0; i < orders.Length; i++)
    {
      semaphore.Wait();
      int instanceNumber = i % online_workers.Length;
      tasks.Add(
        Task.Run(
          async () =>
          {
            try
            {
              var worker = online_workers[instanceNumber];
              if (!long.TryParse(orders[i], out long nota))
                throw new InvalidOperationException(
                  $"Há caracteres inválidos na nota {orders[i]}!");
              var requestInfo = new RequestInfo(extracao, nota);
              var request = new HttpRequestMessage()
              {
                Method = HttpMethod.Get,
                RequestUri = new System.Uri(worker),
                Content = new StringContent(
                  JsonSerializer.Serialize(requestInfo))
              };
              var response = client.Send(request);
              response.EnsureSuccessStatusCode();
              var responseInfo = await response.Content.ReadFromJsonAsync<ResponseInfo>();
              if (responseInfo is null)
              {
                var responseText = await response.Content.ReadAsStringAsync();
                Log.Debug($"Response text: {responseText}");
                throw new InvalidOperationException(
                  $"Houve um erro no formato da resposta!");
              }
            }
            catch (Exception erro)
            {
              Log.Error(erro.Message);
            }
            finally
            {
              semaphore.Release();
            }
          }
        )
      );
    }
    // TODO - Store successful response on DB
    // TODO - Export the report in the end
  }
}
