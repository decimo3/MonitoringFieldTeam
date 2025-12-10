using System.Text;
namespace MonitoringFieldTeam.Persistence;

public class FeedBack
{
  public String recurso { get; set; }
  public String aviso { get; set; }
  public Int32? tempo { get; set; }
  public FeedBack
  (
    String recurso,
    String aviso,
    Int32? tempo
  )
  {
    this.recurso = recurso;
    this.aviso = aviso;
    this.tempo = tempo;
  }
  public override string ToString()
  {
      var relatorios = new StringBuilder();
      relatorios.Append(recurso);
      relatorios.Append($" {aviso}\\!");
      if(tempo != null)
        relatorios.Append($" \\~{tempo}min");
      relatorios.Append('\n');
      return relatorios.ToString();
  }
}
