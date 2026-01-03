namespace Automation.BotTv.Model
{
    public class PainelAcessoMOD
    {
        public int CdPainel {  get; set; }
        public int CdAcesso { get; set; }
        public DateTime DtAlteracao { get; set; }
        public int CdUsuarioAlterou { get; set; }

        public PainelMOD Painel {  get; set; }  = new PainelMOD();
        public AcessoMOD Acesso {  get; set; } = new AcessoMOD();
    }
}
