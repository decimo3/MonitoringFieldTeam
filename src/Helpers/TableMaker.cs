namespace Automation.Helpers;
public static class TableMaker<T>
{
  public static readonly char separador = ';';
  public static String Serialize(List<T> relatorios)
  {
    if (relatorios == null || relatorios.Count == 0)
      throw new ArgumentException("The list of reports cannot be null or empty.");
    var table = new System.Text.StringBuilder();
    var cabecalhos = Cabecalho(relatorios.First());
    table.Append(String.Join(separador, cabecalhos.Keys.ToList()));
    table.Append('\n');
    foreach(var relatorio in relatorios)
    {
      table.Append(String.Join(separador, Rodape(relatorio, cabecalhos)));
      table.Append('\n');
    }
    return table.ToString();
  }
  public static Dictionary<String, Int32> Cabecalho(T obj)
  {
    if (obj == null)
      throw new ArgumentException("The report cannot be null.");
    var contador = 0;
    Type tipo = obj.GetType();
    var cabecalhos = new Dictionary<String, Int32>();
    var atributos = tipo.GetProperties();
    foreach(var atributo in atributos)
    {
      cabecalhos.Add(atributo.Name, contador);
      contador++;
    }
    return cabecalhos;
  }
  public static List<String> Rodape(T obj, Dictionary<String,Int32> keys)
  {
    if (obj == null)
      throw new ArgumentException("The report cannot be null.");
    Type tipo = obj.GetType();
    var rodape = new List<String>(new string[keys.Count]);
    var atributos = tipo.GetProperties();
    foreach(var atributo in atributos)
    {
      var index = keys[atributo.Name];
      var value = atributo.GetValue(obj);
      if(value != null)
      {
        rodape[index] = value.ToString() ?? String.Empty;
      }
    }
    return rodape;
  }
}