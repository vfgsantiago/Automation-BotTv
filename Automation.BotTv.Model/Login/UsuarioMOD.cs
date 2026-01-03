namespace Automation.BotTv.Model
{
    public partial class UsuarioMOD
    {
        public int CdUsuario { get; set; }
        public string NmUsuario { get; set; }

        public string TxEmail { get; set; }

        public string NrCpf { get; set; }

        public string SnAtivo { get; set; }

        public DateTime? DtCadastro { get; set; }

        public string? TxLogin { get; set; }

        public string? TxSenha { get; set; }

        public AvatarMOD Avatar { get; set; } = new AvatarMOD();
    }
}
