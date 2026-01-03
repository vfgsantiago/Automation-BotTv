using System.ComponentModel.DataAnnotations;
using Automation.BotTv.Model;

namespace Automation.BotTv.UI.Web.Models
{
    public class PainelViewMOD
    {
        [Display(Name = "Painel")]
        public int CdPainel { get; set; }
        [Display(Name = "Painel")]
        public string NoPainel { get; set; }
        [Display(Name = "URL do Painel")]
        public string TxUrlPainel { get; set; }
        [Display(Name = "Tipo de Painel")]
        public int CdPainelTipo { get; set; }
        [Display(Name = "Status")]
        public string SnAtivo { get; set; }
        [Display(Name = "Usuário que cadastrou")]
        public int CdUsuarioCadastrou { get; set; }
        [Display(Name = "Data de Cadastro")]
        public DateTime DtCadastro { get; set; }
        [Display(Name = "Usuário que alterou")]
        public int? CdUsuarioAlterou { get; set; }
        [Display(Name = "Data de Alteração")]
        public DateTime? DtAlteracao { get; set; }

        public PainelTipoMOD PainelTipo { get; set; } = new PainelTipoMOD();

        public List<PainelMOD> ListaPainel { get; set; } = new List<PainelMOD>();
        public int QtdTotalDeRegistros { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
