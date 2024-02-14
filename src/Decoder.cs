using System.Text;
using System.Text.RegularExpressions;
using automation.schemas;

namespace automation;
public static class Decoder
{
  public static List<Recurso> Analisador(List<String> html_lista)
  {
    var lista_recursos = new List<Recurso>();
    foreach (var recurso_html in html_lista)
    {
      var recurso = new Recurso();
      var html_limpo = Limpador(recurso_html);
      var elementos = Estripador(html_limpo);
      lista_recursos.Add(recurso);
    }
    return lista_recursos;
  }
  private static String Limpador(String html)
  {
    var removido_tabulacoes = new Regex(@"\s{2,}").Replace(html, String.Empty);
    var removido_tokens = new Regex(@"div\s?").Replace(removido_tabulacoes, String.Empty);
    var substituido_hifens = new Regex(@"([a-z])-").Replace(removido_tokens, @"$1_");
    var removido_unidades = new Regex(@"([0-9])px").Replace(substituido_hifens, @"$1");
    var substituidos_simbolos = removido_unidades.Replace("&quot;", "\"");
    return substituidos_simbolos;
  }
  private static List<String> Estripador(String html)
  {
    var deep = 0;
    var startCaptureElement = false;
    var textCapturedElements = new List<String>();
    var tempTextCaptured = new StringBuilder();
    foreach (var character in html)
    {
      if (character == '<')
      {
        startCaptureElement = true;
        deep++;
        continue;
      }
      if (character == '>')
      {
        var elemento = tempTextCaptured.ToString();
        textCapturedElements.Add(elemento);
        startCaptureElement = false;
        deep--;
        continue;
      }
      if (character == '/')
      {
        deep--;
        continue;
      }
      if (startCaptureElement)
      {
        tempTextCaptured.Append(character);
        continue;
      }
    }
    return textCapturedElements;
  }
}