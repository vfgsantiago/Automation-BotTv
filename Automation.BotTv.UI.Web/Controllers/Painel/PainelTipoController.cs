using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Automation.BotTv.Model;
using Automation.BotTv.Repository;
using Automation.BotTv.UI.Web.Models;

namespace Automation.BotTv.UI.Web.Controllers
{
    [Authorize(Roles = "Admin, Comum")]
    public class PainelTipoController : Controller
    {
        #region Repositorios
        private readonly PainelTipoREP _repositorioPainelTipo;
        #endregion

        #region Contrutores
        public PainelTipoController(PainelTipoREP repositorioPainelTipo)
        {
            _repositorioPainelTipo = repositorioPainelTipo;
        }
        #endregion

        #region Parametros
        private const int _take = 15;
        private int _numeroPagina = 1;
        private int _pagina;
        #endregion

        #region Metodos

        #region Index
        public async Task<IActionResult> Index(int? pagina, string? filtro, string? snAtivo)
        {
            CarregarSimNao(snAtivo);
            int numeroPagina = pagina ?? 1;

            var resultado = await _repositorioPainelTipo.BuscarPaginado(numeroPagina, _take, filtro, snAtivo);

            var viewMOD = new PainelTipoViewMOD
            {
                ListaPainelTipo = resultado.Dados,
                QtdTotalDeRegistros = resultado.Paginacao.TotalItens,
                PaginaAtual = resultado.Paginacao.PaginaAtual,
                TotalPaginas = resultado.Paginacao.TotalPaginas
            };

            ViewBag.Filtro = filtro;
            ViewBag.SnAtivo = snAtivo;
            ViewBag.Titulo = "Tipo de Painel";
            return View("Index", viewMOD);
        }
        #endregion

        #region Cadastrar
        public IActionResult Cadastrar()
        {
            PainelTipoMOD painelTipoMOD = new PainelTipoMOD();

            var viewMOD = painelTipoMOD.Adapt<PainelTipoViewMOD>();
            return View(viewMOD);
        }

        [HttpPost]
        public IActionResult Cadastrar(PainelTipoViewMOD dadosTela)
        {
            var painelTipoMOD = dadosTela.Adapt<PainelTipoMOD>();
            painelTipoMOD.NoPainelTipo = dadosTela.NoPainelTipo;
            painelTipoMOD.TxObservacao = dadosTela.TxObservacao;
            painelTipoMOD.SnAtivo = "S";
            painelTipoMOD.CdUsuarioCadastrou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            painelTipoMOD.DtCadastro = DateTime.Now;

            var cadastrou = _repositorioPainelTipo.Cadastrar(painelTipoMOD);

            if (cadastrou)
            {
                TempData["Modal-Sucesso"] = "Tipo de Painel cadastrado com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao cadastrar Tipo de Painel!";
                return View(dadosTela);
            }
        }
        #endregion

        #region Editar
        public async Task<IActionResult> Editar(int cdPainelTipo)
        {
            var painelTipoMOD = await _repositorioPainelTipo.BuscarPorCodigo(cdPainelTipo);
            if (painelTipoMOD == null)
            {
                TempData["Modal-Erro"] = "Tipo de Painel não encontrado!";
                return RedirectToAction("Index");
            }
            var viewMOD = painelTipoMOD.Adapt<PainelTipoViewMOD>();
            return View(viewMOD);
        }

        [HttpPost]
        public IActionResult Editar(PainelTipoViewMOD dadosTela)
        {
            var painelTipoMOD = dadosTela.Adapt<PainelTipoMOD>();
            painelTipoMOD.NoPainelTipo = dadosTela.NoPainelTipo;
            painelTipoMOD.TxObservacao = dadosTela.TxObservacao;
            painelTipoMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            painelTipoMOD.DtAlteracao = DateTime.Now;
            painelTipoMOD.CdPainelTipo = dadosTela.CdPainelTipo;

            var editou = _repositorioPainelTipo.Editar(painelTipoMOD);

            if (editou)
            {
                TempData["Modal-Sucesso"] = "Tipo de Painel alterado com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao alterar Tipo de Painel!";
                return View(dadosTela);
            }
        }
        #endregion

        #region AlterarStatus
        [HttpPost]
        public async Task<IActionResult> AlterarStatus(int cdPainelTipo)
        {
            var painelTipoMOD = await _repositorioPainelTipo.BuscarPorCodigo(cdPainelTipo);
            if (painelTipoMOD == null)
            {
                TempData["Modal-Erro"] = "Tipo de Painel não encontrado!";
                return RedirectToAction("Index");
            }

            painelTipoMOD.SnAtivo = painelTipoMOD.SnAtivo == "S" ? "N" : "S";
            painelTipoMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            painelTipoMOD.DtAlteracao = DateTime.Now;

            var alterouStatus = _repositorioPainelTipo.AlterarStatus(painelTipoMOD);

            if (alterouStatus)
                TempData["Modal-Sucesso"] = $"Tipo de Painel {(painelTipoMOD.SnAtivo == "S" ? "ativado" : "desativado")} com sucesso!";
            else
                TempData["Modal-Erro"] = "Erro ao alterar status do Tipo de Painel. Tente novamente.";

            return RedirectToAction("Index");
        }
        #endregion

        #region Auxiliar

        #region CarregarSimNao
        public void CarregarSimNao(string? snAtivo)
        {
            var lista = new[]
            {
                new SelectListItem{Value = "S", Text = "Ativo"},
                new SelectListItem {Value = "N", Text = "Inativo"}
            };
            ViewBag.ListaSimNao = new SelectList(lista, "Value", "Text", snAtivo);
        }

        #endregion

        #endregion

        #endregion
    }
}
