namespace automation.schemas;
public class Atividade
{
  public Int32 aid { get; set; }
  public Int32 par_pid { get; set; }
  public DateOnly par_date { get; set; }
  public Boolean ordered { get; set; }
  public Int32 start { get; set; }
  public Int32 dur { get; set; }
  public Boolean movable { get; set; }
  public Boolean multiday { get; set; }
  public Int32 data_activity_eta { get; set; }
  public String data_activity_status { get; set; } = String.Empty;
  public String data_activity_type { get; set; } = String.Empty;
  public Int32 data_activity_worktype { get; set; }
  public Int32 data_master_id { get; set; }
  public Int32 data_activity_duration { get; set; }
  public String aria_label { get; set; } = String.Empty;
  public Int32 style_left { get; set; }
  public Int32 style_top { get; set; }
  public Int32 style_width { get; set; }
  public Int32 style_height { get; set; }
  public String style_bgcolor { get; set; } = String.Empty;
  public Int32 travel_dur { get; set; }
  public String innerText { get; set; } = String.Empty;
}

/*
  <div
    role="button"
    aria_dropeffect="move"
    data_id="a_9167800"
    class="toaGantt_tb"
    aid="9167800"
    tabindex="0"
    par_pid="9939"
    par_date="2024_02_02"
    ordered="true"
    start="910"
    dur="60"
    movable="0"
    multiday="0"
    data_activity_eta="910"
    data_activity_status="started"
    data_activity_type="regular"
    data_activity_worktype="1246"
    data_master_id=""
    data_activity_duration="60"
    aria_label="started regular INDISPONIBILIDADE activity"
    style="
      left: 2616px;
      width: 76px;
      background_color: #5dbe3f;
      border_color: rgb(74, 152, 50);
      z_index: 3000;
    "
  >
*/