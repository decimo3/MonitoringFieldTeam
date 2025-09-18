using System.Text;

namespace Automation.Persistence
{
  public class GeneralInfo
  {
    public string Data { get; set; }
    public string NotaServico { get; set; }
    public string Recurso { get; set; }
    public string Atividade { get; set; }
    public string Servico { get; set; }
    public string Estado { get; set; }
    public string Descricao { get; set; }
    public string Observacao { get; set; }
    public override string ToString()
    {
      var builder = new StringBuilder();
      if(Data is not null) builder.Append($"Data: {Data}\n");
      if(Recurso is not null) builder.Append($"Recurso: {Recurso}\n");
      if(Atividade is not null) builder.Append($"Atividade: {Atividade}\n");
      if(Servico is not null) builder.Append($"Serviço: {Servico}\n");
      if(Descricao is not null) builder.Append($"Descrição: {Descricao}\n");
      if(Estado is not null) builder.Append($"Estado: {Estado}\n");
      if(Observacao is not null) builder.Append($"Observação: {Observacao}\n");
      return builder.ToString();
    }
  }
}
