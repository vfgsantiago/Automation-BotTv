using Dapper;
using Oracle.ManagedDataAccess.Client;
using Automation.BotTv.Data;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class CampoTipoREP
    {
        #region Conexao
        private readonly string _conexaoOracle;
        #endregion

        #region Construtor
        public CampoTipoREP(AcessaDados acessaDados)
        {
            _conexaoOracle = acessaDados.conexaoOracle();
        }
        #endregion

        #region Metodos

        #region BuscarPorCodigo
        /// <summary>
        /// Busca o tipo de campo por código
        /// </summary>
        /// <param name="cdCampoTipo"></param>
        /// <returns></returns>
        public async Task<CampoTipoMOD?> BuscarPorCodigo(int cdCampoTipo)
        {
            CampoTipoMOD campoTipo = new CampoTipoMOD();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    var query = @"SELECT CT.CD_CAMPO_TIPO                AS CdCampoTipo, 
                                                CT.NO_CAMPO_TIPO                AS NoCampoTipo,
                                                CT.TX_OBSERVACAO                AS TxObservacao,
                                                CT.SN_ATIVO                     AS SnAtivo,
                                                CT.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                                CT.DT_CADASTRO                  AS DtCadastro,
                                                CT.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                                CT.DT_ALTERACAO                 AS DtAlteracao
                                           FROM BOT_TV_CAMPO_TIPO CT
                                         WHERE CT.CD_CAMPO_TIPO = :CdCampoTipo";

                    campoTipo = await con.QueryFirstOrDefaultAsync<CampoTipoMOD>(query, new { CdCampoTipo = cdCampoTipo });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar tipo de campo por código.", ex);
                }
            }
            return campoTipo;
        }
        #endregion

        #region BuscarAtivos
        /// <summary>
        /// Busca todos os tipos de campos ativos
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<List<CampoTipoMOD>> BuscarAtivos()
        {
            List<CampoTipoMOD> lista = new List<CampoTipoMOD>();
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();
                    var query = @"SELECT CT.CD_CAMPO_TIPO                AS CdCampoTipo, 
                                            CT.NO_CAMPO_TIPO                AS NoCampoTipo,
                                            CT.TX_OBSERVACAO                AS TxObservacao,
                                            CT.SN_ATIVO                     AS SnAtivo,
                                            CT.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                            CT.DT_CADASTRO                  AS DtCadastro,
                                            CT.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                            CT.DT_ALTERACAO                 AS DtAlteracao
                                       FROM BOT_TV_CAMPO_TIPO CT
                                      WHERE SN_ATIVO = 'S'";

                    lista = (await con.QueryAsync<CampoTipoMOD>(query)).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar todos os tipos de campos ativos.", ex);
                }
            }
            return lista;
        }
        #endregion

        #region BuscarPaginado
        /// <summary>
        /// Busca todos os tipos de campo de acordo com os filtros de forma paginada
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="itensPorPagina"></param>
        /// <param name="filtro"></param>
        /// <param name="snAtivo"></param>
        /// <returns></returns>
        public async Task<PaginacaoResposta<CampoTipoMOD>> BuscarPaginado(int pagina, int itensPorPagina, string? filtro, string? snAtivo)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                await con.OpenAsync();
                int offset = (pagina - 1) * itensPorPagina;

                var parametros = new DynamicParameters();
                parametros.Add("Offset", offset);
                parametros.Add("ItensPorPagina", itensPorPagina);

                string condicaoFiltro = ConstruirCondicaoFiltro(parametros, filtro, snAtivo);

                var query = $@"SELECT CT.CD_CAMPO_TIPO                AS CdCampoTipo, 
                                            CT.NO_CAMPO_TIPO                AS NoCampoTipo,
                                            CT.TX_OBSERVACAO                AS TxObservacao,
                                            CT.SN_ATIVO                     AS SnAtivo,
                                            CT.CD_USUARIO_CADASTROU         AS CdUsuarioCadastrou,
                                            CT.DT_CADASTRO                  AS DtCadastro,
                                            CT.CD_USUARIO_ALTEROU           AS CdUsuarioAlterou,
                                            CT.DT_ALTERACAO                 AS DtAlteracao
                                       FROM BOT_TV_CAMPO_TIPO CT
                                      WHERE 1=1
                                        {condicaoFiltro}
                                       ORDER BY CT.DT_ALTERACAO DESC
                                          OFFSET :Offset ROWS FETCH NEXT :ItensPorPagina ROWS ONLY";

                var lista = (await con.QueryAsync<CampoTipoMOD>(query, parametros)).ToList();
                var totalQuery = $@"SELECT COUNT(*)
                                             FROM BOT_TV_CAMPO_TIPO CT
                                            WHERE 1=1
                                        {condicaoFiltro}";

                int totalItens = await con.ExecuteScalarAsync<int>(totalQuery, parametros);
                return new PaginacaoResposta<CampoTipoMOD>
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
                condicaoFiltro += @" AND UPPER(CT.NO_CAMPO_TIPO ) LIKE UPPER(:Filtro)";
            }
            if (snAtivo != null)
            {
                parametros.Add("SnAtivo", snAtivo);
                condicaoFiltro += " AND CT.SN_ATIVO = :SnAtivo";
            }

            return condicaoFiltro;
        }
        #endregion

        #region Cadastrar
        /// <summary>
        /// Cadastrar o tipo de campo
        /// </summary>
        /// <param name="campoTipoMOD"></param>
        /// <returns></returns>
        public bool Cadastrar(CampoTipoMOD campoTipoMOD)
        {
            bool cadastrou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO BOT_TV_CAMPO_TIPO 
                                                 (NO_CAMPO_TIPO,
                                                 TX_OBSERVACAO,
                                                 SN_ATIVO,
                                                 CD_USUARIO_CADASTROU,
                                                 DT_CADASTRO)
                                           VALUES 
                                                 (:NoCampoTipo,
                                                 :TxObservacao,
                                                 :SnAtivo,
                                                 :CdUsuarioCadastrou,
                                                 :DtCadastro)";

                    var parametros = new DynamicParameters(campoTipoMOD);

                    parametros.Add("NoCampoTipo", campoTipoMOD.NoCampoTipo);
                    parametros.Add("TxObservacao", campoTipoMOD.TxObservacao);
                    parametros.Add("SnAtivo", campoTipoMOD.SnAtivo);
                    parametros.Add("CdUsuarioCadastrou", campoTipoMOD.CdUsuarioCadastrou);
                    parametros.Add("DtCadastro", campoTipoMOD.DtCadastro);

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
        /// Editar o tipo de campo
        /// </summary>
        /// <param name="campoTipoMOD"></param>
        /// <returns></returns>
        public bool Editar(CampoTipoMOD campoTipoMOD)
        {
            bool editou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE BOT_TV_CAMPO_TIPO 
                                        SET NO_CAMPO_TIPO = :NoCampoTipo,
                                            TX_OBSERVACAO = :TxObservacao,
                                            CD_USUARIO_ALTEROU = :CdUsuarioAlterou,
                                            DT_ALTERACAO = :DtAlteracao
                                      WHERE CD_CAMPO_TIPO = :CdCampoTipo";

                    var parametros = new DynamicParameters(campoTipoMOD);

                    parametros.Add("NoCampoTipo", campoTipoMOD.NoCampoTipo);
                    parametros.Add("TxObservacao", campoTipoMOD.TxObservacao);
                    parametros.Add("SnAtivo", campoTipoMOD.SnAtivo);
                    parametros.Add("CdUsuarioAlterou", campoTipoMOD.CdUsuarioAlterou);
                    parametros.Add("DtAlteracao", campoTipoMOD.DtAlteracao);
                    parametros.Add("CdCampoTipo", campoTipoMOD.CdCampoTipo);

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
        /// Altera o status tipo de campo
        /// </summary>
        /// <param name="campoTipoMOD"></param>
        /// <returns></returns>
        public bool AlterarStatus(CampoTipoMOD campoTipoMOD)
        {
            bool alterouStatus = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE BOT_TV_CAMPO_TIPO
                                        SET SN_ATIVO = :SnAtivo,
                                            CD_USUARIO_ALTEROU = :CdUsuarioAlterou,
                                            DT_ALTERACAO = :DtAlteracao
                                      WHERE CD_CAMPO_TIPO = :CdCampoTipo";

                    var parametros = new DynamicParameters(campoTipoMOD);

                    parametros.Add("SnAtivo", campoTipoMOD.SnAtivo);
                    parametros.Add("CdUsuarioAlterou", campoTipoMOD.CdUsuarioAlterou);
                    parametros.Add("DtAlteracao", campoTipoMOD.DtAlteracao);
                    parametros.Add("CdCampoTipo", campoTipoMOD.CdCampoTipo);

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
