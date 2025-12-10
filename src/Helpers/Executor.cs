using Serilog;
namespace MonitoringFieldTeam.Helpers;

public static class Executor
{
  public static String Executar(String aplication, String arguments)
  {
    Log.Debug("{executable} {arguments}", aplication, arguments);
    using(var process = new System.Diagnostics.Process())
    {
      process.StartInfo.FileName = aplication;
      process.StartInfo.Arguments = arguments;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;
      process.StartInfo.CreateNoWindow = true;
      process.Start();
      var stdoutput = process.StandardOutput.ReadToEnd();
      var erroutput = process.StandardError.ReadToEnd();
      process.WaitForExit();
      if(process.ExitCode != 0) throw new InvalidOperationException($"Erro ao executar o processo {process.ExitCode}: {erroutput}");
      Log.Debug(stdoutput);
      return stdoutput;
    }
  }
  public static void Reiniciar()
  {
    var executable = System.Environment.ProcessPath ??
      throw new InvalidOperationException("O caminho do processo não pode ser encontrado!");
    var arguments = System.Environment.GetCommandLineArgs();
    Log.Information("Iniciando o novo processo...");
    System.Diagnostics.Process.Start(executable, String.Join(' ', arguments.Skip(1).ToArray()));
    Log.Information("Encerrando processo atual...");
    System.Environment.Exit(0);
  }
}
