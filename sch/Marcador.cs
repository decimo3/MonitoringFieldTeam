namespace automation.schemas;
public class Marcador
{
  public Int32 par_pid { get; set; }
  public String gps_status { get; set; } = String.Empty;
  public Int32 gps_start { get; set; }
  public Int32 gps_dur { get; set; }
  public Int32 gps_left { get; set; }
  public Int32 gps_width { get; set; }
}

/*
  <div
    class="toaGantt-tl-gpsmark gps-status-normal"
    start="552"
    dur="141"
    style="left: 1686px; width: 141px"
  ></div>
*/