namespace MonitoringFieldTeam.Helpers;

public class ValueNotFoundException : Exception
{
  public ValueNotFoundException() { }
  public ValueNotFoundException(string message) : base(message) { }
  public ValueNotFoundException(string message, Exception innerException) : base(message, innerException) { }
  protected ValueNotFoundException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
public static class Configuration
{
  private static Dictionary<string, object> VALUES = new(StringComparer.InvariantCultureIgnoreCase);
  public static void LoadConf(string confpath)
  {
    var configurations = ArquivoConfiguracao(confpath);

    foreach (var configuration in configurations)
      VALUES[configuration.Key] = ParseValue(configuration.Value);
  }
  private static object GetObject(string key)
  {
    if (!VALUES.TryGetValue(key, out var obj) || obj is null)
      throw new KeyNotFoundException($"Não foi possível obter o valor pela chave {key}");
    return obj;
  }
  public static string GetString(string key)
  {
    var obj = GetObject(key);
    if (obj is string s) return s;
    if (obj is string[] a) return a[0];
    throw new ValueNotFoundException("O objeto de configuração não pode ser convertido para o tipo especificado!");
  }
  public static string[] GetArray(string key)
  {
    var obj = GetObject(key);
    if (obj is string[] a) return a;
    if (obj is string s) return new string[] { s };
    throw new ValueNotFoundException("O objeto de configuração não pode ser convertido para o tipo especificado!");
  }
  public static (string Key, long Value)[] GetPairs(string key)
  {
    var obj = GetObject(key);
    if (obj is (string Key, long Value)[] a) return a;
    if (obj is ValueTuple<string, long> p) return new[] { (p.Item1, p.Item2) };
    throw new ValueNotFoundException("O objeto de configuração não pode ser convertido para o tipo especificado!");
  }
  private static object ParseValue(string value)
  {
    // Simple: no comma → return string
    if (!value.Contains(',')) return value;
    var parts = value.Split(',');
    // Sub-array case: "a|1,b|2,c|3"
    if (parts.Length > 0 && parts[0].Contains('|'))
    {
      var list = new List<(string, long)>();
      foreach (var p in parts)
      {
        var pair = p.Split('|');
        if (pair.Length != 2)
          throw new FormatException($"Invalid pair format in: '{p}'.");
        list.Add((pair[0], long.Parse(pair[1])));
      }
      return list.ToArray(); // (string,long)[]
    }
    // Normal comma list: "a,b,c"
    return parts; // string[]
  }
  public static Dictionary<String, String> ArquivoConfiguracao(String filename, char delimiter = '=')
  {
    var parametros = new Dictionary<string,string>();
    var file = System.IO.File.ReadAllLines(filename);
    foreach (var line in file)
    {
      if (String.IsNullOrEmpty(line)) continue;
      if (line.TrimStart().StartsWith('#')) continue;
      var args = line.Split(delimiter, 2);
      if (args.Length < 2) continue;
      var key = args[0].Trim();
      var val = args[1].Trim();
      parametros[key] = val;
    }
    return parametros;
  }
}
