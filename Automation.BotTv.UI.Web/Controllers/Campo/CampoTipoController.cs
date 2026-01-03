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
    public class CampoTipoController : Controller
    {
        #region Repositorios
        private readonly CampoTipoREP _repositorioCampoTipo;
        #endregion

        #region Contrutores
        public CampoTipoController(CampoTipoREP repositorioCampoTipo)
        {
            _repositorioCampoTipo = repositorioCampoTipo;
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

            var resultado = await _repositorioCampoTipo.BuscarPaginado(numeroPagina, _take, filtro, snAtivo);

            var viewMOD = new CampoTipoViewMOD
            {
                ListaCampoTipo = resultado.Dados,
                QtdTotalDeRegistros = resultado.Paginacao.TotalItens,
                PaginaAtual = resultado.Paginacao.PaginaAtual,
                TotalPaginas = resultado.Paginacao.TotalPaginas
            };

            ViewBag.Filtro = filtro;
            ViewBag.SnAtivo = snAtivo;
            ViewBag.Titulo = "Tipo de Campo";
            return View("Index", viewMOD);
        }
        #endregion

        #region Cadastrar
        public IActionResult Cadastrar()
        {
            CampoTipoMOD campoTipoMOD = new CampoTipoMOD();

            var viewMOD = campoTipoMOD.Adapt<CampoTipoViewMOD>();
            return View(viewMOD);
        }

        [HttpPost]
        public IActionResult Cadastrar(CampoTipoViewMOD dadosTela)
        {
            var campoTipoMOD = dadosTela.Adapt<CampoTipoMOD>();
            campoTipoMOD.NoCampoTipo = dadosTela.NoCampoTipo;
            campoTipoMOD.TxObservacao = dadosTela.TxObservacao;
            campoTipoMOD.SnAtivo = "S";
            campoTipoMOD.CdUsuarioCadastrou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            campoTipoMOD.DtCadastro = DateTime.Now;

            var cadastrou = _repositorioCampoTipo.Cadastrar(campoTipoMOD);

            if (cadastrou)
            {
                TempData["Modal-Sucesso"] = "Tipo de Campo cadastrado com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao cadastrar Tipo de Campo!";
                return View(dadosTela);
            }
        }
        #endregion

        #region Editar
        public async Task<IActionResult> Editar(int cdCampoTipo)
        {
            var campoTipoMOD = await _repositorioCampoTipo.BuscarPorCodigo(cdCampoTipo);
            if (campoTipoMOD == null)
            {
                TempData["Modal-Erro"] = "Tipo de Campo não encontrado!";
                return RedirectToAction("Index");
            }
            var viewMOD = campoTipoMOD.Adapt<CampoTipoViewMOD>();
            return View(viewMOD);
        }

        [HttpPost]
        public IActionResult Editar(CampoTipoViewMOD dadosTela)
        {
            var campoTipoMOD = dadosTela.Adapt<CampoTipoMOD>();
            campoTipoMOD.NoCampoTipo = dadosTela.NoCampoTipo;
            campoTipoMOD.TxObservacao = dadosTela.TxObservacao;
            campoTipoMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            campoTipoMOD.DtAlteracao = DateTime.Now;
            campoTipoMOD.CdCampoTipo = dadosTela.CdCampoTipo;

            var editou = _repositorioCampoTipo.Editar(campoTipoMOD);

            if (editou)
            {
                TempData["Modal-Sucesso"] = "Tipo de Campo alterado com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao alterar Tipo de Campo!";
                return View(dadosTela);
            }
        }
        #endregion

        #region AlterarStatus
        [HttpPost]
        public async Task<IActionResult> AlterarStatus(int cdCampoTipo)
        {
            var campoTipoMOD = await _repositorioCampoTipo.BuscarPorCodigo(cdCampoTipo);
            if (campoTipoMOD == null)
            {
                TempData["Modal-Erro"] = "Tipo de Campo não encontrado!";
                return RedirectToAction("Index");
            }

            campoTipoMOD.SnAtivo = campoTipoMOD.SnAtivo == "S" ? "N" : "S";
            campoTipoMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            campoTipoMOD.DtAlteracao = DateTime.Now;

            var alterouStatus = _repositorioCampoTipo.AlterarStatus(campoTipoMOD);

            if (alterouStatus)
                TempData["Modal-Sucesso"] = $"Tipo de Campo {(campoTipoMOD.SnAtivo == "S" ? "ativado" : "desativado")} com sucesso!";
            else
                TempData["Modal-Erro"] = "Erro ao alterar status do Tipo de Campo. Tente novamente.";

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
