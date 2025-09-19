using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Automation.Persistence;
using System.Collections.ObjectModel;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    private void BackToBlack()
    {
      GetElement(By.ClassName("oj-ux-ico-arrow-up")).Click();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    }
    private bool IsFinished()
    {
      return (GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_SITUACAO"]))?.Text ?? string.Empty).Contains("concluído");
    }
    private ReadOnlyCollection<IWebElement>? GetElements(By by, int miliseconds = 0)
    {
      miliseconds = miliseconds != 0 ? miliseconds : this.cfg.ESPERAS["CURTA"];
      var endTime = DateTime.Now.AddMilliseconds(miliseconds);
      while (DateTime.Now < endTime)
      {
        try
        {
          var elements = this.driver.FindElements(by);
          if (elements.Count == 0)
            continue;
          var element = elements.First();
          if (element.Displayed && element.Enabled)
            return elements;
        }
        catch (NoSuchElementException)
        {
          // Element not yet in DOM, keep trying
        }
        catch (StaleElementReferenceException)
        {
          // DOM updated, try again
        }
        catch (WebDriverException)
        {
          // Catch all transient WebDriver errors, continue retrying
        }
        Thread.Sleep(200); // Avoid busy waiting
      }
      return null;
    }
    private IWebElement? GetElement(By by)
    {
      var elements = GetElements(by);
      if (elements is null)
        return null;
      if (elements.Count > 1)
        throw new Exception($"Mais de um elemento encontrado: {by}");
      return elements[0];
    }
    private List<List<string>> GetTableActivity(IWebElement tableElement)
    {
      var resultTable = new List<List<string>>();
      var linhas = tableElement.FindElements(By.XPath(".//tr"));
      if (linhas.Count == 0)
        return resultTable;
      foreach (var linha in linhas)
      {
        var valores = new List<string>();
        if (String.IsNullOrEmpty(linha.Text)) continue;
        var celulas = linha.FindElements(By.XPath(".//td"));
        foreach (var celula in celulas)
        {
            valores.Add(celula.Text?.Replace(';',' '));
        }
        resultTable.Add(valores);
      }
      return resultTable;
    }
    public void SearchAndEnterActivity(String workorder)
    {
      // Click on search bar to focus cursor on
      GetElements(By.ClassName("search-bar-input")).First().Click();
      // Fill search bar with workorder number char by char
      var actions = new Actions(this.driver);
      foreach (var c in workorder)
      {
        actions.KeyDown(c.ToString()).Perform();
        actions.KeyUp(c.ToString()).Perform();
      }
      // Await amount of time and check if there is a response
      if(!GetElements(By.ClassName("found-item-activity")).Any())
      {
        throw new Exception($"A nota de serviço não foi encontrada!");
      }
      // Click on the first workorder on list
      GetElements(By.ClassName("found-item-activity")).First().Click();
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
      result.Observacao = GetElement(By.XPath(this.cfg.CAMINHOS["ACTIVITY_OBSERVA"]))?.Text.Replace('\n', ' ');
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
            Descricao = linha[3],
            Quantidade = linha[4],
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
      result.DescricaoIrregularidade = GetElement(By.XPath(this.cfg.CAMINHOS["DESCRICAO_IRREGULARIDADE"]))?.Text.Replace('\n', ' ');
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
      result.MedidorObservacao = GetElement(By.XPath(this.cfg.CAMINHOS["MEDIDOR_OBSERVACAO"]))?.Text.Replace('\n', ' ');
      BackToBlack();
      return result;
    }
    public String GetServico(String arg)
    {
      var builder = new System.Text.StringBuilder();
      try
      {
        SearchAndEnterActivity(arg);
      }
      catch (Exception e)
      {
        builder.Append(e.Message);
        return builder.ToString();
      }
      if (IsFinished())
      {
        builder.Append("A nota de servico não está finalizada!");
        return builder.ToString();
      }
      try
      {
        builder.Append(GetActivityGeneralInfo(arg));
      }
      catch (Exception e)
      {
        builder.Append(e.Message);
        return builder.ToString();
      }
      try
      {
        GetActivityUploads(arg);
      }
      catch (Exception e)
      {
        builder.Append(e.Message);
        return builder.ToString();
      }
      try
      {
        builder.Append(GetActivityClosings(arg));
      }
      catch (Exception e)
      {
        builder.Append(e.Message);
        return builder.ToString();
      }
      try
      {
        builder.Append(GetActivityMaterials(arg));
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
