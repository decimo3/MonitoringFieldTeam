using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;
using CsvHelper;
using MonitoringFieldTeam.Persistence;
using Serilog;
namespace MonitoringFieldTeam.Helpers;

public static class Delegator
{
  private static readonly List<string> CODIGOS_DE_RAMAL_OU_MEDIDOR = new()
    {"18.0", "6.11", "6.15", "6.16", "6.43", "7.10", "7.11", "7.12", "7.15", "7.16", "7.17"};
  public static void Run()
  {
    // DONE - Get the list of orders
    Log.Information("Procurando relatórios do OFS...");
    var files = System.IO.Directory.GetFiles(
      Configuration.GetString("DATAPATH"))
        .Where(f => System.IO.Path.GetExtension(f) == ".csv" &&
          System.IO.Path.GetFileName(f).StartsWith("Atividade"))
        .ToArray();
    if (files.Length == 0)
    {
      Log.Error("Não foram encontrados relatórios do OFS!");
      return;
    }
    foreach (var filepath in files)
    {
    string[] orders = Array.Empty<string>();
    Log.Information("Relatório atual {rel}", filepath);
    if (System.IO.Path.GetFileName(filepath) == "ofs.txt")
    {
      orders = System.IO.File.ReadAllLines(filepath);
    }
    if (System.IO.Path.GetExtension(filepath) == ".csv" &&
      System.IO.Path.GetFileName(filepath).StartsWith("Atividades"))
    {
    using (var reader = new StreamReader(filepath))
    {
      using (var csv = new CsvReader(reader,
        System.Globalization.CultureInfo.InvariantCulture))
      {
        var records = csv.GetRecords<dynamic>();
        orders = records
        .Select(r =>
        {
          var dict = (IDictionary<string, object>)r;
          return dict.ToDictionary(
              kv => kv.Key,
              kv => kv.Value?.ToString()
          );
        })
        .Where(r =>
        {
          if (r["Status da Atividade"] != "concluído") return false;
          if (string.IsNullOrWhiteSpace(r["Ordem de Serviço"])) return false;
          var codigos = (r["Códs. de Fechamento"] + r["Motivo de Rejeição"]).Split(';');
          return codigos.Any(c => CODIGOS_DE_RAMAL_OU_MEDIDOR.Contains(c));
        })
        .Select(r => r["Ordem de Serviço"]!).ToArray();
      }
    }
    }
    if (orders.Length == 0)
    {
      Log.Error("Não foram encontradas notas para extração no arquivo {file}!", filepath);
      continue;
    }
    Log.Information("{qtd} ordens de serviço para extração.", orders.Length);
    // DONE - Get the list of workers
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
      var worker = online_workers[instanceNumber];
      tasks.Add(
        Task.Run(
          async () =>
          {
            try
            {
              if (!long.TryParse(order, out long nota))
                throw new InvalidOperationException(
                  $"Há caracteres inválidos na nota {order}!");
              Log.Information("Nota: {nota}, Worker: {worker}", nota, worker);
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
              var responseText = await response.Content.ReadAsStringAsync();
              Log.Information("Nota {nota} respondida pelo worker {worker}", nota, worker);
              Log.Debug("Response text: {responseText}", responseText);
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
    Log.Information("Finalizado reatório {report}", filepath);
    // DONE - Export the report in the end
    if (retry_orders.Count != 0)
    {
      System.IO.File.AppendAllText(
        System.IO.Path.Combine(
          Configuration.GetString("DATAPATH"),
          "ofs.txt"
        ), string.Join('\n', retry_orders) + '\n');
      Log.Warning("Arquivo {file} regravado com {count} ordens para nova tentativa!", filepath, retry_orders.Count);
    }
    System.IO.File.Delete(filepath);
    }
  }
}
