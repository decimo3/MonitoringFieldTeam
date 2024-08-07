namespace Automation.Helpers;
public static class Executor
{
  public static String Executar(String aplication, String arguments)
  {
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
      return stdoutput;
    }
  }
}
