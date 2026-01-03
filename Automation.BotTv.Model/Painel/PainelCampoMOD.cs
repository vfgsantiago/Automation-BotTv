using System.ComponentModel.DataAnnotations;

namespace Automation.BotTv.Model
{ 
    public class PainelCampoMOD
    {
        [Display(Name = "Campo do Painel")]
        public int CdPainelCampo { get; set; }
        [Display(Name = "Campo")]
        public int CdCampo { get; set; }
        [Display(Name = "Painel")]
        public int CdPainel { get; set; }
        [Display(Name = "Ordem de Execução")]
        public int NrOrdem { get; set; }
        public string SnCampoValidacao { get; set; }

        public CampoMOD Campo { get; set; } = new CampoMOD();
    }
}
