using MonitoringFieldTeam.Persistence;
using MonitoringFieldTeam.Service;

public class ListObjectsToCsvTest
{
  [Fact]
  public void TestCsvGeneration()
  {
    var data = new List<FinalizaInfo>
    {
      new FinalizaInfo { NotaServico = "1234567890", Codigo = "12.3", Quantidade = "1" },
      new FinalizaInfo { NotaServico = "1234567890", Codigo = "45.6", Quantidade = "1" },
      new FinalizaInfo { NotaServico = "1234567890", Codigo = "78.9", Quantidade = "1" },
      new FinalizaInfo { NotaServico = "1234567890", Codigo = "AB.C", Quantidade = "1" },
      new FinalizaInfo { NotaServico = "1234567890", Codigo = "DE.F", Quantidade = "1" },
      new FinalizaInfo { NotaServico = "1234567890", Codigo = "GH.I", Quantidade = "1" }
    };
    var csv = Finalizador.ListObjectsToCSV(data);
    Assert.Equal(@"NotaServico,Codigo,Quantidade
1234567890,12.3,1
1234567890,45.6,1
1234567890,78.9,1
1234567890,AB.C,1
1234567890,DE.F,1
1234567890,GH.I,1
", csv);
  }
}
