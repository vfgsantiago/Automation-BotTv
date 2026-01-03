using Automation.BotTv.Model;

namespace Automation.BotTv.UI.Web.Models
{
    public class DetalheViewMOD
    {
        public PainelMOD Painel { get; set; } = new PainelMOD();
        public AcessoMOD? Acesso { get; set; } 
        public PainelCampoMOD PainelCampo { get; set; } = new PainelCampoMOD();
        public CampoMOD Campo { get; set; } = new CampoMOD();

        public IEnumerable<PainelCampoMOD> PainelCampos { get; set; } = new List<PainelCampoMOD>();
        public IEnumerable<PainelMaquinaMOD> PainelMaquinas { get; set; } = new List<PainelMaquinaMOD>(); 

        public IEnumerable<PainelTipoMOD> TiposDePainelDisponiveis { get; set; } = new List<PainelTipoMOD>();
        public IEnumerable<CampoMOD> CamposDisponiveis { get; set; } = new List<CampoMOD>();
        public IEnumerable<MaquinaMOD> MaquinasDisponiveis { get; set; } = new List<MaquinaMOD>();
        public IEnumerable<CampoAcaoMOD> AcoesDeCampoDisponiveis { get; set; } = new List<CampoAcaoMOD>();
        public IEnumerable<CampoTipoMOD> TiposDeCampoDisponiveis { get; set; } = new List<CampoTipoMOD>();
    }
}