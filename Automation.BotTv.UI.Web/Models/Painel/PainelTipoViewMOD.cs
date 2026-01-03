using System.ComponentModel.DataAnnotations;
using Automation.BotTv.Model;

namespace Automation.BotTv.UI.Web.Models
{
    public class PainelTipoViewMOD
    {
        [Display(Name = "Tipo de Painel")]
        public int CdPainelTipo { get; set; }

        [Display(Name = "Tipo de Painel")]
        public string NoPainelTipo { get; set; }

        [Display(Name = "Observação")]
        public string TxObservacao { get; set; }

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

        public List<PainelTipoMOD> ListaPainelTipo { get; set; } = new List<PainelTipoMOD>();
        public int QtdTotalDeRegistros { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
