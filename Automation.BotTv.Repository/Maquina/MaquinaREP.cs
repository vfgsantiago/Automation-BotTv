using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Automation.BotTv.Data;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class MaquinaREP
    {
        #region Conexao
        private readonly string _conexaoOracle;
        #endregion

        #region Construtor
        public MaquinaREP(AcessaDados acessaDados)
        {
            _conexaoOracle = acessaDados.conexaoOracle();
        }
        #endregion

        #region Metodos

        #region BuscarPorCodigo
        /// <summary>
        /// Busca a maquina por código
        /// </summary>
        /// <param name="cdMaquina"></param>
        /// <returns></returns>
        public async Task<MaquinaMOD?> BuscarPorCodigo(int cdMaquina)
        {
            MaquinaMOD maquinaMOD = new MaquinaMOD();
            using (OracleConnection? con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT M.CD_MAQUINA                  AS CdMaquina, 
                                            M.TX_ID_MAQUINA               AS TxIdMaquina,
                                            M.NO_MAQUINA                  AS NoMaquina,
                                            M.SN_ATIVO                     AS SnAtivo,
                                            M.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                            M.DT_CADASTRO                  AS DtCadastro,
                                            M.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                            M.DT_ALTERACAO                 AS DtAlteracao
                                       FROM BOT_TV_MAQUINA M
                                     WHERE M.CD_MAQUINA = :CdMaquina";

                    maquinaMOD = await con.QueryFirstOrDefaultAsync<MaquinaMOD>(query, new { CdMaquina = cdMaquina });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar a maquina por código.", ex);
                }
            }
            return maquinaMOD;
        }
        #endregion

        #region BuscarPorId
        /// <summary>
        /// Busca a maquina pela sua identificação
        /// </summary>
        /// <param name="txIdMaquina"></param>
        /// <returns></returns>
        public async Task<MaquinaMOD?> BuscarPorId(string txIdMaquina)
        {
            MaquinaMOD maquinaMOD = new MaquinaMOD();
            using (OracleConnection? con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT M.CD_MAQUINA                  AS CdMaquina, 
                                            M.TX_ID_MAQUINA               AS TxIdMaquina,
                                            M.NO_MAQUINA                  AS NoMaquina,
                                            M.SN_ATIVO                     AS SnAtivo,
                                            M.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                            M.DT_CADASTRO                  AS DtCadastro,
                                            M.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                            M.DT_ALTERACAO                 AS DtAlteracao
                                       FROM BOT_TV_MAQUINA M
                                     WHERE M.TX_ID_MAQUINA = :TxIdMaquina";

                    maquinaMOD = await con.QueryFirstOrDefaultAsync<MaquinaMOD>(query, new { TxIdMaquina = txIdMaquina });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar a maquina pela identificação.", ex);
                }
            }
            return maquinaMOD;
        }
        #endregion

        #region BuscarAtivos
        /// <summary>
        /// Busca todas as maquinas ativas
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<List<MaquinaMOD>> BuscarAtivos()
        {
            List<MaquinaMOD> lista = new List<MaquinaMOD>();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();
                    var query = @"SELECT M.CD_MAQUINA                   AS CdMaquina, 
                                               M.TX_ID_MAQUINA                AS TxIdMaquina,
                                               M.NO_MAQUINA                  AS NoMaquina,
                                               M.SN_ATIVO                     AS SnAtivo,
                                               M.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                               M.DT_CADASTRO                  AS DtCadastro,
                                               M.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                               M.DT_ALTERACAO                 AS DtAlteracao
                                          FROM BOT_TV_MAQUINA M
                                        WHERE SN_ATIVO = 'S'";

                    lista = (await con.QueryAsync<MaquinaMOD>(query)).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar todas as máquinas ativas.", ex);
                }
            }
            return lista;
        }
        #endregion

        #region BuscarPaginado
        /// <summary>
        /// Busca todos as maquinas de acordo com os filtros de forma paginada
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="itensPorPagina"></param>
        /// <param name="filtro"></param>
        /// <param name="snAtivo"></param>
        /// <returns></returns>
        public async Task<PaginacaoResposta<MaquinaMOD>> BuscarPaginado(int pagina, int itensPorPagina, string? filtro, string? snAtivo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                await con.OpenAsync();
                int offset = (pagina - 1) * itensPorPagina;

                var parametros = new DynamicParameters();
                parametros.Add("Offset", offset);
                parametros.Add("ItensPorPagina", itensPorPagina);

                string condicaoFiltro = ConstruirCondicaoFiltro(parametros, filtro, snAtivo);

                var query = $@"SELECT M.CD_MAQUINA                  AS CdMaquina, 
                                            M.TX_ID_MAQUINA               AS TxIdMaquina,
                                            M.NO_MAQUINA                  AS NoMaquina,
                                            M.SN_ATIVO                     AS SnAtivo,
                                            M.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                            M.DT_CADASTRO                  AS DtCadastro,
                                            M.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                            M.DT_ALTERACAO                 AS DtAlteracao
                                       FROM BOT_TV_MAQUINA M
                                      WHERE 1=1
                                        {condicaoFiltro}
                                       ORDER BY M.DT_ALTERACAO DESC
                                          OFFSET :Offset ROWS FETCH NEXT :ItensPorPagina ROWS ONLY";

                var lista = (await con.QueryAsync<MaquinaMOD>(query, parametros)).ToList();
                var totalQuery = $@"SELECT COUNT(*)
                                            FROM BOT_TV_MAQUINA M
                                           WHERE 1=1
                                        {condicaoFiltro}";

                int totalItens = await con.ExecuteScalarAsync<int>(totalQuery, parametros);
                return new PaginacaoResposta<MaquinaMOD>
                {
                    Dados = lista,
                    Paginacao = new Paginacao
                    {
                        PaginaAtual = pagina,
                        QuantidadePorPagina = itensPorPagina,
                        TotalItens = totalItens,
                        TotalPaginas = (int)Math.Ceiling((double)totalItens / itensPorPagina)
                    }
                };
            }
        }
        private string ConstruirCondicaoFiltro(DynamicParameters parametros, string? filtro, string? snAtivo)
        {
            string condicaoFiltro = "";
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim().ToUpper();
                parametros.Add("Filtro", $"%{filtro}%");
                condicaoFiltro += @" AND (UPPER(M.TX_ID_MAQUINA) LIKE UPPER(:Filtro) OR UPPER(M.NO_MAQUINA) LIKE UPPER(:Filtro))";
            }
            if (snAtivo != null)
            {
                parametros.Add("SnAtivo", snAtivo);
                condicaoFiltro += " AND M.SN_ATIVO = :SnAtivo";
            }

            return condicaoFiltro;
        }
        #endregion

        #region Cadastrar
        /// <summary>
        /// Cadastrar a maquina
        /// </summary>
        /// <param name="maquinaMOD"></param>
        /// <returns></returns>
        public bool Cadastrar(MaquinaMOD maquinaMOD)
        {
            bool cadastrou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO BOT_TV_MAQUINA 
                                                 (TX_ID_MAQUINA,
                                                 NO_MAQUINA,
                                                 SN_ATIVO,
                                                 CD_USUARIO_CADASTROU,
                                                 DT_CADASTRO)
                                           VALUES 
                                                 (:TxIdMaquina,
                                                 :NoMaquina,
                                                 :SnAtivo,
                                                 :CdUsuarioCadastrou,
                                                 :DtCadastro)";

                    var parametros = new DynamicParameters(maquinaMOD);

                    parametros.Add("TxIdMaquina", maquinaMOD.TxIdMaquina);
                    parametros.Add("NoMaquina", maquinaMOD.NoMaquina);
                    parametros.Add("SnAtivo", maquinaMOD.SnAtivo);
                    parametros.Add("CdUsuarioCadastrou", maquinaMOD.CdUsuarioCadastrou);
                    parametros.Add("DtCadastro", maquinaMOD.DtCadastro);

                    con.Execute(query, parametros);
                    transacao.Commit();
                    cadastrou = true;
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                }
            }
            return cadastrou;
        }
        #endregion

        #region Editar
        /// <summary>
        /// Editar a maquina
        /// </summary>
        /// <param name="maquinaMOD"></param>
        /// <returns></returns>
        public bool Editar(MaquinaMOD maquinaMOD)
        {
            bool editou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE BOT_TV_MAQUINA
                                        SET TX_ID_MAQUINA = :TxIdMaquina,
                                            NO_MAQUINA = :NoMaquina,
                                            CD_USUARIO_ALTEROU = :CdUsuarioAlterou,
                                            DT_ALTERACAO = :DtAlteracao
                                      WHERE CD_MAQUINA = :CdMaquina";

                    var parametros = new DynamicParameters(maquinaMOD);

                    parametros.Add("TxIdMaquina", maquinaMOD.TxIdMaquina);
                    parametros.Add("NoMaquina", maquinaMOD.NoMaquina);
                    parametros.Add("SnAtivo", maquinaMOD.SnAtivo);
                    parametros.Add("CdUsuarioAlterou", maquinaMOD.CdUsuarioAlterou);
                    parametros.Add("DtAlteracao", maquinaMOD.DtAlteracao);
                    parametros.Add("CdMaquina", maquinaMOD.CdMaquina);

                    con.Execute(query, parametros);
                    transacao.Commit();
                    editou = true;
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                }
            }
            return editou;
        }
        #endregion

        #region AlterarStatus
        /// <summary>
        /// Altera o status da maquina
        /// </summary>
        /// <param name="maquinaMOD"></param>
        /// <returns></returns>
        public bool AlterarStatus(MaquinaMOD maquinaMOD)
        {
            bool alterouStatus = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE BOT_TV_MAQUINA
                                        SET SN_ATIVO = :SnAtivo,
                                            CD_USUARIO_ALTEROU = :CdUsuarioAlterou,
                                            DT_ALTERACAO = :DtAlteracao
                                      WHERE CD_MAQUINA = :CdMaquina";

                    var parametros = new DynamicParameters(maquinaMOD);

                    parametros.Add("SnAtivo", maquinaMOD.SnAtivo);
                    parametros.Add("CdUsuarioAlterou", maquinaMOD.CdUsuarioAlterou);
                    parametros.Add("DtAlteracao", maquinaMOD.DtAlteracao);
                    parametros.Add("CdMaquina", maquinaMOD.CdMaquina);

                    con.Execute(query, parametros);
                    transacao.Commit();
                    alterouStatus = true;
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                }
            }
            return alterouStatus;
        }
        #endregion

        #region BuscarNaoVinculadas
        /// <summary>
        /// Busca todas as máquinas sem vinculação a um painel
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<IEnumerable<MaquinaMOD>> BuscarNaoVinculadas()
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = @"SELECT M.CD_MAQUINA AS CdMaquina, 
                                           M.TX_ID_MAQUINA AS TxIdMaquina, 
                                           M.NO_MAQUINA AS NoMaquina
                                      FROM BOT_TV_MAQUINA M, BOT_TV_PAINEL_MAQUINA PM
                                     WHERE M.CD_MAQUINA = PM.CD_MAQUINA(+)
                                       AND M.SN_ATIVO = 'S'
                                       AND PM.CD_MAQUINA IS NULL
                                     ORDER BY M.TX_ID_MAQUINA";

                return await con.QueryAsync<MaquinaMOD>(query);
            }
        }
        #endregion

        #region Salvar
        /// <summary>
        /// Salva (cadastra ou altera) uma máquina.
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        public async Task<int> Salvar(MaquinaMOD dados)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                string query;
                if (dados.CdMaquina > 0) 
                {
                    query = @"UPDATE BOT_TV_MAQUINA
                                 SET TX_ID_MAQUINA = UPPER(:TxIdMaquina),
                                     NO_MAQUINA = :NoMaquina,
                                     SN_ATIVO = :SnAtivo,
                                     DT_ALTERACAO = SYSDATE,
                                     CD_USUARIO_ALTEROU = :CdUsuarioAlterou
                                WHERE CD_MAQUINA = :CdMaquina";
                    await con.ExecuteAsync(query, dados);
                    return dados.CdMaquina;
                }
                else
                {
                    query = @"INSERT INTO BOT_TV_MAQUINA 
                                     (TX_ID_MAQUINA, 
                                     NO_MAQUINA, 
                                     SN_ATIVO, 
                                     DT_CADASTRO, 
                                     CD_USUARIO_CADASTROU
                                VALUES 
                                     (UPPER(:TxIdMaquina), 
                                     :NoMaquina,
                                     :SnAtivo, 
                                     SYSDATE, 
                                     :CdUsuarioCadastrou)
                                     RETURNING CD_MAQUINA INTO :CdMaquina";

                    var parametros = new DynamicParameters(dados);
                    parametros.Add("CdMaquina", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await con.ExecuteAsync(query, parametros);
                    return parametros.Get<int>("CdMaquina");
                }
            }
        }
        #endregion

        #endregion
    }
}
