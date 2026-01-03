namespace Automation.BotTv.Model;
public record ApiLogin
{
    /// <summary>
    /// CODIGO
    /// </summary>
    public int CdLogin { get; set; }

    /// <summary>
    /// CODIGO DA API
    /// </summary>
    public int CdApi { get; set; }

    /// <summary>
    /// LOGIN DE ACESSO
    /// </summary>
    public string? TxLogin { get; set; }

    /// <summary>
    /// SENHA DE ACESSO
    /// </summary>
    public string? TxSenha { get; set; }

    /// <summary>
    /// S/N
    /// </summary>
    public string? SnAtivo { get; set; }

    /// <summary>
    /// DATA DO CADASTRO
    /// </summary>
    public DateTime? DtCadastro { get; set; }

    /// <summary>
    /// CODIGO DO USUARIO DO CADASTRO
    /// </summary>
    public decimal? CdUsuario { get; set; }

    /// <summary>
    /// I - INTERNO / E- EXTERNO / T - TEMPORARIO
    /// </summary>
    public string? TpLogin { get; set; }
}
