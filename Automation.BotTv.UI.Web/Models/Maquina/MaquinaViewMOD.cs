using System.ComponentModel.DataAnnotations;
using Automation.BotTv.Model;

namespace Automation.BotTv.UI.Web.Models
{
    public class MaquinaViewMOD
    {

        [Display(Name = "Máquina")]
        public int CdMaquina { get; set; }

        [Display(Name = "Máquina")]
        public string TxIdMaquina { get; set; }

        [Display(Name = "Nome da Máquina")]
        public string NoMaquina { get; set; }

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

        public List<MaquinaMOD> ListaMaquina { get; set; } = new List<MaquinaMOD>();
        public int QtdTotalDeRegistros { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
