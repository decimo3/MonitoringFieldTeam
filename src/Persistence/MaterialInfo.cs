namespace Automation.Persistence
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
            return $"Nota: {Nota}\n" +
                   $"Tipo: {Tipo}\n" +
                   $"Codigo: {Codigo}\n" +
                   $"Serie: {Serie}\n" +
                   $"Descricao: {Descricao}\n" +
                   $"Quantidade: {Quantidade}\n" +
                   $"Origem: {Origem}";
        }
    }
}
