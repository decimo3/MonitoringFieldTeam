using OpenQA.Selenium;
namespace Automation.WebScraper
{
  public partial class Manager
  {
    public Boolean Solicitacoes()
    {
      try
      {
        var erroMensagem = "A solicitação enviada é inválida, verifique e envie a aplicação correta";
        if(!System.IO.File.Exists(cfg.LOCKFILE)) return false;
        var solicitacao = System.IO.File.ReadAllText(cfg.LOCKFILE, System.Text.Encoding.UTF8);
        if(solicitacao.Length == 0 || solicitacao.Length > 50) return false;
        Console.WriteLine($"{DateTime.Now} - Solicitação recebida: {solicitacao}.");
        var args = solicitacao.Split(' ');
        if(args.Length != 2)
        {
          System.IO.File.WriteAllText(cfg.LOCKFILE, erroMensagem, System.Text.Encoding.UTF8);
          Console.WriteLine($"{DateTime.Now} - {erroMensagem}.");
          return false;
        }
        var aplicacao = args[0];
        var informacao = args[1];
        switch (aplicacao)
        {
          case "evidencia":
            var resposta = GetServico(informacao);
            System.IO.File.WriteAllText(cfg.LOCKFILE, resposta, System.Text.Encoding.UTF8);
            Console.WriteLine($"{DateTime.Now} - {resposta}.");
            return true;
          default:
            System.IO.File.WriteAllText(cfg.LOCKFILE, erroMensagem, System.Text.Encoding.UTF8);
            Console.WriteLine($"{DateTime.Now} - {erroMensagem}.");
            return false;
        }
      }
      catch (System.Exception erro)
      {
        System.IO.File.WriteAllText(cfg.LOCKFILE, erro.Message, System.Text.Encoding.UTF8);
        Console.WriteLine($"{DateTime.Now} - {erro.Message}.");
        return true;
      }
    }
  }
}