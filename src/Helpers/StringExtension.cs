using System.Collections;

public static class StringExtension
{
  public static string RemoveLineEndings(this string texto)
  {
    return texto.ReplaceLineEndings().Replace(Environment.NewLine, " ");
  }
}
