namespace Automation.Helpers;
public static class Telegram
{
  public static void SendMessage(Int64 channel, String mensagem)
  {
    var temp = String.Empty;
    try
    {
      if(String.IsNullOrEmpty(mensagem)) return;
      var token = System.Environment.GetEnvironmentVariable("BOT_TOKEN") ??
        throw new InvalidOperationException("Environment variable BOT_TOKEN is not set!");
      var baseurl = new Uri($"https://api.telegram.org/bot{token}/sendMessage");
      using(var client = new HttpClient())
      {
        var message_obj = new { chat_id = channel, text = mensagem, parse_mode = "MarkdownV2" };
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
            }
          }
        }
      }
    }
    catch (System.Exception erro)
    {
      Console.Write($"{DateTime.Now} - {erro.Message}: {temp}");
      Console.Write($"{DateTime.Now} - {erro.StackTrace}");
    }
  }
  public static void sendDocument(Int64 channel, Stream arquivo, String filename)
  {
    var temp = String.Empty;
    try
    {
      if(arquivo.Length == 0) return;
      var token = System.Environment.GetEnvironmentVariable("BOT_TOKEN") ??
        throw new InvalidOperationException("Environment variable BOT_TOKEN is not set!");
      var baseurl = new Uri($"https://api.telegram.org/bot{token}/sendDocument?chat_id={channel}");
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
          }
        }
      }
    }
    catch (System.Exception erro)
    {
      Console.Write($"{DateTime.Now} - {erro.Message}: {temp}");
      Console.Write($"{DateTime.Now} - {erro.StackTrace}");
    }
  }
}
