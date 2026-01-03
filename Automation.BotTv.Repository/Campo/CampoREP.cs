using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Automation.BotTv.Data;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class CampoREP
    {
        #region Conexao
        private readonly string _conexaoOracle;
        #endregion

        #region Construtor
        public CampoREP(AcessaDados acessaDados)
        {
            _conexaoOracle = acessaDados.conexaoOracle();
        }
        #endregion

        #region Metodos

        #region BuscarCampoPorCodigo
        /// <summary>
        /// Busca o campo por código
        /// </summary>
        /// <param name="cdCampo"></param>
        /// <returns></returns>
        public async Task<CampoMOD?> BuscarCampoPorCodigo(int cdCampo)
        {
            CampoMOD campo = new CampoMOD();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT C.CD_CAMPO        AS CdCampo, 
                                               C.CD_CAMPO_TIPO   AS CdCampoTipo,
                                               CT.NO_CAMPO_TIPO  AS NoCampoTipo,
                                               C.NO_CAMPO        AS NoCampo,
                                               C.TX_PATH         AS TxPath,
                                               C.CD_ACAO         AS CdAcao,
                                               CA.NO_ACAO        AS NoAcao
                                          FROM BOT_TV_CAMPO C, BOT_TV_CAMPO_TIPO CT, BOT_TV_CAMPO_ACAO CA
                                         WHERE C.CD_CAMPO_TIPO = CT.CD_CAMPO_TIPO
                                           AND C.CD_ACAO = CA.CD_ACAO
                                           AND C.SN_ATIVO = 'S'
                                           AND CD_CAMPO = :CdCampo";

                    campo = await con.QueryFirstOrDefaultAsync<CampoMOD>(query, new { CdCampo = cdCampo });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar campo por código.", ex);
                }
            }
            return campo;
        }
        #endregion

        #region BuscarAtivos
        /// <summary>
        /// Busca todos os campos ativos
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<List<CampoMOD>> BuscarAtivos()
        {
            List<CampoMOD> lista = new List<CampoMOD>();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();
                    var query = @"SELECT C.CD_CAMPO        AS CdCampo, 
                                               C.CD_CAMPO_TIPO   AS CdCampoTipo,
                                               CT.NO_CAMPO_TIPO  AS NoCampoTipo,
                                               C.NO_CAMPO        AS NoCampo,
                                               C.TX_PATH         AS TxPath,
                                               C.CD_ACAO         AS CdAcao,
                                               CA.NO_ACAO        AS NoAcao
                                          FROM BOT_TV_CAMPO C, BOT_TV_CAMPO_TIPO CT, BOT_TV_CAMPO_ACAO CA
                                         WHERE C.CD_CAMPO_TIPO = CT.CD_CAMPO_TIPO
                                           AND C.CD_ACAO = CA.CD_ACAO
                                           AND C.SN_ATIVO = 'S'";

                    lista = (await con.QueryAsync<CampoMOD>(query)).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar todos os campos ativos.", ex);
                }
            }
            return lista;
        }
        #endregion

        #region Salvar
        /// <summary>
        /// Salva (cadastra ou altera) um campo.
        /// </summary>
        /// <param name="campo"></param>
        /// <returns></returns>
        public async Task<int> Salvar(CampoMOD campo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                if (campo.CdCampo > 0)
                {
                    var query = @"UPDATE BOT_TV_CAMPO SET 
                                               NO_CAMPO = :NoCampo,
                                               TX_PATH = :TxPath,
                                               CD_ACAO = :CdAcao,
                                               CD_CAMPO_TIPO = :CdCampoTipo,
                                               DT_ALTERACAO = SYSDATE,
                                               CD_USUARIO_ALTEROU = :CdUsuarioAlterou
                                         WHERE CD_CAMPO = :CdCampo";
                    await con.ExecuteAsync(query, campo);
                    return campo.CdCampo;
                }
                else 
                {
                    var query = @"INSERT INTO BOT_TV_CAMPO 
                                                (NO_CAMPO, 
                                                TX_PATH, 
                                                CD_ACAO, 
                                                CD_CAMPO_TIPO,
                                                CD_USUARIO_CADASTROU)
                                          VALUES
                                                (:NoCampo, 
                                                :TxPath, 
                                                :CdAcao, 
                                                :CdCampoTipo, 
                                                :CdUsuarioCadastrou)
                                      RETURNING CD_CAMPO INTO :CdCampo";
                    var parametros = new DynamicParameters(campo);
                    parametros.Add("CdCampo", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await con.ExecuteAsync(query, parametros);
                    return parametros.Get<int>("CdCampo");
                }
            }
        }
        #endregion

        #region Remover
        /// <summary>
        /// Remove o campo
        /// </summary>
        /// <param name="cdCampo"></param>
        /// <returns></returns>
        public async Task<bool> Remover(int cdCampo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = "DELETE FROM BOT_TV_CAMPO WHERE CD_CAMPO = :CdCampo";
                var rowsAffected = await con.ExecuteAsync(query, new { CdCampo = cdCampo });
                return rowsAffected > 0;
            }
        }
        #endregion

        #endregion
    }
}
