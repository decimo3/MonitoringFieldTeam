using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using MonitoringFieldTeam.Persistence;
using MonitoringFieldTeam.WebScraper;

namespace MonitoringFieldTeam.Helpers;

public static class WebServer
{
  private static Stream ZipFiles(List<String> files)
  {
    var memoryStream = new MemoryStream();
    using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);
    foreach (var file in files)
    {
      var entry = archive.CreateEntry(Path.GetFileName(file));
      using var entryStream = entry.Open();
      using var fileStream = File.OpenRead(file);
      fileStream.CopyTo(entryStream);
    }
    memoryStream.Position = 0;
    return memoryStream;
  }
  private static object _lock = new();
  public static void Run(WebHandler.WebHandler handler)
  {
    var builder = WebApplication.CreateBuilder();
    builder.WebHost.UseUrls("http://localhost:7826");
    var app = builder.Build();
    app.MapGet("/", (string info, long nota) =>
    {
      lock (_lock)
      {
        try
        {
          var workHandler = new ServicoHandler(handler, nota);
          workHandler.SearchAndEnterActivity();
          switch (info.ToLower())
          {
            case "informacao":
              var generalInfo = workHandler.GetActivityGeneralInfo();
              return Results.Json<GeneralInfo>(generalInfo);
            case "finalizacao":
              var activityClosings = workHandler.GetActivityClosings();
              return Results.Json<List<FinalizaInfo>>(activityClosings);
            case "material":
              var materialInfo = workHandler.GetActivityMaterials();
              return Results.Json<List<MaterialInfo>>(materialInfo);
            case "inspecao":
              var inspecaoInfo = workHandler.GetActivityOcorrencias();
              return Results.Json<OcorrenciaInfo>(inspecaoInfo);
            case "fotografia":
              {
                var uploadsInfo = workHandler.GetActivityUploads(false);
                using var zipedfile = WebServer.ZipFiles(uploadsInfo);
                return Results.File(zipedfile);
              }
            case "evidencia":
              {
                var evidenceInfo = workHandler.GetActivityUploads(true);
                using var zipedfile = WebServer.ZipFiles(evidenceInfo);
                return Results.File(zipedfile);
              }
            // TODO - case "relatorios":
            // TODO - case "retroachieve":
          }
          throw new InvalidOperationException(
            $"A operação `{info}` é inválida!");
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
