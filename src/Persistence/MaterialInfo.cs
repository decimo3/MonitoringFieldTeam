using System.Text;

namespace MonitoringFieldTeam.Persistence
{
    public class MaterialInfo
    {
        public string Nota { get; set; }
        public string Tipo { get; set; }
        public string Codigo { get; set; }
        public string Serie { get; set; }
        public string Descricao { get; set; }
        public string Quantidade { get; set; }
        public string Origem { get; set; }
        public override string ToString()
        {
        var builder = new StringBuilder();
        if (Nota is not null) builder.Append($"Nota: {Nota}\n");
        if (Tipo is not null) builder.Append($"Tipo: {Tipo}\n");
        if (Codigo is not null) builder.Append($"Codigo: {Codigo}\n");
        if (Serie is not null) builder.Append($"Serie: {Serie}\n");
        if (Descricao is not null) builder.Append($"Descrição: {Descricao}\n");
        if (Quantidade is not null) builder.Append($"Quantidade: {Quantidade}\n");
        if (Origem is not null) builder.Append($"Origem: {Origem}\n");
        return builder.ToString();
      }
  }
}
