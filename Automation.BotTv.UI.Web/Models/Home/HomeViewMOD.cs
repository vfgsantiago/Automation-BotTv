using Automation.BotTv.Model;

namespace Automation.BotTv.UI.Web.Models
{
    public class HomeViewMOD
    {
        public List<PainelMOD> Paineis { get; set; }
        public List<PainelTipoMOD> TiposDePainel { get; set; }
        public string NoPainelPesquisa { get; set; }
        public int CdPainelTipoPesquisa { get; set; }

        public int QtdTotalDeRegistros { get; set; }
    }
}
