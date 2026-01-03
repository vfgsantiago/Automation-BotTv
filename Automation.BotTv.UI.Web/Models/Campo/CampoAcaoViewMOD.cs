using System.ComponentModel.DataAnnotations;
using Automation.BotTv.Model;

namespace Automation.BotTv.UI.Web.Models
{
    public class CampoAcaoViewMOD
    {

        [Display(Name = "Ação do Campo")]
        public int CdAcao { get; set; }

        [Display(Name = "Ação do Campo")]
        public string NoAcao { get; set; }

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

        public List<CampoAcaoMOD> ListaCampoAcao { get; set; } = new List<CampoAcaoMOD>();
        public int QtdTotalDeRegistros { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
