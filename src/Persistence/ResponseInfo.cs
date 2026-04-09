namespace MonitoringFieldTeam.Persistence;
/// <summary>
/// Class that hold all info get from server mode
/// </summary>
public class ResponseInfo
{
  public GeneralInfo? GeneralInfo { get; set; }
  public List<FinalizaInfo>? FinalizaInfo { get; set; }
  public List<MaterialInfo>? MaterialInfo { get; set; }
  public OcorrenciaInfo? OcorrenciaInfo { get; set; }
  public List<Uri>? UploadsInfo { get; set; }
  public List<Uri>? EvidenceInfo { get; set; }
}
