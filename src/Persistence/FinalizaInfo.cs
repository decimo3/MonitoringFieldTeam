using System.Text;

namespace Automation.Persistence
{
  public class FinalizaInfo
  {
    public string NotaServico { get; set; }
    public string Codigo { get; set; }
    public string Quantidade { get; set; }
    public override string ToString()
    {
      var builder = new StringBuilder();
      if(NotaServico is not null) builder.Append($"Serviço: {NotaServico}\n");
      if(Codigo is not null) builder.Append($"Código: {Codigo}\n");
      if(Quantidade is not null) builder.Append($"Quantidade: {Quantidade}\n");
      return builder.ToString();
    }
  }
}
