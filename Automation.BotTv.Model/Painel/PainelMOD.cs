using System.ComponentModel.DataAnnotations;

namespace Automation.BotTv.Model
{
    public class PainelMOD
    {
        public int CdPainel { get; set; }
        [Display(Name = "Nome do Painel")]
        public string NoPainel { get; set; }
        [Display(Name = "URL do Painel")]
        public string TxUrlPainel { get; set; }
        [Display(Name = "Tipo do Painel")]
        public int CdPainelTipo { get; set; }
        public string SnAtivo { get; set; }
        public int CdUsuarioCadastrou { get; set; }
        public DateTime DtCadastro { get; set; }
        public int? CdUsuarioAlterou { get; set; }
        public DateTime DtAlteracao { get; set; }

        public PainelTipoMOD PainelTipo { get; set; } = new PainelTipoMOD();
    }
}
