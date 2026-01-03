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
    public class CampoAcaoController : Controller
    {
        #region Repositorios
        private readonly CampoAcaoREP _repositorioCampoAcao;
        #endregion

        #region Contrutores
        public CampoAcaoController(CampoAcaoREP repositorioCampoAcao)
        {
            _repositorioCampoAcao = repositorioCampoAcao;
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

            var resultado = await _repositorioCampoAcao.BuscarPaginado(numeroPagina, _take, filtro, snAtivo);

            var viewMOD = new CampoAcaoViewMOD
            {
                ListaCampoAcao = resultado.Dados,
                QtdTotalDeRegistros = resultado.Paginacao.TotalItens,
                PaginaAtual = resultado.Paginacao.PaginaAtual,
                TotalPaginas = resultado.Paginacao.TotalPaginas
            };

            ViewBag.Filtro = filtro;
            ViewBag.SnAtivo = snAtivo;
            ViewBag.Titulo = "Ação do Campo";
            return View("Index", viewMOD);
        }
        #endregion

        #region Cadastrar
        public IActionResult Cadastrar()
        {
            CampoAcaoMOD campoAcaoMOD = new CampoAcaoMOD();

            var viewMOD = campoAcaoMOD.Adapt<CampoAcaoViewMOD>();
            return View(viewMOD);
        }

        [HttpPost]
        public IActionResult Cadastrar(CampoAcaoViewMOD dadosTela)
        {
            var campoAcaoMOD = dadosTela.Adapt<CampoAcaoMOD>();
            campoAcaoMOD.NoAcao = dadosTela.NoAcao;
            campoAcaoMOD.TxObservacao = dadosTela.TxObservacao;
            campoAcaoMOD.SnAtivo = "S";
            campoAcaoMOD.CdUsuarioCadastrou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            campoAcaoMOD.DtCadastro = DateTime.Now;

            var cadastrou = _repositorioCampoAcao.Cadastrar(campoAcaoMOD);

            if (cadastrou)
            {
                TempData["Modal-Sucesso"] = "Ação do Campo cadastrado com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao cadastrar Ação do Campo!";
                return View(dadosTela);
            }
        }
        #endregion

        #region Editar
        public async Task<IActionResult> Editar(int cdAcao)
        {
            var campoAcaoMOD = await _repositorioCampoAcao.BuscarPorCodigo(cdAcao);
            if (campoAcaoMOD == null)
            {
                TempData["Modal-Erro"] = "Ação do Campo não encontrado!";
                return RedirectToAction("Index");
            }
            var viewMOD = campoAcaoMOD.Adapt<CampoAcaoViewMOD>();
            return View(viewMOD);
        }

        [HttpPost]
        public IActionResult Editar(CampoAcaoViewMOD dadosTela)
        {
            var campoAcaoMOD = dadosTela.Adapt<CampoAcaoMOD>();
            campoAcaoMOD.NoAcao = dadosTela.NoAcao;
            campoAcaoMOD.TxObservacao = dadosTela.TxObservacao;
            campoAcaoMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            campoAcaoMOD.DtAlteracao = DateTime.Now;
            campoAcaoMOD.CdAcao = dadosTela.CdAcao;

            var editou = _repositorioCampoAcao.Editar(campoAcaoMOD);

            if (editou)
            {
                TempData["Modal-Sucesso"] = "Ação do Campo alterado com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao alterar Ação do Campo!";
                return View(dadosTela);
            }
        }
        #endregion

        #region AlterarStatus
        [HttpPost]
        public async Task<IActionResult> AlterarStatus(int cdAcao)
        {
            var campoAcaoMOD = await _repositorioCampoAcao.BuscarPorCodigo(cdAcao);
            if (campoAcaoMOD == null)
            {
                TempData["Modal-Erro"] = "Ação do Campo não encontrado!";
                return RedirectToAction("Index");
            }

            campoAcaoMOD.SnAtivo = campoAcaoMOD.SnAtivo == "S" ? "N" : "S";
            campoAcaoMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            campoAcaoMOD.DtAlteracao = DateTime.Now;

            var alterouStatus = _repositorioCampoAcao.AlterarStatus(campoAcaoMOD);

            if (alterouStatus)
                TempData["Modal-Sucesso"] = $"Ação do Campo {(campoAcaoMOD.SnAtivo == "S" ? "ativado" : "desativado")} com sucesso!";
            else
                TempData["Modal-Erro"] = "Erro ao alterar status do Ação do Campo. Tente novamente.";

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
