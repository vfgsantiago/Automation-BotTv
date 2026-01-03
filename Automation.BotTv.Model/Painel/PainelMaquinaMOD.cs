namespace Automation.BotTv.Model
{
    public class PainelMaquinaMOD
    {
        public int CdPainel { get; set; }
        public int CdMaquina { get; set; }
        public DateTime DtAlteracao { get; set; }
        public int CdUsuarioAlterou { get; set; }

        public PainelMOD Painel { get; set; } = new PainelMOD();
        public MaquinaMOD Maquina { get; set; } = new MaquinaMOD();
    }
}
