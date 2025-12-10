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
    public void SearchAndEnterActivity(String workorder)
    {
      // Click on search bar to focus cursor on
      try
      {
        GetElement(By.ClassName("search-bar-input")).Click();
      }
      catch (ElementClickInterceptedException)
      {
        GetElement(By.ClassName("search-bar-input-clear")).Click();
        GetElement(By.ClassName("logo-image")).Click();
        GetElement(By.ClassName("search-bar-input")).Click();
      }
      // Fill search bar with workorder number char by char
      var actions = new Actions(this.driver);
      foreach (var c in workorder)
      {
        actions.KeyDown(c.ToString()).Perform();
        actions.KeyUp(c.ToString()).Perform();
      }
      // Await amount of time and check if there is a response
      if(GetElements(By.ClassName("found-item-activity")) is null)
      {
        throw new Exception($"A nota de serviço não foi encontrada!");
      }
      // Click on the first workorder on list
      GetElement(By.ClassName("found-item-activity")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    }
    public GeneralInfo? GetActivityGeneralInfo(string nota)
    {
      if (!IsFinished()) return null;
      var result = new GeneralInfo();
      result.Data = GetElement(By.ClassName("page-header-description"))?.Text.Split(',').Last();
      result.Recurso = GetElement(By.ClassName("page-header-description"))?.Text.Split(',').First();
      result.Atividade = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_ATIVIDADE"]))?.Text;
      result.NotaServico = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_SERVICO"]))?.Text;
      result.Situacao = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_SITUACAO"]))?.Text;
      result.Damage = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_DAMAGE"]))?.Text;
      result.Vencimento = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_VENCIMENTO"]))?.Text;
      result.Descricao = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_DESCRICAO"]))?.Text;
      result.Observacao = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_OBSERVA"]))?.Text.RemoveLineEndings();
      return result;
    }
    public void GetActivityUploads(string nota)
    {
      if (!IsFinished()) return;
      GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_ARQUIVOS"])).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      foreach (var download in GetElements(By.ClassName("download-button")))
        download.Click();
      BackToBlack();
    }
    public List<FinalizaInfo> GetActivityClosings(string nota)
    {
      if (!IsFinished()) return new List<FinalizaInfo>();
      var rejeicao = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_REJEICAO"]));
      if (rejeicao is not null)
      {
        return new List<FinalizaInfo>
        {
          new() {
            NotaServico = nota,
            Codigo = rejeicao.Text,
            Quantidade = 1.ToString()
          }
        };
      }
      GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_FINALIZA"])).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      var frame = GetElement(By.ClassName("content-iframe"));
      this.driver.SwitchTo().Frame(frame);
      var tabela = GetElement(By.TagName("tbody")); // By.Id("itens-selected")
      var tabelaResult = GetTableActivity(tabela);
      this.driver.SwitchTo().DefaultContent();
      BackToBlack();
      return tabelaResult.Select(linha =>
        new FinalizaInfo
        {
          NotaServico = nota,
          Codigo = linha[0],
          Quantidade = linha[1]
        }
      ).ToList();
    }
    public List<MaterialInfo> GetActivityMaterials(string nota)
    {
      if (!IsFinished()) return new List<MaterialInfo>();
      var result = new List<MaterialInfo>();
      GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_MATERIAL"])).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      var tabelas = GetElements(By.TagName("tbody"));
      foreach (var tabela in tabelas)
      {
        var origem = tabela.GetDomAttribute("data-ofsc-inventory-pool");
        var conteudoTabela = GetTableActivity(tabela);
        result.AddRange(conteudoTabela.Select(linha =>
          new MaterialInfo
          {
            Nota = nota,
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
      return result;
    }
    public OcorrenciaInfo? GetActivityOcorrencias(string nota)
    {
      if (!IsFinished()) return null;
      GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_INSPECAO"])).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
      if (GetElement(By.XPath(this.cfg.CAMINHOS["IDENTIFICACAO_NUMEROTOI"])) is null) return null;
      var result = new OcorrenciaInfo();
      result.NotaServico = nota;
      // Sessão IDENTIFICAÇÃO no formulário de INSPECAO
      result.CaixaTipo = GetElement(By.XPath(this.cfg.CAMINHOS["IDENTIFICACAO_TIPOCAIXA"]))?.Text;
      result.CaixaModelo = GetElement(By.XPath(this.cfg.CAMINHOS["IDENTIFICACAO_MODELOCAIXA"]))?.Text;
      result.NumeroToi = GetElement(By.XPath(this.cfg.CAMINHOS["IDENTIFICACAO_NUMEROTOI"]))?.Text;
      result.NomeTitular = GetElement(By.XPath(this.cfg.CAMINHOS["IDENTIFICACAO_NOMETITULAR"]))?.Text;
      result.DocumentoTipo = GetElement(By.XPath(this.cfg.CAMINHOS["IDENTIFICACAO_TIPODOC"]))?.Text;
      result.DocumentoNum = GetElement(By.XPath(this.cfg.CAMINHOS["IDENTIFICACAO_NUMDOC"]))?.Text;
      result.ResidenciaClasse = GetElement(By.XPath(this.cfg.CAMINHOS["RESIDENCIA_CLASSE"]))?.Text;
      // Sessão DETALHES no formulário de INSPECAO
      result.MotivoInspecao = GetElement(By.XPath(this.cfg.CAMINHOS["MOTIVO_INSPECAO"]))?.Text;
      result.InstalacaoSuspensa = GetElement(By.XPath(this.cfg.CAMINHOS["INSTALACAO_SUSPENSA"]))?.Text;
      result.InstalacaoNormalizada = GetElement(By.XPath(this.cfg.CAMINHOS["INSTALACAO_NORMALIZADA"]))?.Text;
      result.ConsumidorAcompanhou = GetElement(By.XPath(this.cfg.CAMINHOS["CONSUMIDOR_ACOMPANHOU"]))?.Text;
      result.ClienteAutorizouLevantamento = GetElement(By.XPath(this.cfg.CAMINHOS["CONSUMIDOR_AUTORIZOU"]))?.Text;
      result.ClienteSolicitouPericia = GetElement(By.XPath(this.cfg.CAMINHOS["CONSUMIDOR_SOLICITOU"]))?.Text;
      result.ClienteQualAssinou = GetElement(By.XPath(this.cfg.CAMINHOS["CONSUMIDOR_IDENTIFICADO"]))?.Text;
      result.ClienteRecusouAssinar = GetElement(By.XPath(this.cfg.CAMINHOS["CONSUMIDOR_ASSINOU"]))?.Text;
      result.ClienteRecusouReceber = GetElement(By.XPath(this.cfg.CAMINHOS["CONSUMIDOR_RECEBEU"]))?.Text;
      result.FisicoEntregueTOI = GetElement(By.XPath(this.cfg.CAMINHOS["VIA_AMARELA"]))?.Text;
      result.QuantidadeEvidencias = GetElement(By.XPath(this.cfg.CAMINHOS["EVIDENCIAS_QUANTIDADE"]))?.Text;
      result.ExistenciaEvidencias = GetElement(By.XPath(this.cfg.CAMINHOS["EVIDENCIAS_EXISTEM"]))?.Text;
      result.DescricaoIrregularidade = GetElement(By.XPath(this.cfg.CAMINHOS["DESCRICAO_IRREGULARIDADE"]))?.Text.RemoveLineEndings();
      // Sessão LIGAÇÃO no formulário de INSPECAO
      result.GrupoTarifarico = GetElement(By.XPath(this.cfg.CAMINHOS["GRUPO_TARIFARICO"]))?.Text;
      result.LigacaoTipo = GetElement(By.XPath(this.cfg.CAMINHOS["MEDICAO_TIPO"]))?.Text;
      result.QuantidadeElementos = GetElement(By.XPath(this.cfg.CAMINHOS["ELEMENTOS_QNT"]))?.Text;
      result.FornecimentoTipo = GetElement(By.XPath(this.cfg.CAMINHOS["TIPO_FORNECIMENTO"]))?.Text;
      result.TensaoTipo = GetElement(By.XPath(this.cfg.CAMINHOS["TENSAO_TIPO"]))?.Text;
      result.TensaoNivel = GetElement(By.XPath(this.cfg.CAMINHOS["TENSAO_NIVEL"]))?.Text;
      result.RamalTipo = GetElement(By.XPath(this.cfg.CAMINHOS["RAMAL_TIPO"]))?.Text;
      result.SistemaEncapsulado = GetElement(By.XPath(this.cfg.CAMINHOS["ENCAPSULADO"]))?.Text;
      // Sessão MEDIDOR no formulário de INSPECAO
      result.MedidorTipo = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_TIPO"]))?.Text;
      result.MedidorNumero = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_NUMERO"]))?.Text;
      result.MedidorMarca = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_MARCA"]))?.Text;
      result.MedidorAno = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_ANO"]))?.Text;
      result.MedidorPatrimonio = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_PATRIMONIO"]))?.Text;
      result.MedidorTensao = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_TENSAO"]))?.Text;
      result.MedidorANominal = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_A_NOMINAL"]))?.Text;
      result.MedidorAMaximo = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_A_MAXIMO"]))?.Text;
      result.MedidorConstante = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_CONSTANTE"]))?.Text;
      result.MedidorLocalizacao = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_LOCALIZACAO"]))?.Text;
      result.MedidorObservacao = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_OBSERVACAO"]))?.Text.RemoveLineEndings();
      // Sessão DECLARANTE no formulário de INSPECAO
      result.DeclaranteNomeCompleto = GetElement(By.XPath(this.cfg.CAMINHOS["DECLARANTE_NOMECOMPLETO"]))?.Text;
      result.DeclaranteGrauAfiinidade = GetElement(By.XPath(this.cfg.CAMINHOS["DECLARANTE_GRAUAFINIDADE"]))?.Text;
      result.DeclaranteDocumento = GetElement(By.XPath(this.cfg.CAMINHOS["DECLARANTE_NUMDOCUMENTO"]))?.Text;
      result.DeclaranteTempoOcupacao = GetElement(By.XPath(this.cfg.CAMINHOS["DECLARANTE_TEMPOOCUPACAO"]))?.Text;
      result.DeclaranteTempoUnidade = GetElement(By.XPath(this.cfg.CAMINHOS["DECLARANTE_TEMPOUNIDADE"]))?.Text;
      result.DeclaranteTipoOcupacao = GetElement(By.XPath(this.cfg.CAMINHOS["DECLARANTE_TIPOOCUPACAO"]))?.Text;
      result.DeclaranteQntResidentes = GetElement(By.XPath(this.cfg.CAMINHOS["DECLARANTE_QNTRESIDENTES"]))?.Text;
      result.DeclaranteEmail = GetElement(By.XPath(this.cfg.CAMINHOS["DECLARANTE_EMAIL"]))?.Text;
      result.DeclaranteCelular = GetElement(By.XPath(this.cfg.CAMINHOS["DECLARANTE_CELULAR"]))?.Text;
      // Sessão SELAGEM no formulário de INSPECAO
      result.SelagemTampos = GetElement(By.XPath(this.cfg.CAMINHOS["SELAGEM_TAMPOS"]))?.Text;
      result.SelagemBornes = GetElement(By.XPath(this.cfg.CAMINHOS["SELAGEM_BORNES"]))?.Text;
      result.SelagemParafuso = GetElement(By.XPath(this.cfg.CAMINHOS["SELAGEM_PARAFUSO"]))?.Text;
      result.SelagemTrava = GetElement(By.XPath(this.cfg.CAMINHOS["SELAGEM_TRAVA"]))?.Text;
      result.SelagemTampa = GetElement(By.XPath(this.cfg.CAMINHOS["SELAGEM_TAMPA"]))?.Text;
      result.SelagemBase = GetElement(By.XPath(this.cfg.CAMINHOS["SELAGEM_BASE"]))?.Text;
      result.SelagemGeral = GetElement(By.XPath(this.cfg.CAMINHOS["SELAGEM_GERAL"]))?.Text;
      BackToBlack();
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
