using Dapper;
using Oracle.ManagedDataAccess.Client;
using Automation.BotTv.Data;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class PainelCampoREP
    {
        #region Conexao
        private readonly string _conexaoOracle;
        #endregion

        #region Construtor
        public PainelCampoREP(AcessaDados acessaDados)
        {
            _conexaoOracle = acessaDados.conexaoOracle();
        }
        #endregion

        #region Metodos

        #region BuscarCamposPorPainel
        /// <summary>
        /// Busca os campos por painel
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <returns></returns>
        public async Task<List<PainelCampoMOD>> BuscarCamposPorPainel(int cdPainel)
        {
            List<PainelCampoMOD> lista = new List<PainelCampoMOD>();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT PC.CD_PAINEL_CAMPO       AS CdPainelCampo,
                                               PC.CD_CAMPO              AS CdCampo,
                                               PC.CD_PAINEL             AS CdPainel,
                                               PC.NR_ORDEM              AS NrOrdem,
                                               PC.SN_CAMPO_VALIDACAO    AS SnCampoValidacao,
                                               C.NO_CAMPO               AS NoCampo,
                                               C.TX_PATH                AS TxPath,
                                               C.CD_CAMPO_TIPO          AS CdCampoTipo,
                                               C.CD_ACAO                AS CdAcao,
                                               CT.NO_CAMPO_TIPO         AS NoCampoTipo,
                                               CA.NO_ACAO               AS NoAcao
                                          FROM BOT_TV_PAINEL_CAMPO PC, BOT_TV_CAMPO C, BOT_TV_CAMPO_TIPO CT, BOT_TV_CAMPO_ACAO CA
                                         WHERE PC.CD_CAMPO = C.CD_CAMPO
                                           AND C.CD_CAMPO_TIPO = CT.CD_CAMPO_TIPO
                                           AND C.CD_ACAO = CA.CD_ACAO
                                           AND PC.CD_PAINEL = :CdPainel
                                         ORDER BY NR_ORDEM";

                    lista = (await con.QueryAsync<PainelCampoMOD, CampoMOD, CampoTipoMOD, CampoAcaoMOD, PainelCampoMOD>(
                        query,
                        (painelCampo, campo, campoTipo, campoAcao) =>
                        {
                            painelCampo.Campo = campo;
                            painelCampo.Campo.CampoTipo = campoTipo;
                            painelCampo.Campo.CampoAcao = campoAcao;
                            return painelCampo;
                        },
                        new { CdPainel = cdPainel },
                        splitOn: "NoCampo, NoCampoTipo, NoAcao")).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar campos do painel.", ex);
                }
            }
            return lista;
        }
        #endregion

        #region BuscarPorCodigo
        /// <summary>
        /// Busca por código
        /// </summary>
        /// <param name="cdPainelCampo"></param>
        /// <returns></returns>
        public async Task<PainelCampoMOD> BuscarPorCodigo(int cdPainelCampo)
        {
            PainelCampoMOD paineCampo = new PainelCampoMOD();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT PC.CD_PAINEL_CAMPO       AS CdPainelCampo,
                                               PC.CD_CAMPO              AS CdCampo,
                                               PC.CD_PAINEL             AS CdPainel,
                                               PC.NR_ORDEM              AS NrOrdem,
                                               PC.SN_CAMPO_VALIDACAO    AS SnCampoValidacao,
                                               C.NO_CAMPO               AS NoCampo,
                                               C.TX_PATH                AS TxPath,
                                               CT.NO_CAMPO_TIPO         AS NoCampoTipo,
                                               CA.NO_ACAO               AS NoAcao
                                          FROM BOT_TV_PAINEL_CAMPO PC, BOT_TV_CAMPO C, BOT_TV_CAMPO_TIPO CT, BOT_TV_CAMPO_ACAO CA
                                         WHERE PC.CD_CAMPO = C.CD_CAMPO
                                           AND C.CD_CAMPO_TIPO = CT.CD_CAMPO_TIPO
                                           AND C.CD_ACAO = CA.CD_ACAO
                                           AND PC.CD_PAINEL_CAMPO = :CdPainelCampo
                                         ORDER BY NR_ORDEM";

                    paineCampo = (await con.QueryAsync<PainelCampoMOD, CampoMOD, CampoTipoMOD, CampoAcaoMOD, PainelCampoMOD>(
                        query,
                        (painelCampo, campo, campoTipo, campoAcao) =>
                        {
                            painelCampo.Campo = campo;
                            painelCampo.Campo.CampoTipo = campoTipo;
                            painelCampo.Campo.CampoAcao = campoAcao;
                            return painelCampo;
                        },
                        new { CdPainelCampo = cdPainelCampo },
                        splitOn: "NoCampo, NoCampoTipo, NoAcao")).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar por código.", ex);
                }
            }
            return paineCampo;
        }
        #endregion

        #region Vincular
        /// <summary>
        /// Vincular Campo ao Painel
        /// </summary>
        /// <param name="cdCampo"></param>
        /// <param name="cdPainel"></param>
        /// <param name="nrOrdem"></param>
        /// <param name="snCampoValidacao"></param>
        /// <returns></returns>
        public async Task<bool> Vincular(int cdCampo, int cdPainel, int nrOrdem, string snCampoValidacao)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = @"INSERT INTO BOT_TV_PAINEL_CAMPO 
                                                (CD_CAMPO, 
                                                 CD_PAINEL, 
                                                 NR_ORDEM, 
                                                 SN_CAMPO_VALIDACAO)
                                            VALUES
                                                 (:CdCampo, 
                                                 :CdPainel, 
                                                 :NrOrdem, 
                                                 :SnCampoValidacao)";

                var rowsAffected = await con.ExecuteAsync(query, new { CdCampo = cdCampo, CdPainel = cdPainel, NrOrdem = nrOrdem, SnCampoValidacao = snCampoValidacao });
                return rowsAffected > 0;
            }
        }
        #endregion

        #region AjustarOrdem
        /// <summary>
        /// Incrementa o NrOrdem de todos os campos a partir de um valor específico.
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <param name="nrOrdemInicial"></param>
        /// <returns></returns>
        public async Task<bool> AjustarOrdem(int cdPainel, int nrOrdemInicial)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = @"UPDATE BOT_TV_PAINEL_CAMPO
                                       SET NR_ORDEM = NR_ORDEM + 1
                                     WHERE CD_PAINEL = :CdPainel 
                                       AND NR_ORDEM >= :NrOrdemInicial";

                var rowsAffected = await con.ExecuteAsync(query, new { CdPainel = cdPainel, NrOrdemInicial = nrOrdemInicial });
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Salvar
        /// <summary>
        /// Salva (altera) a vinculação Painel-Campo.
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        public async Task<bool> Salvar(PainelCampoMOD dados)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = @"UPDATE BOT_TV_PAINEL_CAMPO
                                       SET NR_ORDEM = :NrOrdem,
                                           SN_CAMPO_VALIDACAO = :SnCampoValidacao
                                     WHERE CD_PAINEL_CAMPO = :CdPainelCampo";

                var rowsAffected = await con.ExecuteAsync(query, dados);
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Remover
        /// <summary>
        /// Remove uma vinculação Painel-Campo.
        /// </summary>
        /// <param name="cdPainelCampo"></param>
        /// <returns></returns>
        public async Task<bool> Remover(int cdPainelCampo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = "DELETE FROM BOT_TV_PAINEL_CAMPO WHERE CD_PAINEL_CAMPO = :CdPainelCampo";
                var rowsAffected = await con.ExecuteAsync(query, new { CdPainelCampo = cdPainelCampo });
                return rowsAffected > 0;
            }
        }
        #endregion

        #region RecalcularOrdem
        /// <summary>
        /// Decrementa a ordem dos campos após a remoção de um item.
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <param name="nrOrdemExcluida"></param>
        /// <returns></returns>
        public async Task<bool> RecalcularOrdem(int cdPainel, int nrOrdemExcluida)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = @"UPDATE BOT_TV_PAINEL_CAMPO
                                       SET NR_ORDEM = NR_ORDEM - 1
                                     WHERE CD_PAINEL = :CdPainel 
                                       AND NR_ORDEM > :NrOrdemExcluida";
                var rowsAffected = await con.ExecuteAsync(query, new { CdPainel = cdPainel, NrOrdemExcluida = nrOrdemExcluida });
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Reodernar
        /// <summary>
        /// Reordena os campos do painel com base em uma nova lista de IDs.
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <param name="cdsPainelCampo"></param>
        /// <returns></returns>
        public async Task<bool> Reordenar(int cdPainel, List<int> cdsPainelCampo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                await con.OpenAsync();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        var query = @"UPDATE BOT_TV_PAINEL_CAMPO
                                               SET NR_ORDEM = :NrOrdem
                                             WHERE CD_PAINEL = :CdPainel
                                               AND CD_PAINEL_CAMPO = :CdPainelCampo";

                        int nrOrdem = 1;
                        foreach (var cdPainelCampo in cdsPainelCampo)
                        {
                            await con.ExecuteAsync(query, new { NrOrdem = nrOrdem, CdPainel = cdPainel, CdPainelCampo = cdPainelCampo }, transaction);
                            nrOrdem++;
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}
