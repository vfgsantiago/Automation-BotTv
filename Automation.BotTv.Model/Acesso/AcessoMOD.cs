using System.ComponentModel.DataAnnotations;

namespace Automation.BotTv.Model
{
    public class AcessoMOD
    {
        public int CdAcesso { get; set; }
        [Display(Name = "E-mail de Acesso")]
        public string TxLogin { get; set; }
        [Display(Name = "Senha de Acesso")]
        public string TxSenhaCifrada { get; set; }
        public int CdUsuarioCadastrou { get; set; }
        public DateTime DtCadastro { get; set; }
        public int? CdUsuarioAlterou { get; set; }
        public DateTime DtAlteracao { get; set; }
    }
}
