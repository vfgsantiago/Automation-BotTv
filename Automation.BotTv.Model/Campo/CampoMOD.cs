using System.ComponentModel.DataAnnotations;

namespace Automation.BotTv.Model
{
    public class CampoMOD
    {
        public int CdCampo { get; set; }
        [Display(Name = "Tipo de Campo")]
        public int CdCampoTipo { get; set; }
        public string NoCampoTipo { get; set; }
        [Display(Name = "Campo")]
        public string NoCampo { get; set; }
        [Display(Name = "XPath")]
        public string TxPath { get; set; }
        [Display(Name = "Ação")]
        public int CdAcao { get; set; }
        public string NoAcao { get; set; }
        public string SnAtivo { get; set; }
        public int CdUsuarioCadastrou { get; set; }
        public DateTime DtCadastro { get; set; }
        public int CdUsuarioAlterou { get; set; }
        public DateTime DtAlteracao { get; set; }

        public CampoTipoMOD CampoTipo { get; set; } = new CampoTipoMOD();
        public CampoAcaoMOD CampoAcao { get; set; } = new CampoAcaoMOD();

    }
}
