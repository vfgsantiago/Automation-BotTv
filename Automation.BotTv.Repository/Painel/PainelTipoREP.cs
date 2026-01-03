using Dapper;
using Oracle.ManagedDataAccess.Client;
using Automation.BotTv.Data;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class PainelTipoREP
    {
        #region Conexao
        private readonly string _conexaoOracle;
        #endregion

        #region Construtor
        public PainelTipoREP(AcessaDados acessaDados)
        {
            _conexaoOracle = acessaDados.conexaoOracle();
        }
        #endregion

        #region Metodos

        #region BuscarPorCodigo
        /// <summary>
        /// Busca o tipo de painel por código
        /// </summary>
        /// <param name="cdPainelTipo"></param>
        /// <returns></returns>
        public async Task<PainelTipoMOD?> BuscarPorCodigo(int cdPainelTipo)
        {
            PainelTipoMOD painelTipo = new PainelTipoMOD();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT PT.CD_PAINEL_TIPO           AS CdPainelTipo, 
                                           PT.NO_PAINEL_TIPO               AS NoPainelTipo,
                                           PT.TX_OBSERVACAO                AS TxObservacao,
                                           PT.SN_ATIVO                     AS SnAtivo,
                                           PT.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                           PT.DT_CADASTRO                  AS DtCadastro,
                                           PT.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                           PT.DT_ALTERACAO                 AS DtAlteracao
                                      FROM BOT_TV_PAINEL_TIPO PT
                                     WHERE PT.CD_PAINEL_TIPO = :CdPainelTipo";

                    painelTipo = await con.QueryFirstOrDefaultAsync<PainelTipoMOD>(query, new { CdPainelTipo = cdPainelTipo });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar tipo de painel por código.", ex);
                }
            }
            return painelTipo;
        }
        #endregion

        #region BuscarAtivos
        /// <summary>
        /// Busca todos os tipos de painéis ativos
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<List<PainelTipoMOD>> BuscarAtivos()
        {
            List<PainelTipoMOD> lista = new List<PainelTipoMOD>();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();
                    var query = @"SELECT PT.CD_PAINEL_TIPO           AS CdPainelTipo, 
                                           PT.NO_PAINEL_TIPO               AS NoPainelTipo,
                                           PT.TX_OBSERVACAO                AS TxObservacao,
                                           PT.SN_ATIVO                     AS SnAtivo,
                                           PT.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                           PT.DT_CADASTRO                  AS DtCadastro,
                                           PT.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                           PT.DT_ALTERACAO                 AS DtAlteracao
                                      FROM BOT_TV_PAINEL_TIPO PT
                                     WHERE SN_ATIVO = 'S'";

                    lista = (await con.QueryAsync<PainelTipoMOD>(query)).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar todos os tipos de painéis ativos.", ex);
                }
            }
            return lista;
        }
        #endregion

        #region BuscarPaginado
        /// <summary>
        /// Busca todos os tipos de paineis de acordo com os filtros de forma paginada
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="itensPorPagina"></param>
        /// <param name="filtro"></param>
        /// <param name="snAtivo"></param>
        /// <returns></returns>
        public async Task<PaginacaoResposta<PainelTipoMOD>> BuscarPaginado(int pagina, int itensPorPagina, string? filtro, string? snAtivo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                await con.OpenAsync();
                int offset = (pagina - 1) * itensPorPagina;

                var parametros = new DynamicParameters();
                parametros.Add("Offset", offset);
                parametros.Add("ItensPorPagina", itensPorPagina);

                string condicaoFiltro = ConstruirCondicaoFiltro(parametros, filtro, snAtivo);

                var query = $@"SELECT PT.CD_PAINEL_TIPO              AS CdPainelTipo, 
                                            PT.NO_PAINEL_TIPO               AS NoPainelTipo,
                                            PT.TX_OBSERVACAO                AS TxObservacao,
                                            PT.SN_ATIVO                     AS SnAtivo,
                                            PT.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                            PT.DT_CADASTRO                  AS DtCadastro,
                                            PT.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                            PT.DT_ALTERACAO                 AS DtAlteracao
                                       FROM BOT_TV_PAINEL_TIPO PT
                                      WHERE 1=1
                                        {condicaoFiltro}
                                       ORDER BY PT.DT_ALTERACAO DESC
                                          OFFSET :Offset ROWS FETCH NEXT :ItensPorPagina ROWS ONLY";

                var lista = (await con.QueryAsync<PainelTipoMOD>(query, parametros)).ToList();
                var totalQuery = $@"SELECT COUNT(*)
                                            FROM BOT_TV_PAINEL_TIPO PT
                                           WHERE 1=1
                                        {condicaoFiltro}";

                int totalItens = await con.ExecuteScalarAsync<int>(totalQuery, parametros);
                return new PaginacaoResposta<PainelTipoMOD>
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
                condicaoFiltro += @" AND UPPER(PT.NO_PAINEL_TIPO ) LIKE UPPER(:Filtro)";
            }
            if (snAtivo != null)
            {
                parametros.Add("SnAtivo", snAtivo);
                condicaoFiltro += " AND PT.SN_ATIVO = :SnAtivo";
            }

            return condicaoFiltro;
        }
        #endregion

        #region Cadastrar
        /// <summary>
        /// Cadastrar o tipo de painel
        /// </summary>
        /// <param name="painelTipoMOD"></param>
        /// <returns></returns>
        public bool Cadastrar(PainelTipoMOD painelTipoMOD)
        {
            bool cadastrou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO BOT_TV_PAINEL_TIPO 
                                                 (NO_PAINEL_TIPO,
                                                 TX_OBSERVACAO,
                                                 SN_ATIVO,
                                                 CD_USUARIO_CADASTROU,
                                                 DT_CADASTRO)
                                           VALUES 
                                                 (NoPainelTipo,
                                                 TxObservacao,
                                                 SnAtivo,
                                                 CdUsuarioCadastrou,
                                                 DtCadastro)";

                    var parametros = new DynamicParameters(painelTipoMOD);

                    parametros.Add("NoPainelTipo", painelTipoMOD.NoPainelTipo);
                    parametros.Add("TxObservacao", painelTipoMOD.TxObservacao);
                    parametros.Add("SnAtivo", painelTipoMOD.SnAtivo);
                    parametros.Add("CdUsuarioCadastrou", painelTipoMOD.CdUsuarioCadastrou);
                    parametros.Add("DtCadastro", painelTipoMOD.DtCadastro);

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
        /// Editar o tipo de painel
        /// </summary>
        /// <param name="painelTipoMOD"></param>
        /// <returns></returns>
        public bool Editar(PainelTipoMOD painelTipoMOD)
        {
            bool editou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE BOT_TV_PAINEL_TIPO 
                                        SET NO_PAINEL_TIPO = :NoPainelTipo,
                                            TX_OBSERVACAO = :TxObservacao,
                                            CD_USUARIO_ALTEROU = :CdUsuarioAlterou,
                                            DT_ALTERACAO = :DtAlteracao
                                      WHERE CD_PAINEL_TIPO = :CdPainelTipo";

                    var parametros = new DynamicParameters(painelTipoMOD);

                    parametros.Add("NoPainelTipo", painelTipoMOD.NoPainelTipo);
                    parametros.Add("TxObservacao", painelTipoMOD.TxObservacao);
                    parametros.Add("SnAtivo", painelTipoMOD.SnAtivo);
                    parametros.Add("CdUsuarioAlterou", painelTipoMOD.CdUsuarioAlterou);
                    parametros.Add("DtAlteracao", painelTipoMOD.DtAlteracao);
                    parametros.Add("CdPainelTipo", painelTipoMOD.CdPainelTipo);

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
        /// Altera o status tipo de painel
        /// </summary>
        /// <param name="painelTipoMOD"></param>
        /// <returns></returns>
        public bool AlterarStatus(PainelTipoMOD painelTipoMOD)
        {
            bool alterouStatus = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE BOT_TV_PAINEL_TIPO
                                        SET SN_ATIVO = :SnAtivo,
                                            CD_USUARIO_ALTEROU = :CdUsuarioAlterou,
                                            DT_ALTERACAO = :DtAlteracao
                                      WHERE CD_PAINEL_TIPO = :CdPainelTipo";

                    var parametros = new DynamicParameters(painelTipoMOD);

                    parametros.Add("SnAtivo", painelTipoMOD.SnAtivo);
                    parametros.Add("CdUsuarioAlterou", painelTipoMOD.CdUsuarioAlterou);
                    parametros.Add("DtAlteracao", painelTipoMOD.DtAlteracao);
                    parametros.Add("CdPainelTipo", painelTipoMOD.CdPainelTipo);

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
