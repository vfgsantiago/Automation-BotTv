using System.ComponentModel.DataAnnotations;
using Automation.BotTv.Model;

namespace Automation.BotTv.UI.Web.Models
{
    public class CampoTipoViewMOD
    {

        [Display(Name = "Tipo de Campo")]
        public int CdCampoTipo { get; set; }

        [Display(Name = "Tipo de Campo")]
        public string NoCampoTipo { get; set; }

        [Display(Name = "Observação")]
        public string TxObservacao { get; set; }

        [Display(Name = "Status")]
        public string SnAtivo { get; set; }

        [Display(Name = "Usuário que cadastrou")]
        public string CdUsuarioCadastrou { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DtCadastro { get; set; }

        [Display(Name = "Usuário que alterou")]
        public string? CdUsuarioAlterou { get; set; }

        [Display(Name = "Data de Alteração")]
        public DateTime? DtAlteracao { get; set; }

        public List<CampoTipoMOD> ListaCampoTipo { get; set; } = new List<CampoTipoMOD>();
        public int QtdTotalDeRegistros { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
