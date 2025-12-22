using Serilog;
using MonitoringFieldTeam.Persistence;

namespace MonitoringFieldTeam.WebScraper
{
  public class ServicoHandler
  {
    private readonly WebHandler.WebHandler handler;
    private readonly long servico;
    public ServicoHandler
    (
      WebHandler.WebHandler handler,
      long servico
    )
    {
      this.handler = handler;
      this.servico = servico;
    }
    private void BackToBlack()
    {
      var backbtn = handler.GetElement("GLOBAL_BACKBTN", WebHandler.WAITSEC.Agora);
      if (!handler.IsElementCovered(backbtn))
      {
        backbtn.Click();
        return;
      }
      Thread.Sleep(TimeSpan.FromSeconds((int)WebHandler.WAITSEC.Curto));
      BackToBlack();
    }
    private void IsFinished()
    {
      var finished = handler.GetElement("ACTIVITY_SITUACAO", WebHandler.WAITSEC.Curto);
      if (!handler.IsElementCovered(finished))
      {
        if (finished.Text.Contains("concluído")) return;
        throw new InvalidOperationException($"A nota de serviço {servico} não está finalizada!");
      }
      Thread.Sleep(TimeSpan.FromSeconds((int)WebHandler.WAITSEC.Curto));
      IsFinished();
    }
    public void SearchAndEnterActivity()
    {
      Log.Information("Pesquisando nota {nota}", servico);
      try
      {
        // Click on search bar to focus cursor on
        handler.GetElement("SEARCHBAR_INPUT", WebHandler.WAITSEC.Total).Click();
      }
      catch (Exception)
      {
        // Second try to interact with search bar
        handler.GetElement("SEARCHBAR_CLEAR", WebHandler.WAITSEC.Curto).Click();
        handler.GetElement("SEARCHBAR_LOGO", WebHandler.WAITSEC.Curto).Click();
        handler.GetElement("SEARCHBAR_INPUT", WebHandler.WAITSEC.Curto).Click();
      }
      // Fill search bar with workorder number char by char
      handler.SendKeyByKey(servico.ToString());
      // Try to click on the first workorder on list
      handler.GetElement("SEARCHBAR_ITEM", WebHandler.WAITSEC.Medio).Click();
      Log.Information("Nota encontrada! Entrando...", servico);
      handler.GetElement("ACTIVITY_CABECALHO", WebHandler.WAITSEC.Medio);
    }
    public GeneralInfo? GetActivityGeneralInfo()
    {
      IsFinished();
      var result = new GeneralInfo();
      Log.Information("Obtendo informações gerais da nota...");
      var cabecalho = handler.GetElement("ACTIVITY_CABECALHO");
      result.Recurso = cabecalho.Text.Split(',').First().Trim();
      result.Data = cabecalho.Text.Split(',').Last().Trim();
      result.Atividade = handler.GetElement("ACTIVITY_ATIVIDADE").Text;
      result.NotaServico = handler.GetElement("ACTIVITY_SERVICO").Text;
      result.Situacao = handler.GetElement("ACTIVITY_SITUACAO").Text;
      result.Damage = handler.GetElement("ACTIVITY_DAMAGE").Text;
      result.Vencimento = handler.GetElement("ACTIVITY_VENCIMENTO").Text;
      result.Descricao = handler.GetElement("ACTIVITY_DESCRICAO").Text;
      result.Observacao = handler.GetElement("ACTIVITY_OBSERVA").Text.RemoveLineEndings();
      Log.Information("Informações obtidas:\n{resultado}", result);
      return result;
    }
    public List<String> GetActivityUploads(bool fotografias_ou_evidencias)
    {
      IsFinished();
      var files = new List<String>();
      Log.Information("Realizando downloads dos arquivos...");
      var pathname = fotografias_ou_evidencias ? "ACTIVITY_ARQUIVOS" : "ACTIIVITY_EVIDENCIAS";
      var acrescimo = fotografias_ou_evidencias ? 10 : 20;
      handler.GetElement(pathname, WebHandler.WAITSEC.Curto).Click();
      var downloads = handler.GetElements("ACTIVITY_DOWNLOADS", WebHandler.WAITSEC.Medio);
      if (downloads.Count > 10)
        throw new InvalidOperationException("Muitos arquivos anexados! Baixar manualmente!");
      for (int i = 1; i <= downloads.Count; i++)
        files.Add(handler.DownloadFile(downloads[i - 1], servico.ToString(), i - 1 + acrescimo));
      BackToBlack();
      return files;
    }
    public List<FinalizaInfo> GetActivityClosings()
    {
      IsFinished();
      var result = new List<FinalizaInfo>();
      Log.Information("Obtendo informações da finalização...");
      var rejeicao = handler.GetElements("ACTIVITY_REJEICAO", WebHandler.WAITSEC.Agora).FirstOrDefault();
      if (rejeicao is not null)
      {
        result.Add(
          new FinalizaInfo()
          {
            NotaServico = servico.ToString(),
            Codigo = rejeicao.Text,
            Quantidade = 1.ToString()
          }
        );
        Log.Information("Finalizações obtidas:\n{resultado}",
          string.Join('\n', result.Select(r => r.ToString())));
        return result;
      }
      handler.GetElement("ACTIVITY_FINALIZA", WebHandler.WAITSEC.Agora).Click();
      var iframe = handler.GetElement("ACTIVITY_FINALIZA_IFRAME", WebHandler.WAITSEC.Medio);
      handler.ExchangeContext("ACTIVITY_FINALIZA_IFRAME");
      var tabela = handler.GetElement("ACTIVITY_FINALIZA_TABLE");
      var tabelaResult = handler.GetTableData(tabela);
      handler.ExchangeContext();
      BackToBlack();
      result = tabelaResult.Select(linha =>
        new FinalizaInfo
        {
          NotaServico = servico.ToString(),
          Codigo = linha[0],
          Quantidade = linha[1]
        }
      ).ToList();
      Log.Information("Finalizações obtidas:\n{resultado}",
        string.Join('\n', result.Select(r => r.ToString())));
      return result;
    }
    public List<MaterialInfo> GetActivityMaterials()
    {
      IsFinished();
      var result = new List<MaterialInfo>();
      Log.Information("Obtendo informações do material...");
      handler.GetElement("ACTIVITY_MATERIAL", WebHandler.WAITSEC.Agora).Click();
      var tabelas = handler.GetElements("GLOBAL_TABLE", WebHandler.WAITSEC.Total);
      if (!tabelas.Any()) throw new InvalidOperationException("Nenhuma tabela foi encontrada!");
      foreach (var tabela in tabelas)
      {
        var origem = handler.GetElementAttribute(tabela, "ACTIVITY_MATERIAL_ORIGEM");
        var conteudoTabela = handler.GetTableData(tabela);
        result.AddRange(conteudoTabela.Select(linha =>
          new MaterialInfo
          {
            Nota = servico.ToString(),
            Tipo = linha[0],
            Codigo = linha[1],
            Serie = linha[2],
            Descricao = linha.Count == 5 ? linha[3] : origem == "customer" ? linha[3] : linha[4],
            Quantidade = linha.Count == 5 ? linha[4] : linha[5],
            Origem = origem
          }
        ));
      }
      BackToBlack();
      Log.Information("Materiais obtidos:\n{resultado}",
        string.Join('\n', result.Select(r => r.ToString())));
      return result;
    }
    public OcorrenciaInfo? GetActivityOcorrencias()
    {
      IsFinished();
      Log.Information("Obtendo informações da ocorrências...");
      handler.GetElement("ACTIVITY_INSPECAO", WebHandler.WAITSEC.Curto).Click();
      if (!handler.GetElements("IDENTIFICACAO_NUMEROTOI", WebHandler.WAITSEC.Medio).Any()) return null;
      var result = new OcorrenciaInfo();
      result.NotaServico = servico.ToString();
      // Sessão IDENTIFICAÇÃO no formulário de INSPECAO
      result.CaixaTipo = handler.GetElements("IDENTIFICACAO_TIPOCAIXA").FirstOrDefault()?.Text;
      result.CaixaModelo = handler.GetElements("IDENTIFICACAO_MODELOCAIXA").FirstOrDefault()?.Text;
      result.NumeroToi = handler.GetElements("IDENTIFICACAO_NUMEROTOI").FirstOrDefault()?.Text;
      result.NomeTitular = handler.GetElements("IDENTIFICACAO_NOMETITULAR").FirstOrDefault()?.Text;
      result.DocumentoTipo = handler.GetElements("IDENTIFICACAO_TIPODOC").FirstOrDefault()?.Text;
      result.DocumentoNum = handler.GetElements("IDENTIFICACAO_NUMDOC").FirstOrDefault()?.Text;
      result.ResidenciaClasse = handler.GetElements("RESIDENCIA_CLASSE").FirstOrDefault()?.Text;
      // Sessão DETALHES no formulário de INSPECAO
      result.MotivoInspecao = handler.GetElements("MOTIVO_INSPECAO").FirstOrDefault()?.Text;
      result.InstalacaoSuspensa = handler.GetElements("INSTALACAO_SUSPENSA").FirstOrDefault()?.Text;
      result.InstalacaoNormalizada = handler.GetElements("INSTALACAO_NORMALIZADA").FirstOrDefault()?.Text;
      result.ConsumidorAcompanhou = handler.GetElements("CONSUMIDOR_ACOMPANHOU").FirstOrDefault()?.Text;
      result.ClienteAutorizouLevantamento = handler.GetElements("CONSUMIDOR_AUTORIZOU").FirstOrDefault()?.Text;
      result.ClienteSolicitouPericia = handler.GetElements("CONSUMIDOR_SOLICITOU").FirstOrDefault()?.Text;
      result.ClienteQualAssinou = handler.GetElements("CONSUMIDOR_IDENTIFICADO").FirstOrDefault()?.Text;
      result.ClienteRecusouAssinar = handler.GetElements("CONSUMIDOR_ASSINOU").FirstOrDefault()?.Text;
      result.ClienteRecusouReceber = handler.GetElements("CONSUMIDOR_RECEBEU").FirstOrDefault()?.Text;
      result.FisicoEntregueTOI = handler.GetElements("VIA_AMARELA").FirstOrDefault()?.Text;
      result.QuantidadeEvidencias = handler.GetElements("EVIDENCIAS_QUANTIDADE").FirstOrDefault()?.Text;
      result.ExistenciaEvidencias = handler.GetElements("EVIDENCIAS_EXISTEM").FirstOrDefault()?.Text;
      result.DescricaoIrregularidade = handler.GetElements("DESCRICAO_IRREGULARIDADE").FirstOrDefault()?.Text.RemoveLineEndings();
      // Sessão LIGAÇÃO no formulário de INSPECAO
      result.GrupoTarifarico = handler.GetElements("GRUPO_TARIFARICO").FirstOrDefault()?.Text;
      result.LigacaoTipo = handler.GetElements("MEDICAO_TIPO").FirstOrDefault()?.Text;
      result.QuantidadeElementos = handler.GetElements("ELEMENTOS_QNT").FirstOrDefault()?.Text;
      result.FornecimentoTipo = handler.GetElements("TIPO_FORNECIMENTO").FirstOrDefault()?.Text;
      result.TensaoTipo = handler.GetElements("TENSAO_TIPO").FirstOrDefault()?.Text;
      result.TensaoNivel = handler.GetElements("TENSAO_NIVEL").FirstOrDefault()?.Text;
      result.RamalTipo = handler.GetElements("RAMAL_TIPO").FirstOrDefault()?.Text;
      result.SistemaEncapsulado = handler.GetElements("ENCAPSULADO").FirstOrDefault()?.Text;
      // Sessão MEDIDOR no formulário de INSPECAO
      result.MedidorTipo = handler.GetElements("MEDIDOR_TIPO").FirstOrDefault()?.Text;
      result.MedidorNumero = handler.GetElements("MEDIDOR_NUMERO").FirstOrDefault()?.Text;
      result.MedidorMarca = handler.GetElements("MEDIDOR_MARCA").FirstOrDefault()?.Text;
      result.MedidorAno = handler.GetElements("MEDIDOR_ANO").FirstOrDefault()?.Text;
      result.MedidorPatrimonio = handler.GetElements("MEDIDOR_PATRIMONIO").FirstOrDefault()?.Text;
      result.MedidorTensao = handler.GetElements("MEDIDOR_TENSAO").FirstOrDefault()?.Text;
      result.MedidorANominal = handler.GetElements("MEDIDOR_A_NOMINAL").FirstOrDefault()?.Text;
      result.MedidorAMaximo = handler.GetElements("MEDIDOR_A_MAXIMO").FirstOrDefault()?.Text;
      result.MedidorConstante = handler.GetElements("MEDIDOR_CONSTANTE").FirstOrDefault()?.Text;
      result.MedidorLocalizacao = handler.GetElements("MEDIDOR_LOCALIZACAO").FirstOrDefault()?.Text;
      result.MedidorObservacao = handler.GetElements("MEDIDOR_OBSERVACAO").FirstOrDefault()?.Text.RemoveLineEndings();
      // Sessão DECLARANTE no formulário de INSPECAO
      result.DeclaranteNomeCompleto = handler.GetElements("DECLARANTE_NOMECOMPLETO").FirstOrDefault()?.Text;
      result.DeclaranteGrauAfiinidade = handler.GetElements("DECLARANTE_GRAUAFINIDADE").FirstOrDefault()?.Text;
      result.DeclaranteDocumento = handler.GetElements("DECLARANTE_NUMDOCUMENTO").FirstOrDefault()?.Text;
      result.DeclaranteTempoOcupacao = handler.GetElements("DECLARANTE_TEMPOOCUPACAO").FirstOrDefault()?.Text;
      result.DeclaranteTempoUnidade = handler.GetElements("DECLARANTE_TEMPOUNIDADE").FirstOrDefault()?.Text;
      result.DeclaranteTipoOcupacao = handler.GetElements("DECLARANTE_TIPOOCUPACAO").FirstOrDefault()?.Text;
      result.DeclaranteQntResidentes = handler.GetElements("DECLARANTE_QNTRESIDENTES").FirstOrDefault()?.Text;
      result.DeclaranteEmail = handler.GetElements("DECLARANTE_EMAIL").FirstOrDefault()?.Text;
      result.DeclaranteCelular = handler.GetElements("DECLARANTE_CELULAR").FirstOrDefault()?.Text;
      // Sessão SELAGEM no formulário de INSPECAO
      result.SelagemTampos = handler.GetElements("SELAGEM_TAMPOS").FirstOrDefault()?.Text;
      result.SelagemBornes = handler.GetElements("SELAGEM_BORNES").FirstOrDefault()?.Text;
      result.SelagemParafuso = handler.GetElements("SELAGEM_PARAFUSO").FirstOrDefault()?.Text;
      result.SelagemTrava = handler.GetElements("SELAGEM_TRAVA").FirstOrDefault()?.Text;
      result.SelagemTampa = handler.GetElements("SELAGEM_TAMPA").FirstOrDefault()?.Text;
      result.SelagemBase = handler.GetElements("SELAGEM_BASE").FirstOrDefault()?.Text;
      result.SelagemGeral = handler.GetElements("SELAGEM_GERAL").FirstOrDefault()?.Text;
      BackToBlack();
      Log.Information("Ocorrência obtida:\n{resultado}", result);
      return result;
    }
    public String GetServico()
    {
      var builder = new System.Text.StringBuilder();
      try
      {
        SearchAndEnterActivity();
        builder.Append(GetActivityGeneralInfo());
        GetActivityUploads();
        builder.Append(GetActivityClosings());
        builder.Append(GetActivityMaterials());
      }
      catch (Exception e)
      {
        builder.Append(e.Message);
        return builder.ToString();
      }
      return builder.ToString();
    }
  }
}
