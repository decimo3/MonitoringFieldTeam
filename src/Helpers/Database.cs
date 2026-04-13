using System.Data.SQLite;
using MonitoringFieldTeam.Persistence;

namespace MonitoringFieldTeam.Helpers;

public sealed class Database : IDisposable
{
  private static readonly string DNS = Configuration.GetString("DATAPATH");
  private readonly SQLiteConnection _conn;

  public Database()
  {
    _conn = new SQLiteConnection(DNS);
    _conn.Open();
    CreateDatabaseScheme();
  }

  public void Dispose()
  {
    _conn?.Dispose();
  }

  public void CreateDatabaseScheme()
  {
    var datapath = Configuration.GetString("DATAPATH");
    if (System.IO.File.Exists(datapath)) return;
    using var curr = new SQLiteCommand(_conn);
    var filepath = System.IO.Path.Combine(
      System.AppContext.BaseDirectory,
      "database.sql");
    if (!System.IO.File.Exists(filepath))
      throw new FileNotFoundException(
        "O arquivo `database.sql` não foi encontrado!");
    curr.CommandText = System.IO.File.ReadAllText(filepath);
    curr.ExecuteNonQuery();
  }

  public void AddGeneralInfo(GeneralInfo generalInfo)
  {
    using var curr = new SQLiteCommand(_conn);
    curr.CommandText = @"INSERT INTO general (data, notaservico, recurso,
            atividade, situacao, damage, vencimento, descricao, observacao)
            VALUES (@data, @nota, @recurso,
            @atividade, @situacao, @damage, @vencimento, @descricao, @observacao)";
    curr.Parameters.Clear();
    curr.Parameters.Add(new SQLiteParameter("@data", generalInfo.Data));
    curr.Parameters.Add(new SQLiteParameter("@nota", generalInfo.NotaServico));
    curr.Parameters.Add(new SQLiteParameter("@recurso", generalInfo.Recurso));
    curr.Parameters.Add(new SQLiteParameter("@atividade", generalInfo.Atividade));
    curr.Parameters.Add(new SQLiteParameter("@situacao", generalInfo.Situacao));
    curr.Parameters.Add(new SQLiteParameter("@damage", generalInfo.Damage));
    curr.Parameters.Add(new SQLiteParameter("@vencimento", generalInfo.Vencimento));
    curr.Parameters.Add(new SQLiteParameter("@descricao", generalInfo.Descricao));
    curr.Parameters.Add(new SQLiteParameter("@observacao", generalInfo.Observacao));
    curr.ExecuteNonQuery();
  }

  public void AddFinalizaInfo(List<FinalizaInfo> finalizaInfos)
  {
    using var curr = new SQLiteCommand(_conn);
    curr.CommandText = @"INSERT INTO finaliza (notaservico, codigo, quantidade) VALUES (@notaservico, @codigo, @quantidade)";
    curr.Parameters.Clear();
    foreach (var finalizaInfo in finalizaInfos)
    {
      curr.Parameters.Add(new SQLiteParameter("@notaservico", finalizaInfo.NotaServico));
      curr.Parameters.Add(new SQLiteParameter("@codigo", finalizaInfo.Codigo));
      curr.Parameters.Add(new SQLiteParameter("@quantidade", finalizaInfo.Quantidade));
    }
    curr.ExecuteNonQuery();
  }

  public void AddMaterialInfo(List<MaterialInfo> materialInfos)
  {
    using var curr = new SQLiteCommand(_conn);
    curr.CommandText = @"INSERT INTO material (nota, tipo, codigo, serie, descricao, quantidade, origem)
      VALUES (@nota, @tipo, @codigo, @serie, @descricao, @quantidade, @origem)";
    curr.Parameters.Clear();
    foreach (var materialInfo in materialInfos)
    {
      curr.Parameters.Add(new SQLiteParameter("@nota", materialInfo.Nota));
      curr.Parameters.Add(new SQLiteParameter("@tipo", materialInfo.Tipo));
      curr.Parameters.Add(new SQLiteParameter("@codigo", materialInfo.Codigo));
      curr.Parameters.Add(new SQLiteParameter("@serie", materialInfo.Serie));
      curr.Parameters.Add(new SQLiteParameter("@descricao", materialInfo.Descricao));
      curr.Parameters.Add(new SQLiteParameter("@quantidade", materialInfo.Quantidade));
      curr.Parameters.Add(new SQLiteParameter("@origem", materialInfo.Origem));
    }
    curr.ExecuteNonQuery();
  }

  public void AddOcorrenciaInfo(OcorrenciaInfo ocorrenciaInfo)
  {
    using var curr = new SQLiteCommand(_conn);
    curr.CommandText = @"INSERT INTO ocorrencia (
      notaservico, caixatipo, caixamodelo, numerotoi, nometitular, documentotipo, documentonum, residenciaclasse,
      motivoinspecao, instalacaosuspensa, instalacaonormalizada, consumidoracompanhou, clienteautorizoulevantamento,
      clientesolicitoupericia, clientequalassinou, clienterecusouassinar, clienterecusoureceber, fisicoentreguetoi,
      quantidadeevidencias, existenciaevidencias, descricaoirregularidade,
      grupotarifarico, ligacaotipo, quantidadeelementos, fornecimentotipo, tensaotipo, tensaonivel, ramaltipo, sistemaencapsulado,
      medidortipo, medidornumero, medidormarca, medidorano, medidorpatrimonio, medidortensao, medidoranominal, medidoramaximo,
      medidorconstante, medidorlocalizacao, medidorobservacao,
      declarantenomecompleto, declarantegrauafiinidade, declarantedocumento, declarantetempoocupacao, declarantetempounidade,
      declarantetipoocupacao, declaranteqntresidentes, declaranteemail, declarantecelular,
      selagemtampos, selagembornes, selagemparafuso, selagemtrava, selagemtampa, selagembase, selagemgeral
    ) VALUES (
      @notaservico, @caixatipo, @caixamodelo, @numerotoi, @nometitular, @documentotipo, @documentonum, @residenciaclasse,
      @motivoinspecao, @instalacaosuspensa, @instalacaonormalizada, @consumidoracompanhou, @clienteautorizoulevantamento,
      @clientesolicitoupericia, @clientequalassinou, @clienterecusouassinar, @clienterecusoureceber, @fisicoentreguetoi,
      @quantidadeevidencias, @existenciaevidencias, @descricaoirregularidade,
      @grupotarifarico, @ligacaotipo, @quantidadeelementos, @fornecimentotipo, @tensaotipo, @tensaonivel, @ramaltipo, @sistemaencapsulado,
      @medidortipo, @medidornumero, @medidormarca, @medidorano, @medidorpatrimonio, @medidortensao, @medidoranominal, @medidoramaximo,
      @medidorconstante, @medidorlocalizacao, @medidorobservacao,
      @declarantenomecompleto, @declarantegrauafiinidade, @declarantedocumento, @declarantetempoocupacao, @declarantetempounidade,
      @declarantetipoocupacao, @declaranteqntresidentes, @declaranteemail, @declarantecelular,
      @selagemtampos, @selagembornes, @selagemparafuso, @selagemtrava, @selagemtampa, @selagembase, @selagemgeral
    )";
    curr.Parameters.Clear();
    curr.Parameters.Add(new SQLiteParameter("@notaservico", ocorrenciaInfo.NotaServico));
    curr.Parameters.Add(new SQLiteParameter("@caixatipo", ocorrenciaInfo.CaixaTipo));
    curr.Parameters.Add(new SQLiteParameter("@caixamodelo", ocorrenciaInfo.CaixaModelo));
    curr.Parameters.Add(new SQLiteParameter("@numerotoi", ocorrenciaInfo.NumeroToi));
    curr.Parameters.Add(new SQLiteParameter("@nometitular", ocorrenciaInfo.NomeTitular));
    curr.Parameters.Add(new SQLiteParameter("@documentotipo", ocorrenciaInfo.DocumentoTipo));
    curr.Parameters.Add(new SQLiteParameter("@documentonum", ocorrenciaInfo.DocumentoNum));
    curr.Parameters.Add(new SQLiteParameter("@residenciaclasse", ocorrenciaInfo.ResidenciaClasse));

    curr.Parameters.Add(new SQLiteParameter("@motivoinspecao", ocorrenciaInfo.MotivoInspecao));
    curr.Parameters.Add(new SQLiteParameter("@instalacaosuspensa", ocorrenciaInfo.InstalacaoSuspensa));
    curr.Parameters.Add(new SQLiteParameter("@instalacaonormalizada", ocorrenciaInfo.InstalacaoNormalizada));
    curr.Parameters.Add(new SQLiteParameter("@consumidoracompanhou", ocorrenciaInfo.ConsumidorAcompanhou));
    curr.Parameters.Add(new SQLiteParameter("@clienteautorizoulevantamento", ocorrenciaInfo.ClienteAutorizouLevantamento));
    curr.Parameters.Add(new SQLiteParameter("@clientesolicitoupericia", ocorrenciaInfo.ClienteSolicitouPericia));
    curr.Parameters.Add(new SQLiteParameter("@clientequalassinou", ocorrenciaInfo.ClienteQualAssinou));
    curr.Parameters.Add(new SQLiteParameter("@clienterecusouassinar", ocorrenciaInfo.ClienteRecusouAssinar));
    curr.Parameters.Add(new SQLiteParameter("@clienterecusoureceber", ocorrenciaInfo.ClienteRecusouReceber));
    curr.Parameters.Add(new SQLiteParameter("@fisicoentreguetoi", ocorrenciaInfo.FisicoEntregueTOI));
    curr.Parameters.Add(new SQLiteParameter("@quantidadeevidencias", ocorrenciaInfo.QuantidadeEvidencias));
    curr.Parameters.Add(new SQLiteParameter("@existenciaevidencias", ocorrenciaInfo.ExistenciaEvidencias));
    curr.Parameters.Add(new SQLiteParameter("@descricaoirregularidade", ocorrenciaInfo.DescricaoIrregularidade));

    curr.Parameters.Add(new SQLiteParameter("@grupotarifarico", ocorrenciaInfo.GrupoTarifarico));
    curr.Parameters.Add(new SQLiteParameter("@ligacaotipo", ocorrenciaInfo.LigacaoTipo));
    curr.Parameters.Add(new SQLiteParameter("@quantidadeelementos", ocorrenciaInfo.QuantidadeElementos));
    curr.Parameters.Add(new SQLiteParameter("@fornecimentotipo", ocorrenciaInfo.FornecimentoTipo));
    curr.Parameters.Add(new SQLiteParameter("@tensaotipo", ocorrenciaInfo.TensaoTipo));
    curr.Parameters.Add(new SQLiteParameter("@tensaonivel", ocorrenciaInfo.TensaoNivel));
    curr.Parameters.Add(new SQLiteParameter("@ramaltipo", ocorrenciaInfo.RamalTipo));
    curr.Parameters.Add(new SQLiteParameter("@sistemaencapsulado", ocorrenciaInfo.SistemaEncapsulado));

    curr.Parameters.Add(new SQLiteParameter("@medidortipo", ocorrenciaInfo.MedidorTipo));
    curr.Parameters.Add(new SQLiteParameter("@medidornumero", ocorrenciaInfo.MedidorNumero));
    curr.Parameters.Add(new SQLiteParameter("@medidormarca", ocorrenciaInfo.MedidorMarca));
    curr.Parameters.Add(new SQLiteParameter("@medidorano", ocorrenciaInfo.MedidorAno));
    curr.Parameters.Add(new SQLiteParameter("@medidorpatrimonio", ocorrenciaInfo.MedidorPatrimonio));
    curr.Parameters.Add(new SQLiteParameter("@medidortensao", ocorrenciaInfo.MedidorTensao));
    curr.Parameters.Add(new SQLiteParameter("@medidoranominal", ocorrenciaInfo.MedidorANominal));
    curr.Parameters.Add(new SQLiteParameter("@medidoramaximo", ocorrenciaInfo.MedidorAMaximo));
    curr.Parameters.Add(new SQLiteParameter("@medidorconstante", ocorrenciaInfo.MedidorConstante));
    curr.Parameters.Add(new SQLiteParameter("@medidorlocalizacao", ocorrenciaInfo.MedidorLocalizacao));
    curr.Parameters.Add(new SQLiteParameter("@medidorobservacao", ocorrenciaInfo.MedidorObservacao));

    curr.Parameters.Add(new SQLiteParameter("@declarantenomecompleto", ocorrenciaInfo.DeclaranteNomeCompleto));
    curr.Parameters.Add(new SQLiteParameter("@declarantegrauafiinidade", ocorrenciaInfo.DeclaranteGrauAfiinidade));
    curr.Parameters.Add(new SQLiteParameter("@declarantedocumento", ocorrenciaInfo.DeclaranteDocumento));
    curr.Parameters.Add(new SQLiteParameter("@declarantetempoocupacao", ocorrenciaInfo.DeclaranteTempoOcupacao));
    curr.Parameters.Add(new SQLiteParameter("@declarantetempounidade", ocorrenciaInfo.DeclaranteTempoUnidade));
    curr.Parameters.Add(new SQLiteParameter("@declarantetipoocupacao", ocorrenciaInfo.DeclaranteTipoOcupacao));
    curr.Parameters.Add(new SQLiteParameter("@declaranteqntresidentes", ocorrenciaInfo.DeclaranteQntResidentes));
    curr.Parameters.Add(new SQLiteParameter("@declaranteemail", ocorrenciaInfo.DeclaranteEmail));
    curr.Parameters.Add(new SQLiteParameter("@declarantecelular", ocorrenciaInfo.DeclaranteCelular));

    curr.Parameters.Add(new SQLiteParameter("@selagemtampos", ocorrenciaInfo.SelagemTampos));
    curr.Parameters.Add(new SQLiteParameter("@selagembornes", ocorrenciaInfo.SelagemBornes));
    curr.Parameters.Add(new SQLiteParameter("@selagemparafuso", ocorrenciaInfo.SelagemParafuso));
    curr.Parameters.Add(new SQLiteParameter("@selagemtrava", ocorrenciaInfo.SelagemTrava));
    curr.Parameters.Add(new SQLiteParameter("@selagemtampa", ocorrenciaInfo.SelagemTampa));
    curr.Parameters.Add(new SQLiteParameter("@selagembase", ocorrenciaInfo.SelagemBase));
    curr.Parameters.Add(new SQLiteParameter("@selagemgeral", ocorrenciaInfo.SelagemGeral));

    curr.ExecuteNonQuery();
  }
}
