using Microsoft.Data.Sqlite;
using MonitoringFieldTeam.Persistence;

namespace MonitoringFieldTeam.Helpers;

public sealed class Database : IDisposable
{
  private static readonly string DNS = Configuration.GetString("DATABASE");
  private readonly SqliteConnection _conn;

  public Database()
  {
    _conn = new SqliteConnection(DNS);
    _conn.Open();
    CreateDatabaseScheme();
  }

  public void Dispose()
  {
    _conn?.Dispose();
  }

  public void CreateDatabaseScheme()
  {
    using var curr = _conn.CreateCommand();
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
    using var curr = _conn.CreateCommand();
    curr.CommandText = @"INSERT INTO general (data, notaservico, recurso,
            atividade, situacao, damage, vencimento, descricao, observacao)
            VALUES (@data, @nota, @recurso,
            @atividade, @situacao, @damage, @vencimento, @descricao, @observacao)";
    curr.Parameters.Clear();
    curr.Parameters.Add(new SqliteParameter("@data", generalInfo.Data));
    curr.Parameters.Add(new SqliteParameter("@nota", generalInfo.NotaServico));
    curr.Parameters.Add(new SqliteParameter("@recurso", generalInfo.Recurso));
    curr.Parameters.Add(new SqliteParameter("@atividade", generalInfo.Atividade));
    curr.Parameters.Add(new SqliteParameter("@situacao", generalInfo.Situacao));
    curr.Parameters.Add(new SqliteParameter("@damage", generalInfo.Damage));
    curr.Parameters.Add(new SqliteParameter("@vencimento", generalInfo.Vencimento));
    curr.Parameters.Add(new SqliteParameter("@descricao", generalInfo.Descricao));
    curr.Parameters.Add(new SqliteParameter("@observacao", generalInfo.Observacao));
    curr.ExecuteNonQuery();
  }

  public void AddFinalizaInfo(List<FinalizaInfo> finalizaInfos)
  {
    using var curr = _conn.CreateCommand();
    using var transaction = _conn.BeginTransaction();
    curr.Transaction = transaction;
    curr.CommandText = @"INSERT INTO finaliza (notaservico, codigo, quantidade) VALUES (@notaservico, @codigo, @quantidade)";
    foreach (var finalizaInfo in finalizaInfos)
    {
      curr.Parameters.Clear();
      curr.Parameters.Add(new SqliteParameter("@notaservico", finalizaInfo.NotaServico));
      curr.Parameters.Add(new SqliteParameter("@codigo", finalizaInfo.Codigo));
      curr.Parameters.Add(new SqliteParameter("@quantidade", finalizaInfo.Quantidade));
      curr.ExecuteNonQuery();
    }
    transaction.Commit();
  }

  public void AddMaterialInfo(List<MaterialInfo> materialInfos)
  {
    using var curr = _conn.CreateCommand();
    using var transaction = _conn.BeginTransaction();
    curr.Transaction = transaction;
    curr.CommandText = @"INSERT INTO material (nota, tipo, codigo, serie, descricao, quantidade, origem)
      VALUES (@nota, @tipo, @codigo, @serie, @descricao, @quantidade, @origem)";
    foreach (var materialInfo in materialInfos)
    {
      curr.Parameters.Clear();
      curr.Parameters.Add(new SqliteParameter("@nota", materialInfo.Nota));
      curr.Parameters.Add(new SqliteParameter("@tipo", materialInfo.Tipo));
      curr.Parameters.Add(new SqliteParameter("@codigo", materialInfo.Codigo));
      curr.Parameters.Add(new SqliteParameter("@serie", materialInfo.Serie));
      curr.Parameters.Add(new SqliteParameter("@descricao", materialInfo.Descricao));
      curr.Parameters.Add(new SqliteParameter("@quantidade", materialInfo.Quantidade));
      curr.Parameters.Add(new SqliteParameter("@origem", materialInfo.Origem));
      curr.ExecuteNonQuery();
    }
    transaction.Commit();
  }

  public void AddOcorrenciaInfo(OcorrenciaInfo ocorrenciaInfo)
  {
    using var curr = _conn.CreateCommand();
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
    curr.Parameters.Add(new SqliteParameter("@notaservico", ocorrenciaInfo.NotaServico));
    curr.Parameters.Add(new SqliteParameter("@caixatipo", ocorrenciaInfo.CaixaTipo));
    curr.Parameters.Add(new SqliteParameter("@caixamodelo", ocorrenciaInfo.CaixaModelo));
    curr.Parameters.Add(new SqliteParameter("@numerotoi", ocorrenciaInfo.NumeroToi));
    curr.Parameters.Add(new SqliteParameter("@nometitular", ocorrenciaInfo.NomeTitular));
    curr.Parameters.Add(new SqliteParameter("@documentotipo", ocorrenciaInfo.DocumentoTipo));
    curr.Parameters.Add(new SqliteParameter("@documentonum", ocorrenciaInfo.DocumentoNum));
    curr.Parameters.Add(new SqliteParameter("@residenciaclasse", ocorrenciaInfo.ResidenciaClasse));

    curr.Parameters.Add(new SqliteParameter("@motivoinspecao", ocorrenciaInfo.MotivoInspecao));
    curr.Parameters.Add(new SqliteParameter("@instalacaosuspensa", ocorrenciaInfo.InstalacaoSuspensa));
    curr.Parameters.Add(new SqliteParameter("@instalacaonormalizada", ocorrenciaInfo.InstalacaoNormalizada));
    curr.Parameters.Add(new SqliteParameter("@consumidoracompanhou", ocorrenciaInfo.ConsumidorAcompanhou));
    curr.Parameters.Add(new SqliteParameter("@clienteautorizoulevantamento", ocorrenciaInfo.ClienteAutorizouLevantamento));
    curr.Parameters.Add(new SqliteParameter("@clientesolicitoupericia", ocorrenciaInfo.ClienteSolicitouPericia));
    curr.Parameters.Add(new SqliteParameter("@clientequalassinou", ocorrenciaInfo.ClienteQualAssinou));
    curr.Parameters.Add(new SqliteParameter("@clienterecusouassinar", ocorrenciaInfo.ClienteRecusouAssinar));
    curr.Parameters.Add(new SqliteParameter("@clienterecusoureceber", ocorrenciaInfo.ClienteRecusouReceber));
    curr.Parameters.Add(new SqliteParameter("@fisicoentreguetoi", ocorrenciaInfo.FisicoEntregueTOI));
    curr.Parameters.Add(new SqliteParameter("@quantidadeevidencias", ocorrenciaInfo.QuantidadeEvidencias));
    curr.Parameters.Add(new SqliteParameter("@existenciaevidencias", ocorrenciaInfo.ExistenciaEvidencias));
    curr.Parameters.Add(new SqliteParameter("@descricaoirregularidade", ocorrenciaInfo.DescricaoIrregularidade));

    curr.Parameters.Add(new SqliteParameter("@grupotarifarico", ocorrenciaInfo.GrupoTarifarico));
    curr.Parameters.Add(new SqliteParameter("@ligacaotipo", ocorrenciaInfo.LigacaoTipo));
    curr.Parameters.Add(new SqliteParameter("@quantidadeelementos", ocorrenciaInfo.QuantidadeElementos));
    curr.Parameters.Add(new SqliteParameter("@fornecimentotipo", ocorrenciaInfo.FornecimentoTipo));
    curr.Parameters.Add(new SqliteParameter("@tensaotipo", ocorrenciaInfo.TensaoTipo));
    curr.Parameters.Add(new SqliteParameter("@tensaonivel", ocorrenciaInfo.TensaoNivel));
    curr.Parameters.Add(new SqliteParameter("@ramaltipo", ocorrenciaInfo.RamalTipo));
    curr.Parameters.Add(new SqliteParameter("@sistemaencapsulado", ocorrenciaInfo.SistemaEncapsulado));

    curr.Parameters.Add(new SqliteParameter("@medidortipo", ocorrenciaInfo.MedidorTipo));
    curr.Parameters.Add(new SqliteParameter("@medidornumero", ocorrenciaInfo.MedidorNumero));
    curr.Parameters.Add(new SqliteParameter("@medidormarca", ocorrenciaInfo.MedidorMarca));
    curr.Parameters.Add(new SqliteParameter("@medidorano", ocorrenciaInfo.MedidorAno));
    curr.Parameters.Add(new SqliteParameter("@medidorpatrimonio", ocorrenciaInfo.MedidorPatrimonio));
    curr.Parameters.Add(new SqliteParameter("@medidortensao", ocorrenciaInfo.MedidorTensao));
    curr.Parameters.Add(new SqliteParameter("@medidoranominal", ocorrenciaInfo.MedidorANominal));
    curr.Parameters.Add(new SqliteParameter("@medidoramaximo", ocorrenciaInfo.MedidorAMaximo));
    curr.Parameters.Add(new SqliteParameter("@medidorconstante", ocorrenciaInfo.MedidorConstante));
    curr.Parameters.Add(new SqliteParameter("@medidorlocalizacao", ocorrenciaInfo.MedidorLocalizacao));
    curr.Parameters.Add(new SqliteParameter("@medidorobservacao", ocorrenciaInfo.MedidorObservacao));

    curr.Parameters.Add(new SqliteParameter("@declarantenomecompleto", ocorrenciaInfo.DeclaranteNomeCompleto));
    curr.Parameters.Add(new SqliteParameter("@declarantegrauafiinidade", ocorrenciaInfo.DeclaranteGrauAfiinidade));
    curr.Parameters.Add(new SqliteParameter("@declarantedocumento", ocorrenciaInfo.DeclaranteDocumento));
    curr.Parameters.Add(new SqliteParameter("@declarantetempoocupacao", ocorrenciaInfo.DeclaranteTempoOcupacao));
    curr.Parameters.Add(new SqliteParameter("@declarantetempounidade", ocorrenciaInfo.DeclaranteTempoUnidade));
    curr.Parameters.Add(new SqliteParameter("@declarantetipoocupacao", ocorrenciaInfo.DeclaranteTipoOcupacao));
    curr.Parameters.Add(new SqliteParameter("@declaranteqntresidentes", ocorrenciaInfo.DeclaranteQntResidentes));
    curr.Parameters.Add(new SqliteParameter("@declaranteemail", ocorrenciaInfo.DeclaranteEmail));
    curr.Parameters.Add(new SqliteParameter("@declarantecelular", ocorrenciaInfo.DeclaranteCelular));

    curr.Parameters.Add(new SqliteParameter("@selagemtampos", ocorrenciaInfo.SelagemTampos));
    curr.Parameters.Add(new SqliteParameter("@selagembornes", ocorrenciaInfo.SelagemBornes));
    curr.Parameters.Add(new SqliteParameter("@selagemparafuso", ocorrenciaInfo.SelagemParafuso));
    curr.Parameters.Add(new SqliteParameter("@selagemtrava", ocorrenciaInfo.SelagemTrava));
    curr.Parameters.Add(new SqliteParameter("@selagemtampa", ocorrenciaInfo.SelagemTampa));
    curr.Parameters.Add(new SqliteParameter("@selagembase", ocorrenciaInfo.SelagemBase));
    curr.Parameters.Add(new SqliteParameter("@selagemgeral", ocorrenciaInfo.SelagemGeral));

    curr.ExecuteNonQuery();
  }
}
