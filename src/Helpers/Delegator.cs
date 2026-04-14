using System.Collections.Concurrent;
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
    var workers = Configuration.GetArray("WORKERS");
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
    using var database = new Database();
    var retry_orders = new ConcurrentBag<string>();
    var semaphore = new SemaphoreSlim(online_workers.Length);
    var extracao = Configuration.GetArray("EXTRACAO");
    for(var i = 0; i < orders.Length; i++)
    {
      semaphore.Wait();
      var order = orders[i];
      int instanceNumber = i % online_workers.Length;
      tasks.Add(
        Task.Run(
          async () =>
          {
            try
            {
              var worker = online_workers[instanceNumber];
              if (!long.TryParse(order, out long nota))
                throw new InvalidOperationException(
                  $"Há caracteres inválidos na nota {order}!");
              var requestInfo = new RequestInfo(extracao, nota);
              var request = new HttpRequestMessage()
              {
                Method = HttpMethod.Get,
                RequestUri = new System.Uri(worker),
                Content = new StringContent(
                  JsonSerializer.Serialize(requestInfo))
              };
              var response = await client.SendAsync(request);
              response.EnsureSuccessStatusCode();
              var responseInfo = await response.Content.ReadFromJsonAsync<ResponseInfo>();
              if (responseInfo is null)
              {
                var responseText = await response.Content.ReadAsStringAsync();
                Log.Debug($"Response text: {responseText}");
                throw new InvalidOperationException(
                  $"Houve um erro no formato da resposta!");
              }
              // DONE - Store successful response on DB
              if (responseInfo.GeneralInfo is not null)
                database.AddGeneralInfo(responseInfo.GeneralInfo);
              if (responseInfo.FinalizaInfo is not null)
                database.AddFinalizaInfo(responseInfo.FinalizaInfo);
              if (responseInfo.MaterialInfo is not null)
                database.AddMaterialInfo(responseInfo.MaterialInfo);
              if (responseInfo.OcorrenciaInfo is not null)
                database.AddOcorrenciaInfo(responseInfo.OcorrenciaInfo);
            }
            catch (Exception erro)
            {
              retry_orders.Add(order);
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
    Task.WhenAll(tasks).GetAwaiter().GetResult();
    // DONE - Export the report in the end
    if (retry_orders.Count != 0)
    {
      System.IO.File.WriteAllText(filepath, string.Join('\n', retry_orders));
      return;
    }
    System.IO.File.Delete(filepath);
  }
}
