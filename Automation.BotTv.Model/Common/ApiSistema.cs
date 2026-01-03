namespace Automation.BotTv.Model;
public record ApiSistema
{
    public decimal CdApi { get; set; }

    /// <summary>
    /// NOME DA API
    /// </summary>
    public string? NmApi { get; set; }

    /// <summary>
    /// URL BASE
    /// </summary>
    public string? TxUrl { get; set; }

    /// <summary>
    /// DESCRICAO DA FINALIDADE
    /// </summary>
    public string? TxDescricao { get; set; }

    /// <summary>
    /// ROLE C- COMUM / A- ADMIN
    /// </summary>
    public string? TpAcesso { get; set; }

    /// <summary>
    /// I- INTERNO/E- EXTERNO
    /// </summary>
    public string? TpVisisibilidade { get; set; }

    /// <summary>
    /// DESCRICAO DO CODIGO USADO
    /// </summary>
    public string? TpVersao { get; set; }

    /// <summary>
    /// (P)POST,(T)PUT,(U)UPDATE,(D)DELETE
    /// </summary>
    public string? TpMetodos { get; set; }

    /// <summary>
    /// REST/SOAP
    /// </summary>
    public string? TpApi { get; set; }

    /// <summary>
    /// CHAVE,JWT,TOKEN
    /// </summary>
    public string? TpSeguranca { get; set; }

    /// <summary>
    /// S/N 
    /// </summary>
    public string? SnAtivo { get; set; }

    /// <summary>
    /// URL DE AUTENTICACAO
    /// </summary>
    public string? TxSecretKey { get; set; }

}
