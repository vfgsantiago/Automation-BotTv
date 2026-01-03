using Dapper;
using Oracle.ManagedDataAccess.Client;
using Automation.BotTv.Data;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class CampoAcaoREP
    {
        #region Conexao
        private readonly string _conexaoOracle;
        #endregion

        #region Construtor
        public CampoAcaoREP(AcessaDados acessaDados)
        {
            _conexaoOracle = acessaDados.conexaoOracle();
        }
        #endregion

        #region Metodos

        #region BuscarPorCodigo
        /// <summary>
        /// Busca ação de campo por código
        /// </summary>
        /// <param name="cdAcao"></param>
        /// <returns></returns>
        public async Task<CampoAcaoMOD?> BuscarPorCodigo(int cdAcao)
        {
            CampoAcaoMOD painelTipo = new CampoAcaoMOD();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT CA.CD_ACAO                     AS CdAcao, 
                                            CA.NO_ACAO                      AS NoAcao,
                                            CA.TX_OBSERVACAO                AS TxObservacao,
                                            CA.SN_ATIVO                     AS SnAtivo,
                                            CA.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                            CA.DT_CADASTRO                  AS DtCadastro,
                                            CA.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                            CA.DT_ALTERACAO                 AS DtAlteracao
                                       FROM BOT_TV_CAMPO_ACAO CA
                                     WHERE CA.CD_ACAO = :CdAcao";

                    painelTipo = await con.QueryFirstOrDefaultAsync<CampoAcaoMOD>(query, new { CdAcao = cdAcao });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar ação de campo por código.", ex);
                }
            }
            return painelTipo;
        }
        #endregion

        #region BuscarAtivos
        /// <summary>
        /// Busca todos as ações de campo
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<List<CampoAcaoMOD>> BuscarAtivos()
        {
            List<CampoAcaoMOD> lista = new List<CampoAcaoMOD>();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();
                    var query = @"SELECT CA.CD_ACAO                   AS CdAcao, 
                                            CA.NO_ACAO                      AS NoAcao,
                                            CA.TX_OBSERVACAO                AS TxObservacao,
                                            CA.SN_ATIVO                     AS SnAtivo,
                                            CA.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                            CA.DT_CADASTRO                  AS DtCadastro,
                                            CA.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                            CA.DT_ALTERACAO                 AS DtAlteracao
                                       FROM BOT_TV_CAMPO_ACAO CA
                                     WHERE CA.SN_ATIVO = 'S'";

                    lista = (await con.QueryAsync<CampoAcaoMOD>(query)).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar todos as ações de campo ativos.", ex);
                }
            }
            return lista;
        }
        #endregion

        #region BuscarPaginado
        /// <summary>
        /// Busca todos ações dos campos de acordo com os filtros de forma paginada
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="itensPorPagina"></param>
        /// <param name="filtro"></param>
        /// <param name="snAtivo"></param>
        /// <returns></returns>
        public async Task<PaginacaoResposta<CampoAcaoMOD>> BuscarPaginado(int pagina, int itensPorPagina, string? filtro, string? snAtivo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                await con.OpenAsync();
                int offset = (pagina - 1) * itensPorPagina;

                var parametros = new DynamicParameters();
                parametros.Add("Offset", offset);
                parametros.Add("ItensPorPagina", itensPorPagina);

                string condicaoFiltro = ConstruirCondicaoFiltro(parametros, filtro, snAtivo);

                var query = $@"SELECT CA.CD_ACAO                     AS CdAcao, 
                                            CA.NO_ACAO                      AS NoAcao,
                                            CA.TX_OBSERVACAO                AS TxObservacao,
                                            CA.SN_ATIVO                     AS SnAtivo,
                                            CA.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                            CA.DT_CADASTRO                  AS DtCadastro,
                                            CA.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                            CA.DT_ALTERACAO                 AS DtAlteracao
                                       FROM BOT_TV_CAMPO_ACAO CA
                                      WHERE 1=1
                                        {condicaoFiltro}
                                       ORDER BY CA.DT_ALTERACAO DESC
                                          OFFSET :Offset ROWS FETCH NEXT :ItensPorPagina ROWS ONLY";

                var lista = (await con.QueryAsync<CampoAcaoMOD>(query, parametros)).ToList();
                var totalQuery = $@"SELECT COUNT(*)
                                            FROM BOT_TV_CAMPO_ACAO CA
                                           WHERE 1=1
                                        {condicaoFiltro}";

                int totalItens = await con.ExecuteScalarAsync<int>(totalQuery, parametros);
                return new PaginacaoResposta<CampoAcaoMOD>
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
                condicaoFiltro += @" AND UPPER(CA.NO_ACAO) LIKE UPPER(:Filtro)";
            }
            if (snAtivo != null)
            {
                parametros.Add("SnAtivo", snAtivo);
                condicaoFiltro += " AND CA.SN_ATIVO = :SnAtivo";
            }

            return condicaoFiltro;
        }
        #endregion

        #region Cadastrar
        /// <summary>
        /// Cadastrar a ação do campo
        /// </summary>
        /// <param name="campoAcaoMOD"></param>
        /// <returns></returns>
        public bool Cadastrar(CampoAcaoMOD campoAcaoMOD)
        {
            bool cadastrou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO BOT_TV_CAMPO_ACAO
                                                 (NO_ACAO,
                                                 TX_OBSERVACAO,
                                                 SN_ATIVO,
                                                 CD_USUARIO_CADASTROU,
                                                 DT_CADASTRO)
                                           VALUES 
                                                 (:NoAcao,
                                                 :TxObservacao,
                                                 :SnAtivo,
                                                 :CdUsuarioCadastrou,
                                                 :DtCadastro)";

                    var parametros = new DynamicParameters(campoAcaoMOD);

                    parametros.Add("NoAcao", campoAcaoMOD.NoAcao);
                    parametros.Add("TxObservacao", campoAcaoMOD.TxObservacao);
                    parametros.Add("SnAtivo", campoAcaoMOD.SnAtivo);
                    parametros.Add("CdUsuarioCadastrou", campoAcaoMOD.CdUsuarioCadastrou);
                    parametros.Add("DtCadastro", campoAcaoMOD.DtCadastro);

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
        /// Editar a ação do campo
        /// </summary>
        /// <param name="campoAcaoMOD"></param>
        /// <returns></returns>
        public bool Editar(CampoAcaoMOD campoAcaoMOD)
        {
            bool editou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE BOT_TV_CAMPO_ACAO
                                        SET NO_ACAO = :NoAcao,
                                            TX_OBSERVACAO = :TxObservacao,
                                            CD_USUARIO_ALTEROU = :CdUsuarioAlterou,
                                            DT_ALTERACAO = :DtAlteracao
                                      WHERE CD_ACAO = :CdAcao";

                    var parametros = new DynamicParameters(campoAcaoMOD);

                    parametros.Add("NoAcao", campoAcaoMOD.NoAcao);
                    parametros.Add("TxObservacao", campoAcaoMOD.TxObservacao);
                    parametros.Add("SnAtivo", campoAcaoMOD.SnAtivo);
                    parametros.Add("CdUsuarioAlterou", campoAcaoMOD.CdUsuarioAlterou);
                    parametros.Add("DtAlteracao", campoAcaoMOD.DtAlteracao);
                    parametros.Add("CdAcao", campoAcaoMOD.CdAcao);

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
        /// Altera o status da ação do campo
        /// </summary>
        /// <param name="campoAcaoMOD"></param>
        /// <returns></returns>
        public bool AlterarStatus(CampoAcaoMOD campoAcaoMOD)
        {
            bool alterouStatus = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE BOT_TV_CAMPO_ACAO
                                        SET SN_ATIVO = :SnAtivo,
                                            CD_USUARIO_ALTEROU = :CdUsuarioAlterou,
                                            DT_ALTERACAO = :DtAlteracao
                                      WHERE CD_ACAO = :CdAcao";

                    var parametros = new DynamicParameters(campoAcaoMOD);

                    parametros.Add("SnAtivo", campoAcaoMOD.SnAtivo);
                    parametros.Add("CdUsuarioAlterou", campoAcaoMOD.CdUsuarioAlterou);
                    parametros.Add("DtAlteracao", campoAcaoMOD.DtAlteracao);
                    parametros.Add("CdAcao", campoAcaoMOD.CdAcao);

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

        #endregion
    }
}
