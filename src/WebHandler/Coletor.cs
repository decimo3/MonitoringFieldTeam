using Automation.Persistence;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
namespace Automation.WebScraper;
public partial class Manager
{
  public void Coletor()
  {
    Console.WriteLine($"{DateTime.Now} - Coletando espelhos...");
    // Coleta de nome de recurso e par_pid
    for(var i = 1; true; i++)
    {
      var recursos_path = this.cfg.CAMINHOS["RECURSOS_"].Replace("_", i.ToString());
      var recursos_query = this.driver.FindElements(By.XPath(recursos_path));
      if(!recursos_query.Any()) break;
      var recursos = recursos_query.Single();
      var texto = recursos.FindElement(By.XPath(".//div")).Text;
      while(String.IsNullOrEmpty(texto))
      {
        // DONE - Scroll down para obter nome dos demais recursos
        var tabela = this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["CONTAINER"]));
        var scrollOrigin = new WheelInputDevice.ScrollOrigin { Element = tabela };
        new Actions(this.driver).ScrollFromOrigin(scrollOrigin, 0, 1).Perform();
        texto = recursos.FindElement(By.XPath(".//div")).Text;
      }
      var par_pid = Int32.Parse(recursos.GetAttribute("par_pid"));
      var style_top = ColetarStyle(recursos.GetDomAttribute("style"))["top"];
      this.espelhos.Add(new Espelho(texto, par_pid, style_top));
    }
    if(!this.espelhos.Any()) return;
    // Retornar até o topo da lista
    Console.WriteLine($"{DateTime.Now} - Retornando ao topo...");
    for(var a = 0; a < this.espelhos.Count; a++)
    {
      var tabela = this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["CONTAINER"]));
      var scrollOrigin = new WheelInputDevice.ScrollOrigin { Element = tabela };
      new Actions(this.driver).ScrollFromOrigin(scrollOrigin, 0, -1).Perform();
    }
    // Pecorre a lista de linhas do gráfico Gantt
    Console.WriteLine($"{DateTime.Now} - Coletando atividades...");
    for(var i = 1; true; i++)
    {
      var gantt_path = this.cfg.CAMINHOS["ESPELHOS_"].Replace("_", i.ToString());
      var gantt_query = this.driver.FindElements(By.XPath(gantt_path));
      if(!gantt_query.Any()) break;
      var gantt = gantt_query.Single();
      var style_top = ColetarStyle(gantt.GetDomAttribute("style"))["top"];
      var espelho = this.espelhos.Where(s => s.style_top == style_top).Single();
      var gantt_classes = gantt.GetAttribute("class").Split(" ");
      if(gantt_classes.Contains("toaGantt-hour-line")) continue;
      if(gantt_classes.Contains("toaGantt-tl"))
      {
        var par_pid = Int32.Parse(gantt.GetAttribute("par_pid"));
        var servicos = gantt.FindElements(By.XPath(".//div"));
        if(!servicos.Any()) break;
        // Pecorre a lista de atividades filhos do elemento
        foreach (var servico in servicos)
        {
          var servico_classes = servico.GetAttribute("class").Split(" ");
          // Verifica se é uma ordem de servico
          if(servico_classes.Contains("toaGantt-tb"))
          {
            if(servico_classes.Contains("final"))
            {
              espelho.final_dur = Int32.Parse(servico.GetDomAttribute("dur"));
              continue;
            }
            var servico_obj = new Servico();
            var estilos = ColetarStyle(servico.GetDomAttribute("style"));
            servico_obj.par_pid = Int32.Parse(servico.GetDomAttribute("par_pid"));
            servico_obj.start = Int32.Parse(servico.GetDomAttribute("start"));
            servico_obj.aid = Int32.Parse(servico.GetDomAttribute("aid").Split('|').Last());
            servico_obj.dur = Int32.Parse(servico.GetDomAttribute("dur"));
            servico_obj.data_activity_eta = Int32.Parse(servico.GetDomAttribute("data-activity-eta"));
            servico_obj.data_activity_status = (int)Enum.Parse<Servico.Status>(servico.GetDomAttribute("data-activity-status"));
            servico_obj.data_activity_worktype = Int32.Parse(servico.GetDomAttribute("data-activity-worktype"));
            servico_obj.data_activity_duration = Int32.Parse(servico.GetDomAttribute("data-activity-duration"));
            servico_obj.style_left = estilos["left"];
            servico_obj.style_width = estilos["width"];
            espelho.servicos.Add(servico_obj);
          }
          // Verifica se é uma janela de tempo
          if(servico_classes.Contains("toaGantt-tl-shift"))
          {
            espelho.shift_start = Int32.Parse(servico.GetDomAttribute("start"));
            espelho.shift_dur = Int32.Parse(servico.GetDomAttribute("dur"));
            var estilos = ColetarStyle(servico.GetDomAttribute("style"));
            espelho.shift_left = estilos["left"];
            espelho.shift_width = estilos["width"];
            continue;
          }
          // verifica se é uma alteração da jornada
          if(servico_classes.Contains("toaGantt-queue"))
          {
            if(servico_classes.Contains("toaGantt-queue-start"))
            {
              espelho.queue_start_start = Int32.Parse(servico.GetDomAttribute("start"));
              espelho.queue_start_left = ColetarStyle(servico.GetDomAttribute("style"))["left"];
              continue;
            }
            if(servico_classes.Contains("toaGantt-queue-reactivated"))
            {
              espelho.queue_reactivated_start = Int32.Parse(servico.GetDomAttribute("start"));
              espelho.queue_reactivated_left = ColetarStyle(servico.GetDomAttribute("style"))["left"];
              continue;
            }
            if(servico_classes.Contains("toaGantt-queue-end"))
            {
              espelho.queue_end_start = Int32.Parse(servico.GetDomAttribute("start"));
              espelho.queue_end_left = ColetarStyle(servico.GetDomAttribute("style"))["left"];
              continue;
            }
          }
          // Verifica se é uma alteração na tempo
          if(servico_classes.Contains("toaGantt-tl-gpsmark"))
          {
            var roteiros = new Roteiro();
            roteiros.start = Int32.Parse(servico.GetDomAttribute("start"));
            roteiros.dur = Int32.Parse(servico.GetDomAttribute("dur"));
            if(servico_classes.Contains("gps-status-normal")) roteiros.status = Roteiro.Status.normal;
            if(servico_classes.Contains("gps-status-idle")) roteiros.status = Roteiro.Status.idle;
            if(servico_classes.Contains("gps-status-alert")) roteiros.status = Roteiro.Status.alert;
            var estilos = ColetarStyle(servico.GetDomAttribute("style"));
            roteiros.style_width = estilos["width"];
            roteiros.style_left = estilos["left"];
            espelho.roteiros.Add(roteiros);
            continue;
          }
          if(servico_classes.Contains("toaGantt-tw"))
          {
            var estilos = ColetarStyle(servico.GetDomAttribute("style"));
            espelho.tw_alert_display = (estilos["display"] != 0) ? true : false;
            espelho.tw_alert_left = estilos["left"];
            continue;
          }
        }
      }
    }
  }
  public static Dictionary<String,Int32> ColetarStyle(String texto_estilo)
  {
    var resposta = new Dictionary<String,Int32>();
    var estilos = texto_estilo.Trim().Split(";");
    foreach (var estilo in estilos)
    {
      if(String.IsNullOrEmpty(estilo)) continue;
      var key_val = estilo.Replace(" ", "").Split(":");
      if(key_val.Length != 2) continue;
      var valor_sanitizado = key_val[1].Replace("px", "");
      if(Int32.TryParse(valor_sanitizado, out Int32 valor_numero))
      {
        resposta.Add(key_val[0], valor_numero);
        continue;
      }
      if(key_val[1] == "none") resposta.Add(key_val[0], 0);
      if(key_val[1] == "block") resposta.Add(key_val[0], 1);
    }
    return resposta;
  }
}