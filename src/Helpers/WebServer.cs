using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using MonitoringFieldTeam.Persistence;
using MonitoringFieldTeam.WebScraper;

namespace MonitoringFieldTeam.Helpers;

public static class WebServer
{
  private static object _lock = new();
  public static void Run(WebHandler.WebHandler handler)
  {
    var builder = WebApplication.CreateBuilder();
    builder.WebHost.UseUrls("http://localhost:7826");
    var app = builder.Build();
    app.MapGet("/", (HttpContext context) =>
    {
      using var reader = new StreamReader(context.Request.Body);
      var requestInfo = JsonSerializer.Deserialize<RequestInfo>(reader.ReadToEnd());
      if (requestInfo is null)
        return Results.Ok();
      lock (_lock)
      {
        try
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
          /* TODO - convert from List<String> to List<Uri>
          if (requestInfo.info.Contains("JPG"))
            responseInfo.UploadsInfo = workHandler.GetActivityUploads(false);
          if (requestInfo.info.Contains("EVD"))
            responseInfo.EvidenceInfo = workHandler.GetActivityUploads(true);
          */
          return Results.Json(responseInfo);
        }
        catch (System.Exception erro)
        {
          return Results.Text(erro.Message, statusCode: 400);
        }
      }
    });
    app.Run();
  }
}
