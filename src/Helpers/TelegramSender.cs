using Serilog;
namespace MonitoringFieldTeam.Helpers;
public static class Telegram
{
  private static readonly String TOKEN = Configuration.GetString("BOT_TOKEN");
  private static long GetChannelId(String bucketName)
  {
    var pair = Configuration.GetPairs("BOT_CHANNEL").FirstOrDefault(c => c.Key == bucketName);
    if (pair.Key is null)
      throw new KeyNotFoundException($"Bucket '{bucketName}' was not found in BOT_CHANNELS.");
    return pair.Value;
  }
  public static void SendMessage(String bucketName, String mensagem)
  {
    var temp = String.Empty;
    try
    {
      if (String.IsNullOrEmpty(mensagem)) return;
      var baseurl = new Uri($"https://api.telegram.org/bot{TOKEN}/sendMessage");
      using(var client = new HttpClient())
      {
        var message_obj = new { chat_id = GetChannelId(bucketName), text = mensagem, parse_mode = "MarkdownV2" };
        var jsonContent = System.Text.Json.JsonSerializer.Serialize(message_obj);
        using(var msgContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"))
        {
          using(var request = new HttpRequestMessage(HttpMethod.Get, baseurl))
          {
            request.Content = msgContent;
            var response = client.Send(request);
            using(var stream = new StreamReader(response.Content.ReadAsStream()))
            {
              temp = stream.ReadToEnd();
              response.EnsureSuccessStatusCode();
              Log.Information("Mensagem enviada com sucesso!\n{mensagem}", mensagem);
            }
          }
        }
      }
    }
    catch (System.Exception erro)
    {
      Log.Error(erro.Message);
      if (!string.IsNullOrWhiteSpace(temp))
        Log.Error("Mensagem de erro retornada: {temp}", temp);
      if (erro.StackTrace is not null)
        Log.Error(erro.StackTrace);
    }
  }
  public static void SendDocument(String bucketName, String filepath)
  {
    var temp = String.Empty;
    var filename = Path.GetFileName(filepath);
    var channel = GetChannelId(bucketName);
    try
    {
      using var arquivo = new FileStream(filepath, FileMode.Open);
      if (arquivo.Length == 0) return;
      var baseurl = new Uri($"https://api.telegram.org/bot{TOKEN}/sendDocument?chat_id={channel}");
      var formulario = new MultipartFormDataContent();
      HttpContent content = new StreamContent(arquivo);
      formulario.Add(content, "document", filename);
      using(var client = new HttpClient())
      {
        using(var request = new HttpRequestMessage(HttpMethod.Post, baseurl))
        {
          request.Content = formulario;
          var response = client.Send(request);
          using(var stream = new StreamReader(response.Content.ReadAsStream()))
          {
            temp = stream.ReadToEnd();
            response.EnsureSuccessStatusCode();
            Log.Information("Documento {arquivo} enviado com sucesso!", filename);
          }
        }
      }
    }
    catch (System.Exception erro)
    {
      Log.Error(erro.Message);
      if (!string.IsNullOrWhiteSpace(temp))
        Log.Error("Mensagem de erro retornada: {temp}", temp);
      if (erro.StackTrace is not null)
        Log.Error(erro.StackTrace);
    }
  }
}
