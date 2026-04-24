using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MonitoringFieldTeam.Persistence;
using MonitoringFieldTeam.WebScraper;

namespace MonitoringFieldTeam.Helpers;

public sealed class WebServer : IDisposable
{
  private readonly string url;
  private WebApplication? app = null;
  private readonly WebHandler.WebHandler handler;
  private object _lock = new();
  private readonly string ROOT = Configuration.GetString("DATAPATH");
  public string BuildFileUrl(HttpRequest request, string fullPath)
  {
    if (!fullPath.StartsWith(ROOT, StringComparison.OrdinalIgnoreCase))
      throw new Exception("O arquivo exportado foi salvo fora da pasta compartilhada!");
    var relativePath = Path.GetRelativePath(ROOT, fullPath).Replace('\\', '/');
    return $"{request.Scheme}://{request.Host}/{relativePath}";
  }
  public WebServer
  (
    WebHandler.WebHandler handler,
    string url = "http://*:7826"
  )
  {
    this.handler = handler;
    this.url = url;
  }
  public void Run()
  {
    // DONE - Implement multi-port instance
    var builder = WebApplication.CreateBuilder();
    builder.WebHost.UseUrls(url);
    app = builder.Build();
    app.UseStaticFiles(new StaticFileOptions
    {
      RequestPath = "",
      FileProvider = new PhysicalFileProvider(ROOT)
    });
    app.MapGet("/", async (HttpContext context) =>
    {
      try
      {
        using var reader = new StreamReader(context.Request.Body);
        var payload = await reader.ReadToEndAsync();
        if (string.IsNullOrEmpty(payload)) return Results.Ok();
        var requestInfo = JsonSerializer.Deserialize<RequestInfo>(payload);
        if (requestInfo is null)
          return Results.Ok();
        lock (_lock)
        {
          var responseInfo = new ResponseInfo();
          var workHandler = new ServicoHandler(handler, requestInfo.nota);
          workHandler.SearchAndEnterActivity();
          if (requestInfo.info.Contains("INF"))
            responseInfo.GeneralInfo = workHandler.GetActivityGeneralInfo();
          if (requestInfo.info.Contains("COD"))
            responseInfo.FinalizaInfo = workHandler.GetActivityClosings();
          if (requestInfo.info.Contains("MAT"))
            responseInfo.MaterialInfo = workHandler.GetActivityMaterials();
          if (requestInfo.info.Contains("TOI"))
            responseInfo.OcorrenciaInfo = workHandler.GetActivityOcorrencias();
          // DONE - convert from local file path to remote resource path
          if (requestInfo.info.Contains("JPG"))
            responseInfo.UploadsInfo = workHandler.GetActivityUploads(true)
              .Select(p => BuildFileUrl(context.Request, p)).ToList();
          if (requestInfo.info.Contains("EVD"))
            responseInfo.EvidenceInfo = workHandler.GetActivityUploads(false)
              .Select(p => BuildFileUrl(context.Request, p)).ToList();
          return Results.Json(responseInfo);
        }
      }
      catch (System.Exception erro)
      {
        return Results.Text(erro.Message, statusCode: 400);
      }
    });
    MonitoringFieldTeam.WebScraper.Parametrizador.VerificarPagina(handler);
    app.Run();
  }
  public void Dispose()
  {
    Task.Run(() => app?.DisposeAsync()).GetAwaiter().GetResult();
  }
}
