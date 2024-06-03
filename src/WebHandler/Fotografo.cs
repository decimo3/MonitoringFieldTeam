namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Fotografo()
    {
      var filename = $"{this.cfg.DOWNFOLDER}/{this.agora.ToString("yyyyMMdd_HHmmss")}_{this.balde_nome}.png";
      var largura_script = "return document.body.parentNode.scrollWidth";
      var tamanho_script = "return document.body.parentNode.scrollHeight";
      var largura_retorno = (Int64)this.driver.ExecuteScript(largura_script);
      var tamanho_retorno = (Int64)this.driver.ExecuteScript(tamanho_script);
      var largura_necessario = Int32.Parse(largura_retorno.ToString());
      var tamanho_necessario = Int32.Parse(tamanho_retorno.ToString());
      var tamanho_janela = new System.Drawing.Size(largura_necessario, tamanho_necessario);
      this.driver.Manage().Window.Size = tamanho_janela;
      this.driver.GetScreenshot().SaveAsFile(filename);
      this.driver.Manage().Window.Maximize();
      return;
    }
  }
}
