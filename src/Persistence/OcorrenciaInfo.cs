namespace MonitoringFieldTeam.Persistence
{
  public class OcorrenciaInfo
  {
    public string NotaServico { get; set; }
    // Sessão IDENTIFICAÇÃO no formulário de INSPECAO
    public string CaixaTipo { get; set; }
    public string CaixaModelo { get; set; }
    public string NumeroToi { get; set; }
    public string NomeTitular { get; set; }
    public string DocumentoTipo { get; set; }
    public string DocumentoNum { get; set; }
    public string ResidenciaClasse { get; set; }
    // Sessão DETALHES no formulário de INSPECAO
    public string MotivoInspecao { get; set; }
    public string InstalacaoSuspensa { get; set; }
    public string InstalacaoNormalizada { get; set; }
    public string ConsumidorAcompanhou { get; set; }
    public string ClienteAutorizouLevantamento { get; set; }
    public string ClienteSolicitouPericia { get; set; }
    public string ClienteQualAssinou { get; set; }
    public string ClienteRecusouAssinar { get; set; }
    public string ClienteRecusouReceber { get; set; }
    public string FisicoEntregueTOI { get; set; }
    public string QuantidadeEvidencias { get; set; }
    public string ExistenciaEvidencias { get; set; }
    public string DescricaoIrregularidade { get; set; }
    // Sessão LIGACAO no formulário de INSPECAO
    public string GrupoTarifarico { get; set; }
    public string LigacaoTipo { get; set; }
    public string QuantidadeElementos { get; set; }
    public string FornecimentoTipo { get; set; }
    public string TensaoTipo { get; set; }
    public string TensaoNivel { get; set; }
    public string RamalTipo { get; set; }
    public string SistemaEncapsulado { get; set; }
    // Sessão MEDIDOR no formulário de INSPECAO
    public string MedidorTipo { get; set; }
    public string MedidorNumero { get; set; }
    public string MedidorMarca { get; set; }
    public string MedidorAno { get; set; }
    public string MedidorPatrimonio { get; set; }
    public string MedidorTensao { get; set; }
    public string MedidorANominal { get; set; }
    public string MedidorAMaximo { get; set; }
    public string MedidorConstante { get; set; }
    public string MedidorLocalizacao { get; set; }
    public string MedidorObservacao { get; set; }
    // Sessão DECLARANTE no formulário de INSPECAO
    public string? DeclaranteNomeCompleto { get; set; }
    public string? DeclaranteGrauAfiinidade { get; set; }
    public string? DeclaranteDocumento { get; set; }
    public string? DeclaranteTempoOcupacao { get; set; }
    public string? DeclaranteTempoUnidade { get; set; }
    public string? DeclaranteTipoOcupacao { get; set; }
    public string? DeclaranteQntResidentes { get; set; }
    public string? DeclaranteEmail { get; set; }
    public string? DeclaranteCelular { get; set; }
    // Sessão SELAGEM no formulário de INSPECAO
    public string? SelagemTampos { get; set; }
    public string? SelagemBornes { get; set; }
    public string? SelagemParafuso { get; set; }
    public string? SelagemTrava { get; set; }
    public string? SelagemTampa { get; set; }
    public string? SelagemBase { get; set; }
    public string? SelagemGeral { get; set; }
  }
}
