using Npgsql;
using MonitoringFieldTeam.Persistence;

namespace MonitoringFieldTeam.Helpers;

public sealed class Database : IDisposable
{
  private static readonly string DNS = Configuration.GetString("DATABASE");
  private readonly System.Data.IDbConnection _conn;

  public Database()
  {
    _conn = new NpgsqlConnection(DNS);
    _conn.Open();
  }

  public void Dispose()
  {
    _conn?.Dispose();
  }

  public void AddGeneralInfo(GeneralInfo generalInfo)
  {
    using var curr = _conn.CreateCommand();
    curr.CommandText = @"INSERT INTO general (data, notaservico, recurso,
            atividade, situacao, damage, vencimento, descricao, observacao)
            VALUES (@data, @nota, @recurso,
            @atividade, @situacao, @damage, @vencimento, @descricao, @observacao)";
    curr.Parameters.Clear();
    curr.Parameters.Add(new NpgsqlParameter("@data", generalInfo.Data));
    curr.Parameters.Add(new NpgsqlParameter("@nota", generalInfo.NotaServico));
    curr.Parameters.Add(new NpgsqlParameter("@recurso", generalInfo.Recurso));
    curr.Parameters.Add(new NpgsqlParameter("@atividade", generalInfo.Atividade));
    curr.Parameters.Add(new NpgsqlParameter("@situacao", generalInfo.Situacao));
    curr.Parameters.Add(new NpgsqlParameter("@damage", generalInfo.Damage));
    curr.Parameters.Add(new NpgsqlParameter("@vencimento", generalInfo.Vencimento));
    curr.Parameters.Add(new NpgsqlParameter("@descricao", generalInfo.Descricao));
    curr.Parameters.Add(new NpgsqlParameter("@observacao", generalInfo.Observacao));
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
      curr.Parameters.Add(new NpgsqlParameter("@notaservico", finalizaInfo.NotaServico));
      curr.Parameters.Add(new NpgsqlParameter("@codigo", finalizaInfo.Codigo));
      curr.Parameters.Add(new NpgsqlParameter("@quantidade", finalizaInfo.Quantidade));
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
      curr.Parameters.Add(new NpgsqlParameter("@nota", materialInfo.Nota));
      curr.Parameters.Add(new NpgsqlParameter("@tipo", materialInfo.Tipo));
      curr.Parameters.Add(new NpgsqlParameter("@codigo", materialInfo.Codigo));
      curr.Parameters.Add(new NpgsqlParameter("@serie", materialInfo.Serie));
      curr.Parameters.Add(new NpgsqlParameter("@descricao", materialInfo.Descricao));
      curr.Parameters.Add(new NpgsqlParameter("@quantidade", materialInfo.Quantidade));
      curr.Parameters.Add(new NpgsqlParameter("@origem", materialInfo.Origem));
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
    curr.Parameters.Add(new NpgsqlParameter("@notaservico", ocorrenciaInfo.NotaServico));
    curr.Parameters.Add(new NpgsqlParameter("@caixatipo", ocorrenciaInfo.CaixaTipo));
    curr.Parameters.Add(new NpgsqlParameter("@caixamodelo", ocorrenciaInfo.CaixaModelo));
    curr.Parameters.Add(new NpgsqlParameter("@numerotoi", ocorrenciaInfo.NumeroToi));
    curr.Parameters.Add(new NpgsqlParameter("@nometitular", ocorrenciaInfo.NomeTitular));
    curr.Parameters.Add(new NpgsqlParameter("@documentotipo", ocorrenciaInfo.DocumentoTipo));
    curr.Parameters.Add(new NpgsqlParameter("@documentonum", ocorrenciaInfo.DocumentoNum));
    curr.Parameters.Add(new NpgsqlParameter("@residenciaclasse", ocorrenciaInfo.ResidenciaClasse));

    curr.Parameters.Add(new NpgsqlParameter("@motivoinspecao", ocorrenciaInfo.MotivoInspecao));
    curr.Parameters.Add(new NpgsqlParameter("@instalacaosuspensa", ocorrenciaInfo.InstalacaoSuspensa));
    curr.Parameters.Add(new NpgsqlParameter("@instalacaonormalizada", ocorrenciaInfo.InstalacaoNormalizada));
    curr.Parameters.Add(new NpgsqlParameter("@consumidoracompanhou", ocorrenciaInfo.ConsumidorAcompanhou));
    curr.Parameters.Add(new NpgsqlParameter("@clienteautorizoulevantamento", ocorrenciaInfo.ClienteAutorizouLevantamento));
    curr.Parameters.Add(new NpgsqlParameter("@clientesolicitoupericia", ocorrenciaInfo.ClienteSolicitouPericia));
    curr.Parameters.Add(new NpgsqlParameter("@clientequalassinou", ocorrenciaInfo.ClienteQualAssinou));
    curr.Parameters.Add(new NpgsqlParameter("@clienterecusouassinar", ocorrenciaInfo.ClienteRecusouAssinar));
    curr.Parameters.Add(new NpgsqlParameter("@clienterecusoureceber", ocorrenciaInfo.ClienteRecusouReceber));
    curr.Parameters.Add(new NpgsqlParameter("@fisicoentreguetoi", ocorrenciaInfo.FisicoEntregueTOI));
    curr.Parameters.Add(new NpgsqlParameter("@quantidadeevidencias", ocorrenciaInfo.QuantidadeEvidencias));
    curr.Parameters.Add(new NpgsqlParameter("@existenciaevidencias", ocorrenciaInfo.ExistenciaEvidencias));
    curr.Parameters.Add(new NpgsqlParameter("@descricaoirregularidade", ocorrenciaInfo.DescricaoIrregularidade));

    curr.Parameters.Add(new NpgsqlParameter("@grupotarifarico", ocorrenciaInfo.GrupoTarifarico));
    curr.Parameters.Add(new NpgsqlParameter("@ligacaotipo", ocorrenciaInfo.LigacaoTipo));
    curr.Parameters.Add(new NpgsqlParameter("@quantidadeelementos", ocorrenciaInfo.QuantidadeElementos));
    curr.Parameters.Add(new NpgsqlParameter("@fornecimentotipo", ocorrenciaInfo.FornecimentoTipo));
    curr.Parameters.Add(new NpgsqlParameter("@tensaotipo", ocorrenciaInfo.TensaoTipo));
    curr.Parameters.Add(new NpgsqlParameter("@tensaonivel", ocorrenciaInfo.TensaoNivel));
    curr.Parameters.Add(new NpgsqlParameter("@ramaltipo", ocorrenciaInfo.RamalTipo));
    curr.Parameters.Add(new NpgsqlParameter("@sistemaencapsulado", ocorrenciaInfo.SistemaEncapsulado));

    curr.Parameters.Add(new NpgsqlParameter("@medidortipo", ocorrenciaInfo.MedidorTipo));
    curr.Parameters.Add(new NpgsqlParameter("@medidornumero", ocorrenciaInfo.MedidorNumero));
    curr.Parameters.Add(new NpgsqlParameter("@medidormarca", ocorrenciaInfo.MedidorMarca));
    curr.Parameters.Add(new NpgsqlParameter("@medidorano", ocorrenciaInfo.MedidorAno));
    curr.Parameters.Add(new NpgsqlParameter("@medidorpatrimonio", ocorrenciaInfo.MedidorPatrimonio));
    curr.Parameters.Add(new NpgsqlParameter("@medidortensao", ocorrenciaInfo.MedidorTensao));
    curr.Parameters.Add(new NpgsqlParameter("@medidoranominal", ocorrenciaInfo.MedidorANominal));
    curr.Parameters.Add(new NpgsqlParameter("@medidoramaximo", ocorrenciaInfo.MedidorAMaximo));
    curr.Parameters.Add(new NpgsqlParameter("@medidorconstante", ocorrenciaInfo.MedidorConstante));
    curr.Parameters.Add(new NpgsqlParameter("@medidorlocalizacao", ocorrenciaInfo.MedidorLocalizacao));
    curr.Parameters.Add(new NpgsqlParameter("@medidorobservacao", ocorrenciaInfo.MedidorObservacao));

    curr.Parameters.Add(new NpgsqlParameter("@declarantenomecompleto", ocorrenciaInfo.DeclaranteNomeCompleto));
    curr.Parameters.Add(new NpgsqlParameter("@declarantegrauafiinidade", ocorrenciaInfo.DeclaranteGrauAfiinidade));
    curr.Parameters.Add(new NpgsqlParameter("@declarantedocumento", ocorrenciaInfo.DeclaranteDocumento));
    curr.Parameters.Add(new NpgsqlParameter("@declarantetempoocupacao", ocorrenciaInfo.DeclaranteTempoOcupacao));
    curr.Parameters.Add(new NpgsqlParameter("@declarantetempounidade", ocorrenciaInfo.DeclaranteTempoUnidade));
    curr.Parameters.Add(new NpgsqlParameter("@declarantetipoocupacao", ocorrenciaInfo.DeclaranteTipoOcupacao));
    curr.Parameters.Add(new NpgsqlParameter("@declaranteqntresidentes", ocorrenciaInfo.DeclaranteQntResidentes));
    curr.Parameters.Add(new NpgsqlParameter("@declaranteemail", ocorrenciaInfo.DeclaranteEmail));
    curr.Parameters.Add(new NpgsqlParameter("@declarantecelular", ocorrenciaInfo.DeclaranteCelular));

    curr.Parameters.Add(new NpgsqlParameter("@selagemtampos", ocorrenciaInfo.SelagemTampos));
    curr.Parameters.Add(new NpgsqlParameter("@selagembornes", ocorrenciaInfo.SelagemBornes));
    curr.Parameters.Add(new NpgsqlParameter("@selagemparafuso", ocorrenciaInfo.SelagemParafuso));
    curr.Parameters.Add(new NpgsqlParameter("@selagemtrava", ocorrenciaInfo.SelagemTrava));
    curr.Parameters.Add(new NpgsqlParameter("@selagemtampa", ocorrenciaInfo.SelagemTampa));
    curr.Parameters.Add(new NpgsqlParameter("@selagembase", ocorrenciaInfo.SelagemBase));
    curr.Parameters.Add(new NpgsqlParameter("@selagemgeral", ocorrenciaInfo.SelagemGeral));

    curr.ExecuteNonQuery();
  }

  public void AddOrderList(List<OrderInfo> orders)
  {
    using var curr = _conn.CreateCommand();
    using var transaction = _conn.BeginTransaction();
    curr.Transaction = transaction;
    curr.CommandText = @"INSERT INTO ordenacao (order_number, activity_id, status_code, created_at, updated_at, observation) VALUES (@order_number, @activity_id, @status_code, @created_at, @updated_at, @observation)";
    foreach (var order in orders)
    {
      curr.Parameters.Clear();
      curr.Parameters.Add(new NpgsqlParameter("@order_number", order.OrderNumber));
      curr.Parameters.Add(new NpgsqlParameter("@activity_id", order.ActivityId));
      curr.Parameters.Add(new NpgsqlParameter("@status_code", order.StatusCode));
      curr.Parameters.Add(new NpgsqlParameter("@created_at", order.CreatedAt));
      curr.Parameters.Add(new NpgsqlParameter("@updated_at", order.UpdatedAt));
      curr.Parameters.Add(new NpgsqlParameter("@observation", order.Observation ?? (object)DBNull.Value));
      curr.ExecuteNonQuery();
    }
    transaction.Commit();
  }

  public List<OrderInfo> GetOrderList()
  {
    var orders = new List<OrderInfo>();
    using var curr = _conn.CreateCommand();
    curr.CommandText = @"SELECT identifier, order_number, activity_id, status_code, created_at, updated_at, observation FROM ordenacao";
    using var reader = curr.ExecuteReader();
    while (reader.Read())
    {
      orders.Add(new OrderInfo
      {
        Identifier = reader.GetInt64(0),
        OrderNumber = reader.GetInt64(1),
        ActivityId = reader.IsDBNull(2) ? 0 : reader.GetInt64(2),
        StatusCode = reader.GetInt32(3),
        CreatedAt = reader.GetDateTime(4),
        UpdatedAt = reader.GetDateTime(5),
        Observation = reader.IsDBNull(6) ? null : reader.GetString(6)
      });
    }
    return orders;
  }

  public void PutOrderInfo(OrderInfo item)
  {
    using var curr = _conn.CreateCommand();
    curr.CommandText = @"UPDATE ordenacao SET status_code = @status_code, updated_at = @updated_at, observation = @observation WHERE identifier = @identifier";
    curr.Parameters.Clear();
    curr.Parameters.Add(new NpgsqlParameter("@identifier", item.Identifier));
    curr.Parameters.Add(new NpgsqlParameter("@status_code", item.StatusCode));
    curr.Parameters.Add(new NpgsqlParameter("@updated_at", item.UpdatedAt));
    curr.Parameters.Add(new NpgsqlParameter("@observation", item.Observation ?? (object)DBNull.Value));
    curr.ExecuteNonQuery();
  }
}
