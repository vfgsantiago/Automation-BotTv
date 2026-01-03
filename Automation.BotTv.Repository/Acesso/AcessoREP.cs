using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Automation.BotTv.Data;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class AcessoREP
    {
        #region Conexao
        private readonly string _conexaoOracle;
        #endregion

        #region Construtor
        public AcessoREP(AcessaDados acessaDados)
        {
            _conexaoOracle = acessaDados.conexaoOracle();
        }
        #endregion

        #region Metodos

        #region BuscarPorBotTv
        /// <summary>
        /// Busca o acesso por Bot TV
        /// </summary>
        /// <param name="cdBotTv"></param>
        /// <returns></returns>
        public async Task<AcessoMOD?> BuscarPorBotTv(int cdBotTv)
        {
            AcessoMOD acesso = new AcessoMOD();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT B.CD_ACESSO          AS CdAcesso, 
                                           B.CD_BOT_TV              AS CdBotTv, 
                                           A.TX_LOGIN               AS TxLogin, 
                                           A.TX_SENHA_CIFRADA       AS TxSenhaCifrada, 
                                           A.CD_USUARIO_CADASTROU   AS CdUsuarioCadastrou, 
                                           A.DT_CADASTRO            AS DtCadastro, 
                                           A.CD_USUARIO_ALTEROU     AS CdUsuarioAlterou, 
                                           A.DT_ALTERACAO           AS DtAlteracao 
                                      FROM BOT_TV_ACESSO A, BOT_TV B
                                     WHERE B.CD_ACESSO = A.CD_ACESSO
                                       AND B.CD_BOT_TV = :CdBotTv";

                    acesso = await con.QueryFirstOrDefaultAsync<AcessoMOD>(query, new { CdBotTv = cdBotTv });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar acesso por Bot TV.", ex);
                }
            }
            return acesso;
        }
        #endregion

        #region BuscarPorPainel
        /// <summary>
        /// Busca o acesso por Painel. Retorna null se não houver vínculo.
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <returns></returns>
        public async Task<AcessoMOD?> BuscarPorPainel(int cdPainel)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = @"SELECT A.CD_ACESSO AS CdAcesso,
                                            A.TX_LOGIN AS TxLogin,
                                            A.TX_SENHA_CIFRADA AS TxSenhaCifrada,
                                            A.DT_CADASTRO AS DtCadastro,
                                            A.CD_USUARIO_CADASTROU AS CdUsuarioCadastrou,
                                            A.DT_ALTERACAO AS DtAlteracao,
                                            A.CD_USUARIO_ALTEROU AS CdUsuarioAlterou
                                       FROM BOT_TV_PAINEL_ACESSO PA, BOT_TV_ACESSO A
                                      WHERE PA.CD_ACESSO = A.CD_ACESSO
                                        AND PA.CD_PAINEL = :CdPainel";

                return await con.QueryFirstOrDefaultAsync<AcessoMOD>(query, new { CdPainel = cdPainel });
            }
        }
        #endregion

        #region Salvar
        /// <summary>
        /// Salva (cadastra ou altera) o acesso
        /// </summary>
        /// <param name="acesso"></param>
        /// <returns></returns>
        public async Task<int> Salvar(AcessoMOD acesso)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = "";

                    if (acesso.CdAcesso > 0)
                    {
                        query = @"UPDATE BOT_TV_ACESSO
                                     SET TX_LOGIN = :TxLogin,
                                         TX_SENHA_CIFRADA = :TxSenhaCifrada,
                                         DT_ALTERACAO = SYSDATE,
                                         CD_USUARIO_ALTEROU = :CdUsuarioAlterou
                                   WHERE CD_ACESSO = :CdAcesso";

                        await con.ExecuteAsync(query, acesso);
                        return acesso.CdAcesso;
                    }
                    else
                    {
                        query = @"INSERT INTO BOT_TV_ACESSO 
                                              (TX_LOGIN, 
                                              TX_SENHA_CIFRADA, 
                                              DT_CADASTRO, 
                                              CD_USUARIO_CADASTROU)
                                        VALUES 
                                              (:TxLogin, 
                                              :TxSenhaCifrada, 
                                              SYSDATE, 
                                              :CdUsuarioCadastrou)
                                    RETURNING CD_ACESSO INTO :CdAcesso";
                    }

                    var p = new DynamicParameters(acesso);
                    p.Add("CdAcesso", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await con.ExecuteAsync(query, p);
                    return p.Get<int>("CdAcesso");
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao salvar as credenciais de acesso.", ex);
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
                var query = @"INSERT INTO BOT_TV_PAINEL_ACESSO 
                                                (CD_PAINEL, 
                                                CD_ACESSO, 
                                                CD_USUARIO_ALTEROU, 
                                                DT_ALTERACAO)
                                           VALUES 
                                                (:CdPainel, 
                                                :CdAcesso, 
                                                :CdUsuarioAlterou, 
                                                SYSDATE)";

                var parametros = new { CdPainel = cdPainel, CdAcesso = cdAcesso, CdUsuarioAlterou = cdUsuario };
                var rowsAffected = await con.ExecuteAsync(query, parametros);
                return rowsAffected > 0;
            }
        }
        #endregion

        #endregion
    }
}
