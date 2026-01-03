namespace Automation.BotTv.Model
{
    public class MaquinaMOD
    {
        public int CdMaquina { get; set; }
        public string TxIdMaquina { get; set; }
        public string NoMaquina { get; set; }
        public string SnAtivo { get; set; }
        public int CdUsuarioCadastrou { get; set; }
        public DateTime DtCadastro { get; set; }
        public int? CdUsuarioAlterou { get; set; }
        public DateTime DtAlteracao { get; set; }
    }
}
