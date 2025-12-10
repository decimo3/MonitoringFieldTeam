using Serilog;
namespace MonitoringFieldTeam.WebScraper;

public static class Atualizador
{
  public static string SelecionarBalde(WebHandler.WebHandler handler, string piscina, bool direcao)
  {
    Log.Information("Selecionando o(s) balde(s) {piscina}", piscina);
    // Check if it is on GANNT graph display. Throw error if not it is.
    handler.GetElement("GANNT_DISPLAY", WebHandler.WAITSEC.Total);
    // Get bucket parts and order due direction (to open or close buckets)
    var sub_baldes = direcao ? piscina.Split('>') : piscina.Split('>').Reverse().ToArray();
    // Get bucket arrow class due direction (to open or close buckets)
    var arrow_class = direcao ? "GANNT_ARROWPLUS" : "GANNT_ARROWMINUS";
    foreach (var sub_balde in sub_baldes)
    {
      var baldes = handler.GetElements("GANNT_BUCKETS", WebHandler.WAITSEC.Curto);
      foreach (var balde in baldes)
      {
        // var balde = handler.GetElement("GANNT_BUCKET", WebHandler.WAITSEC.Curto, j + 1);
        var texto = balde.Text; // handler.GetElementAttribute(balde, "GLOBAL_TEXT");
        if (string.IsNullOrEmpty(texto)) continue;
        Log.Debug("Balde {balde}, Texto: {texto}", balde, texto);
        if (texto.Contains(sub_balde))
        {
          if (sub_balde == sub_baldes.Last())
          {
            handler.GetNestedElements(balde, "GANNT_BUCKET").First().Click();
            Log.Information("Balde '{sub_balde}' selecionado com sucesso!", sub_balde);
            return direcao ? sub_baldes.Last() : sub_baldes.First();
          }
          handler.GetNestedElements(balde, arrow_class).FirstOrDefault()?.Click();
        }
      }
      throw new InvalidOperationException($"O balde `{sub_balde}` não foi encontrado!");
    }
    throw new InvalidOperationException($"Houve um erro ao tentar selecionar `{piscina}`!");
  }
  public static DateOnly Atualizar(WebHandler.WebHandler handler)
  {
    Log.Information("Atualizando o gráfico...");
    // Selecionar a visualização do gráfico de Gantt
    handler.GetElement("GANNT_CLOCKICON", WebHandler.WAITSEC.Total).Click();
    // Abrir menu de seleção de preferências
    var element = handler.GetElements("GANNT_TOOLBAR").First();
    handler.GetNestedElements(element, "GANNT_SHOWBTN").First().Click();
    // Selecionar para exibir de forma herarquica
    element = handler.GetElements("GANNT_FILTERVIEW", WebHandler.WAITSEC.Curto).First();
    element = handler.GetNestedElements(element, "GANNT_TREEGROUP").First();
    element = handler.GetNestedElements(element, "GANNT_FILTERCHECK").First();
    if (!element.Selected) element.Click();
    // Selecionar para exibir a rota do recurso
    element = handler.GetElements("GANNT_FILTERVIEW", WebHandler.WAITSEC.Curto).First();
    element = handler.GetNestedElements(element, "GANNT_ROUTEGROUP").First();
    element = handler.GetNestedElements(element, "GANNT_FILTERCHECK").First();
    if (!element.Selected) element.Click();
    // Ajusta o zoom da página para visualizar toda linha do tempo
    //parent = handler.GetElements("GANNT_FILTERVIEW", WebHandler.WAITSEC.Curto).First();
    //handler.GetNestedElements(parent, "ZOOM_FIT").First().Click();
    // Aplicar as preferências de seleções
    element = handler.GetElements("GANNT_FILTERVIEW").First();
    handler.GetNestedElements(element, "GANNT_APPLYBTN").First().Click();
    Log.Information("Gráfico atualizado!");
    return DateOnly.Parse(handler.GetElement("GANNT_DATEPICK", WebHandler.WAITSEC.Curto).Text);
  }
}
