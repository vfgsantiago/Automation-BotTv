using Dapper;
using Oracle.ManagedDataAccess.Client;
using Automation.BotTv.Data;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class PainelAcessoREP
    {
        #region Conexao
        private readonly string _conexaoOracle;
        #endregion

        #region Construtor
        public PainelAcessoREP(AcessaDados acessaDados)
        {
            _conexaoOracle = acessaDados.conexaoOracle();
        }
        #endregion

        #region Metodos

        #region BuscarPorPainel
        /// <summary>
        /// Busca todos acessos por Painel.
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PainelAcessoMOD>> BuscarPorPainel(int cdPainel)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT 
                                               PA.CD_PAINEL          AS CdPainel,
                                               PA.CD_ACESSO          AS CdAcesso,
                                               PA.CD_USUARIO_ALTEROU AS CdUsuarioAlterou,
                                               PA.DT_ALTERACAO       AS DtAlteracao,
                                               A.TX_LOGIN            AS TxLogin,
                                               A.TX_SENHA_CIFRADA    AS TxSenhaCifrada
                                          FROM BOT_TV_PAINEL_ACESSO PA, BOT_TV_ACESSO A
                                         WHERE PA.CD_ACESSO = A.CD_ACESSO
                                           AND PA.CD_PAINEL = :CdPainel
                                         ORDER BY PA.DT_ALTERACAO";

                    var painelAcesso = await con.QueryAsync<PainelAcessoMOD, AcessoMOD, PainelAcessoMOD>(
                        query,
                        (painelAcesso, acesso) =>
                        {
                            painelAcesso.Acesso = acesso;
                            return painelAcesso;
                        },
                        new { CdPainel = cdPainel },
                        splitOn: "TxLogin"
                    );
                    return painelAcesso;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar acesso por painel.", ex);
                }
            }
        }
        #endregion

        #region Vincular
        /// <summary>
        /// Vincula um acesso a um painel
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <param name="cdAcesso"></param>
        /// <param name="cdUsuario"></param>
        /// <returns></returns>
        public async Task<bool> Vincular(int cdPainel, int cdAcesso, int cdUsuario)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"INSERT INTO BOT_TV_PAINEL_ACESSO 
                                                    (CD_PAINEL, 
                                                    CD_ACESSO, 
                                                    CD_USUARIO_ALTEROU, 
                                                    DT_ALTERACAO)
                                               VALUES
                                                    (:CdPainel, 
                                                    :CdMaquina, 
                                                    :CdUsuarioAlterou, 
                                                    SYSDATE)";

                    var rowsAffected = await con.ExecuteAsync(query, new { CdPainel = cdPainel, CdMaquina = cdAcesso, CdUsuario = cdUsuario });
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao vincular acesso ao painel.", ex);
                }
            }
        }
        #endregion

        #endregion
    }
}
