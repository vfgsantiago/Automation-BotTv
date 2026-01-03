using Dapper;
using Oracle.ManagedDataAccess.Client;
using Automation.BotTv.Data;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class PainelREP
    {
        #region Conexao
        private readonly string _conexaoOracle;
        #endregion

        #region Construtor
        public PainelREP(AcessaDados acessaDados)
        {
            _conexaoOracle = acessaDados.conexaoOracle();
        }
        #endregion

        #region Metodos

        #region BuscarPorCodigo
        /// <summary>
        /// Busca o painel por código
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <returns></returns>
        public async Task<PainelMOD?> BuscarPorCodigo(int cdPainel)
        {
            PainelMOD painel = new PainelMOD();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT CD_PAINEL, 
                                           NO_PAINEL, 
                                           TX_URL_PAINEL, 
                                           CD_PAINEL_TIPO, 
                                           SN_ATIVO, 
                                           CD_USUARIO_CADASTROU, 
                                           DT_CADASTRO, 
                                           CD_USUARIO_ALTEROU, 
                                           DT_ALTERACAO 
                                      FROM BOT_TV_PAINEL 
                                     WHERE CD_PAINEL = :CdPainel";

                    painel = await con.QueryFirstOrDefaultAsync<PainelMOD>(query, new { CdPainel = cdPainel });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar painel por código.", ex);
                }
            }
            return painel;
        }
        #endregion

        #region BuscarAtivos
        /// <summary>
        /// Busca todos os painéis ativos
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<List<PainelMOD>> BuscarAtivos()
        {
            List<PainelMOD> lista = new List<PainelMOD>();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();
                    var query = @"SELECT CD_PAINEL               AS CdPainel,  
                                           NO_PAINEL             AS NoPainel, 
                                           TX_URL_PAINEL         AS TxUrlPainel, 
                                           CD_PAINEL_TIPO        AS CdPainelTipo, 
                                           SN_ATIVO              AS SnAtivo, 
                                           CD_USUARIO_CADASTROU  AS CdUsuarioCadastrou, 
                                           DT_CADASTRO           AS DtCadastro, 
                                           CD_USUARIO_ALTEROU    AS CdUsuarioAlterou, 
                                           DT_ALTERACAO          AS DtAlteracao           
                                      FROM BOT_TV_PAINEL 
                                     WHERE SN_ATIVO = 'S'";

                    lista = (await con.QueryAsync<PainelMOD>(query)).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar todos os painéis ativos.", ex);
                }
            }
            return lista;
        }
        #endregion

        #region BuscarPaginado
        /// <summary>
        /// Busca todos os paineis de acordo com os filtros de forma paginada
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="itensPorPagina"></param>
        /// <param name="filtro"></param>
        /// <param name="cdPainelTipo"></param>
        /// <param name="snAtivo"></param>
        /// <returns></returns>
        public async Task<PaginacaoResposta<PainelMOD>> BuscarPaginado(int pagina, int itensPorPagina, string? filtro, int? cdPainelTipo, string? snAtivo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                await con.OpenAsync();
                int offset = (pagina - 1) * itensPorPagina;

                var parametros = new DynamicParameters();
                parametros.Add("Offset", offset);
                parametros.Add("ItensPorPagina", itensPorPagina);

                string condicaoFiltro = ConstruirCondicaoFiltro(parametros, filtro, cdPainelTipo, snAtivo);

                var query = $@"SELECT P.CD_PAINEL               AS CdPainel,
                                           P.NO_PAINEL                AS NoPainel,
                                           P.TX_URL_PAINEL            AS TxUrlPainel,
                                           P.CD_PAINEL_TIPO           AS CdPainelTipo,
                                           P.SN_ATIVO                 AS SnAtivo,
                                           P.CD_USUARIO_CADASTROU     AS CdUsuarioCadastrou,
                                           P.DT_CADASTRO              AS DtCadastro,
                                           P.CD_USUARIO_ALTEROU       AS CdUsuarioAlterou,
                                           P.DT_ALTERACAO             AS DtAlteracao,
                                           T.NO_PAINEL_TIPO           AS NoPainelTipo
                                      FROM BOT_TV_PAINEL P, BOT_TV_PAINEL_TIPO T
                                     WHERE P.CD_PAINEL_TIPO = T.CD_PAINEL_TIPO
                                        {condicaoFiltro}
                                       ORDER BY P.DT_ALTERACAO DESC
                                          OFFSET :Offset ROWS FETCH NEXT :ItensPorPagina ROWS ONLY";

                var lista = (await con.QueryAsync<PainelMOD, PainelTipoMOD, PainelMOD>(
                    query,
                    (painel, painelTIpo) =>
                    {
                        painel.PainelTipo = painelTIpo;
                        return painel;
                    }, 
                    parametros,
                    splitOn: "NoPainelTipo")).ToList();

                var totalQuery = $@"SELECT COUNT(*)
                                      FROM BOT_TV_PAINEL P, BOT_TV_PAINEL_TIPO T
                                     WHERE P.CD_PAINEL_TIPO = T.CD_PAINEL_TIPO
                                        {condicaoFiltro}";

                int totalItens = await con.ExecuteScalarAsync<int>(totalQuery, parametros);
                return new PaginacaoResposta<PainelMOD>
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
        private string ConstruirCondicaoFiltro(DynamicParameters parametros, string? filtro, int? cdPainelTipo, string? snAtivo)
        {
            string condicaoFiltro = "";
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim().ToUpper();
                parametros.Add("Filtro", $"%{filtro}%");
                condicaoFiltro += @" AND UPPER(P.NO_PAINEL) LIKE UPPER(:Filtro)";
            }
            if (cdPainelTipo.HasValue)
            {
                parametros.Add("CdPainelTipo", cdPainelTipo.Value);
                condicaoFiltro += " AND P.CD_PAINEL_TIPO = :CdPainelTipo";
            }
            if (snAtivo != null)
            {
                parametros.Add("SnAtivo", snAtivo);
                condicaoFiltro += " AND P.SN_ATIVO = :SnAtivo";
            }

            return condicaoFiltro;
        }
        #endregion

        #region Cadastrar
        /// <summary>
        /// Cadastrar painel
        /// </summary>
        /// <param name="painelMOD"></param>
        /// <returns></returns>
        public bool Cadastrar(PainelMOD painelMOD)
        {
            bool cadastrou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO BOT_TV_PAINEL
                                                 (
                                                 NO_PAINEL,
                                                 TX_URL_PAINEL,
                                                 CD_PAINEL_TIPO,
                                                 SN_ATIVO,
                                                 CD_USUARIO_CADASTROU,
                                                 DT_CADASTRO
                                                 )
                                           VALUES
                                                 (
                                                 :NoPainel,
                                                 :TxUrlPainel,
                                                 :CdPainelTipo,
                                                 :SnAtivo,
                                                 :CdUsuarioCadastrou,
                                                 :DtCadastro
                                                 )";

                    var parametros = new DynamicParameters(painelMOD);

                    parametros.Add("NoPainel", painelMOD.NoPainel);
                    parametros.Add("TxUrlPainel", painelMOD.TxUrlPainel);
                    parametros.Add("CdPainelTipo", painelMOD.CdPainelTipo);
                    parametros.Add("SnAtivo", painelMOD.SnAtivo);
                    parametros.Add("CdUsuarioCadastrou", painelMOD.CdUsuarioCadastrou);
                    parametros.Add("DtCadastro", painelMOD.DtCadastro);

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
        /// Editar o painel
        /// </summary>
        /// <param name="painelMOD"></param>
        /// <returns></returns>
        public bool Editar(PainelMOD painelMOD)
        {
            bool editou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE BOT_TV_PAINEL
                                        SET NO_PAINEL = :NoPainel,
                                            TX_URL_PAINEL = :TxUrlPainel,
                                            CD_PAINEL_TIPO = :CdPainelTipo,
                                            CD_USUARIO_ALTEROU = :CdUsuarioAlterou,
                                            DT_ALTERACAO = :DtAlteracao
                                      WHERE CD_PAINEL = :CdPainel";

                    var parametros = new DynamicParameters(painelMOD);

                    parametros.Add("NoPainel", painelMOD.NoPainel);
                    parametros.Add("TxUrlPainel", painelMOD.TxUrlPainel);
                    parametros.Add("CdPainelTipo", painelMOD.CdPainelTipo);
                    parametros.Add("CdUsuarioAlterou", painelMOD.CdUsuarioAlterou);
                    parametros.Add("DtAlteracao", painelMOD.DtAlteracao);
                    parametros.Add("CdPainel", painelMOD.CdPainel);

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
        /// Altera o status do painel
        /// </summary>
        /// <param name="painelMOD"></param>
        /// <returns></returns>
        public bool AlterarStatus(PainelMOD painelMOD)
        {
            bool alterouStatus = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE BOT_TV_PAINEL
                                        SET
                                            SN_ATIVO = :SnAtivo,
                                            CD_USUARIO_ALTEROU = :CdUsuarioAlterou,
                                            DT_ALTERACAO = :DtAlteracao
                                      WHERE
                                            CD_PAINEL = :CdPainel";

                    var parametros = new DynamicParameters(painelMOD);

                    parametros.Add("SnAtivo", painelMOD.SnAtivo);
                    parametros.Add("CdUsuarioAlterou", painelMOD.CdUsuarioAlterou);
                    parametros.Add("DtAlteracao", painelMOD.DtAlteracao);
                    parametros.Add("CdPainel", painelMOD.CdPainel);

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

        #region BuscarTodosComFiltro
        /// <summary>
        /// Buscar todos os paineis com filtro
        /// </summary>
        /// <param name="nomePainel"></param>>
        /// <param name="cdPainelTipo"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PainelMOD>> BuscarTodosComFiltro(string nomePainel, int? cdPainelTipo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT P.CD_PAINEL      AS CdPainel,
                                                P.NO_PAINEL      AS NoPainel,
                                                P.TX_URL_PAINEL  AS TxUrlPainel,
                                                P.SN_ATIVO       AS SnAtivo,
                                                P.CD_PAINEL_TIPO AS CdPainelTipo,
                                                T.NO_PAINEL_TIPO AS NoPainelTipo
                                           FROM BOT_TV_PAINEL P, BOT_TV_PAINEL_TIPO T
                                          WHERE P.CD_PAINEL_TIPO = T.CD_PAINEL_TIPO
                                            AND P.SN_ATIVO = 'S'";

                    var parametros = new DynamicParameters();

                    if (!string.IsNullOrEmpty(nomePainel))
                    {
                        query += " AND UPPER(P.NO_PAINEL) LIKE UPPER(:NoPainelPesquisa) ";
                        parametros.Add("NoPainelPesquisa", "%" + nomePainel + "%");
                    }

                    if (cdPainelTipo.HasValue && cdPainelTipo.Value > 0)
                    {
                        query += " AND P.CD_PAINEL_TIPO = :CdPainelTipoPesquisa ";
                        parametros.Add("CdPainelTipoPesquisa", cdPainelTipo.Value);
                    }

                    query += " ORDER BY P.NO_PAINEL ";

                    var paineis = await con.QueryAsync<PainelMOD, PainelTipoMOD, PainelMOD>(
                        query,
                        (painel, tipo) => {
                            painel.PainelTipo = tipo;
                            return painel;
                        },
                        parametros,
                        splitOn: "NoPainelTipo"
                    );

                    return paineis;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar painéis com filtro.", ex);
                }
            }
        }
        #endregion

        #region ContarComFiltro
        /// <summary>
        /// Contar Paineis com filtros
        /// </summary>
        /// <param name="nomePainel"></param>>
        /// <param name="cdPainelTipo"></param>
        /// <returns></returns>
        public async Task<int> ContarComFiltro(string nomePainel, int? cdPainelTipo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                var query = @"SELECT COUNT(*) 
                                      FROM BOT_TV_PAINEL P 
                                     WHERE P.SN_ATIVO = 'S' ";
                var parametros = new DynamicParameters();
                if (!string.IsNullOrEmpty(nomePainel))
                {
                    query += " AND P.NO_PAINEL LIKE :NoPainelPesquisa ";
                    parametros.Add("NoPainelPesquisa", "%" + nomePainel + "%");
                }
                if (cdPainelTipo.HasValue && cdPainelTipo.Value > 0)
                {
                    query += " AND P.CD_PAINEL_TIPO = :CdPainelTipoPesquisa ";
                    parametros.Add("CdPainelTipoPesquisa", cdPainelTipo.Value);
                }
                return await con.ExecuteScalarAsync<int>(query, parametros);
            }
        }
        #endregion

        #endregion
    }
}
