 using Dapper;
using Oracle.ManagedDataAccess.Client;
using Automation.BotTv.Data;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class PainelMaquinaREP
    {
        #region Conexao
        private readonly string _conexaoOracle;
        #endregion

        #region Construtor
        public PainelMaquinaREP(AcessaDados acessaDados)
        {
            _conexaoOracle = acessaDados.conexaoOracle();
        }
        #endregion

        #region Metodos

        #region BuscarPorPainelEMaquina
        /// <summary>
        /// Buscar por Painel e Maquina
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <param name="txIdMaquina"></param>
        /// <returns></returns>
        public async Task<PainelMaquinaMOD?> BuscarPorPainelEMaquina(int cdPainel, string txIdMaquina)
        {
            PainelMaquinaMOD bot = new PainelMaquinaMOD();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT 
                                               PM.CD_PAINEL          AS CdPainel,
                                               PM.CD_MAQUINA         AS CdMaquina,
                                               PM.CD_USUARIO_ALTEROU AS CdUsuarioAlterou,
                                               PM.DT_ALTERACAO       AS DtAlteracao
                                          FROM BOT_TV_PAINEL_MAQUINA PM, BOT_TV_MAQUINA M
                                         WHERE PM.CD_MAQUINA = M.CD_MAQUINA
                                           AND PM.CD_PAINEL = :CdPainel
                                           AND M.TX_ID_MAQUINA = :TxIdMaquina";

                    bot = await con.QueryFirstOrDefaultAsync<PainelMaquinaMOD>(query, new { CdPainel = cdPainel, TxIdMaquina = txIdMaquina });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar vinculo painel e maquina.", ex);
                }
            }
            return bot;
        }
        #endregion

        #region BuscarPorPainel
        /// <summary>
        /// Buscar por Painel
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PainelMaquinaMOD>> BuscarPorPainel(int cdPainel)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = @"SELECT PM.CD_PAINEL AS CdPainel,
                                     PM.CD_MAQUINA AS CdMaquina,
                                     PM.CD_USUARIO_ALTEROU AS CdUsuarioAlterou,
                                     PM.DT_ALTERACAO AS DtAlteracao,
                                     M.TX_ID_MAQUINA AS TxIdMaquina,
                                     M.NO_MAQUINA AS NoMaquina,
                                     M.SN_ATIVO AS SnAtivo
                              FROM BOT_TV_PAINEL_MAQUINA PM
                              INNER JOIN BOT_TV_MAQUINA M ON PM.CD_MAQUINA = M.CD_MAQUINA
                              WHERE PM.CD_PAINEL = :CdPainel
                              ORDER BY PM.DT_ALTERACAO";

                var painelMaquina = await con.QueryAsync<PainelMaquinaMOD, MaquinaMOD, PainelMaquinaMOD>(
                    query,
                    (painelMaquina, maquina) =>
                    {
                        painelMaquina.Maquina = maquina;
                        return painelMaquina;
                    },
                    new { CdPainel = cdPainel },
                    splitOn: "TxIdMaquina"
                );
                return painelMaquina;
            }
        }
        #endregion

        #region Vincular
        /// <summary>
        /// Vincular Painel e Maquina
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <param name="cdMaquina"></param>
        /// <param name="cdUsuario"></param>
        /// <returns></returns>
        public async Task<bool> Vincular(int cdPainel, int cdMaquina, int cdUsuario)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = @"INSERT INTO BOT_TV_PAINEL_MAQUINA (
                                                CD_PAINEL,
                                                CD_MAQUINA,
                                                CD_USUARIO_ALTEROU,
                                                DT_ALTERACAO)
                                            VALUES (
                                                :CdPainel,
                                                :CdMaquina,
                                                :CdUsuarioAlterou,
                                                SYSDATE)";

                var parametros = new
                {
                    CdPainel = cdPainel,
                    CdMaquina = cdMaquina,
                    CdUsuarioAlterou = cdUsuario
                };

                var rowsAffected = await con.ExecuteAsync(query, parametros);
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Desvincular
        /// <summary>
        /// Desvincular Painel e Maquina
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <param name="cdMaquina"></param>
        /// <returns></returns>
        public async Task<bool> Desvincular(int cdPainel, int cdMaquina)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = @"DELETE FROM BOT_TV_PAINEL_MAQUINA
                                     WHERE CD_PAINEL = :CdPainel
                                       AND CD_MAQUINA = :CdMaquina";
                var rowsAffected = await con.ExecuteAsync(query, new { CdPainel = cdPainel, CdMaquina = cdMaquina });
                return rowsAffected > 0;
            }
        }
        #endregion

        #endregion
    }
}
