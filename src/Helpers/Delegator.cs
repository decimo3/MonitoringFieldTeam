using System.Net.Http.Json;
using System.Text.Json;
using CsvHelper;
using MonitoringFieldTeam.Persistence;
using Serilog;
namespace MonitoringFieldTeam.Helpers;

public static class Delegator
{
  private static readonly int ESTIMATED_TIME_PER_ORDER = 20;

  private static List<OrderInfo> GetOrdersFromFile(string filepath)
  {
    if (System.IO.Path.GetFileName(filepath) == "ofs.txt")
    {
      return System.IO.File.ReadAllLines(filepath)
        .Where(line => long.TryParse(line, out _))
        .Select(l => new OrderInfo
        {
          ActivityId = 0,
          OrderNumber = long.Parse(l)
        })
        .ToList();
    }
    if (System.IO.Path.GetExtension(filepath) == ".csv" &&
      System.IO.Path.GetFileName(filepath).StartsWith("Atividades"))
    {
      using var reader = new StreamReader(filepath);
      using var csv = new CsvReader(reader,
        System.Globalization.CultureInfo.InvariantCulture);
      var records = csv.GetRecords<dynamic>();
      return records
        .Select(r =>
        {
          var dict = (IDictionary<string, object>)r;
          return dict.ToDictionary(
              kv => kv.Key,
              kv => kv.Value?.ToString()
          );
        })
        .Where(r =>
          (r["Status da Atividade"] == "concluído") &&
          !string.IsNullOrWhiteSpace(r["Ordem de Serviço"]) &&
          long.TryParse(r["Ordem de Serviço"], out _)
        )
        .Select(r => new OrderInfo
        {
          ActivityId = long.Parse(r["ID da Atividade"]!),
          OrderNumber = long.Parse(r["Ordem de Serviço"]!)
        })
        .ToList();
    }
    Log.Error("Não foram encontradas notas para extração no arquivo {file}!", filepath);
    return new List<OrderInfo>();
  }

  private static void AddOrdersFromFile()
  {
    using var database = new Database();
    Log.Information("Procurando arquivos de notas...");
    var files = System.IO.Directory.GetFiles(Configuration.GetString("DATAPATH"));
    if (files.Length == 0)
    {
      Log.Error("Não foram encontrados arquivos na pasta designada!");
      return;
    }
    foreach (var filepath in files)
    {
      Log.Information("Relatório atual {rel}", filepath);
      var orders = GetOrdersFromFile(filepath);
      if (orders.Count == 0)
        Log.Error("Não foram encontradas notas para extração no arquivo {file}!", filepath);
      else
        database.AddOrderList(orders);
      System.IO.File.Delete(filepath);
    }
  }

  public static List<OrderInfo> GetOrdersFromBase()
  {
    using var database = new Database();
    var orders = database.GetOrderList()
      .Where(x => x.StatusCode != 200).ToList();
    if (orders.Count == 0)
    {
      throw new InvalidOperationException(
        "Não foram encontradas notas para extração na base de dados!");
    }
    Log.Information("{qtd} ordens de serviço para extração.", orders.Count);
    return orders;
  }

  public static string[] GetOnlineWorkers()
  {
    Log.Information("Obtendo as lista de servidores...");
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
      throw new InvalidOperationException(
        "Não foram encontrado workers online!");
    }
    Log.Information("{qtd} workers online!", online_workers.Length);
    return online_workers;
  }

  public static void Run()
  {
    AddOrdersFromFile();
    // DONE - Get the list of orders
    var orders = GetOrdersFromBase();
    // DONE - Get the list of workers
    var online_workers = GetOnlineWorkers();
    var estimate_time = TimeSpan.FromSeconds(orders.Count * ESTIMATED_TIME_PER_ORDER / online_workers.Length);
    Log.Information("Tempo estimado para processamento: {estimate_time}.", estimate_time);
    // DONE - Send orders to online workers
    var tasks = new List<Task>();
    var semaphore = new SemaphoreSlim(online_workers.Length);
    var extracao = Configuration.GetArray("EXTRACAO");
    using var database = new Database();
    using var client = new HttpClient();
    for (var i = 0; i < orders.Count; i++)
    {
      semaphore.Wait();
      var order = orders[i];
      int instanceNumber = i % online_workers.Length;
      var worker = online_workers[instanceNumber];
      tasks.Add(
        Task.Run(
          async () =>
          {
            var response = new HttpResponseMessage();
            try
            {
              Log.Information("Nota: {nota}, Worker: {worker}", order.OrderNumber, worker);
              var requestInfo = new RequestInfo(extracao, order.OrderNumber);
              var request = new HttpRequestMessage()
              {
                Method = HttpMethod.Get,
                RequestUri = new System.Uri(worker),
                Content = new StringContent(
                  JsonSerializer.Serialize(requestInfo))
              };
              response = await client.SendAsync(request);
              var responseText = await response.Content.ReadAsStringAsync();
              Log.Information("Nota {nota} respondida pelo worker {worker}", order.OrderNumber, worker);
              if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException(responseText);
              var responseInfo = await response.Content.ReadFromJsonAsync<ResponseInfo>() ??
                throw new InvalidOperationException("Houve um erro no formato da resposta!");
              // DONE - Store successful response on DB
              if (responseInfo.GeneralInfo is not null)
                database.AddGeneralInfo(responseInfo.GeneralInfo);
              if (responseInfo.FinalizaInfo is not null)
                database.AddFinalizaInfo(responseInfo.FinalizaInfo);
              if (responseInfo.MaterialInfo is not null)
                database.AddMaterialInfo(responseInfo.MaterialInfo);
              if (responseInfo.OcorrenciaInfo is not null)
                database.AddOcorrenciaInfo(responseInfo.OcorrenciaInfo);
              order.Observation = $"Processada informações {string.Join(", ", extracao)} com sucesso!";
            }
            catch (TaskCanceledException erro)
            {
              response.StatusCode = System.Net.HttpStatusCode.RequestTimeout;
              order.Observation = erro.Message;
              Log.Error("O worker {worker} ficou offline durante a requisição da nota {nota}!", worker, order);
            }
            catch (Exception erro)
            {
              response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
              order.Observation = erro.Message;
              Log.Error("Aconteceu um erro na nota {nota} no worker {worker}!\nERRO: {erro}.",
                order, worker, erro.Message);
            }
            finally
            {
              order.StatusCode = (int)response.StatusCode;
              order.UpdatedAt = DateTime.Now;
              database.PutOrderInfo(order);
              semaphore.Release();
            }
          }
        )
      );
    }
    Task.WhenAll(tasks).GetAwaiter().GetResult();
  }
}
