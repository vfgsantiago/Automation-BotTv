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
    public class PainelController : Controller
    {
        #region Repositorios
        private readonly PainelREP _repositorioPainel;
        private readonly PainelTipoREP _repositorioPainelTipo;
        private readonly PainelCampoREP _repositorioPainelCampo;
        private readonly AcessoREP _repositorioAcesso;
        private readonly MaquinaREP _repositorioMaquina;
        private readonly CampoREP _repositorioCampo;
        private readonly CampoAcaoREP _repositorioCampoAcao;
        private readonly CampoTipoREP _repositorioCampoTipo;
        #endregion

        #region Contrutores
        public PainelController(PainelREP repositorioPainel,
            PainelTipoREP repositorioPainelTipo,
            PainelCampoREP repositorioPainelCampo,
            AcessoREP repositorioAcesso,
            MaquinaREP repositorioMaquina,
            CampoREP repositorioCampo,
            CampoAcaoREP repositorioCampoAcao,
            CampoTipoREP repositorioCampoTipo)
        {
            _repositorioPainel = repositorioPainel;
            _repositorioPainelTipo = repositorioPainelTipo;
            _repositorioPainelCampo = repositorioPainelCampo;
            _repositorioAcesso = repositorioAcesso;
            _repositorioMaquina = repositorioMaquina;
            _repositorioCampo = repositorioCampo;
            _repositorioCampoAcao = repositorioCampoAcao;
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
        public async Task<IActionResult> Index(int? pagina, string? filtro, int? cdPainelTipo, string? snAtivo)
        {
            CarregarSimNao(snAtivo);
            await CarregarPainelTipo(cdPainelTipo);
            int numeroPagina = pagina ?? 1;

            var resultado = await _repositorioPainel.BuscarPaginado(numeroPagina, _take, filtro, cdPainelTipo, snAtivo);

            var viewMOD = new PainelViewMOD
            {
                ListaPainel = resultado.Dados,
                QtdTotalDeRegistros = resultado.Paginacao.TotalItens,
                PaginaAtual = resultado.Paginacao.PaginaAtual,
                TotalPaginas = resultado.Paginacao.TotalPaginas
            };

            ViewBag.Filtro = filtro;
            ViewBag.PainelTipo = cdPainelTipo;
            ViewBag.SnAtivo = snAtivo;
            ViewBag.Titulo = "Painel";
            return View("Index", viewMOD);
        }
        #endregion

        #region Cadastrar
        public async Task<IActionResult> Cadastrar()
        {
            var painelMOD = new PainelMOD();

            var listaPainelTipo = await _repositorioPainelTipo.BuscarAtivos();
            ViewBag.ListaPainelTipo = new SelectList(listaPainelTipo, "CdPainelTipo", "NoPainelTipo");

            var viewMOD = painelMOD.Adapt<PainelViewMOD>();
            return View(viewMOD);
        }

        [HttpPost]
        public IActionResult Cadastrar(PainelViewMOD dadosTela)
        {
            var painelMOD = dadosTela.Adapt<PainelMOD>();
            painelMOD.NoPainel = dadosTela.NoPainel;
            painelMOD.TxUrlPainel = dadosTela.TxUrlPainel;
            painelMOD.CdPainelTipo = dadosTela.CdPainelTipo;
            painelMOD.SnAtivo = "S";
            painelMOD.CdUsuarioCadastrou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            painelMOD.DtCadastro = DateTime.Now;

            var cadastrou = _repositorioPainel.Cadastrar(painelMOD);

            if (cadastrou)
            {
                TempData["Modal-Sucesso"] = "Painel cadastrado com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao cadastrar Painel!";
                return View(dadosTela);
            }
        }
        #endregion

        #region Editar
        public async Task<IActionResult> Editar(int cdPainel)
        {
            var painelMOD = await _repositorioPainel.BuscarPorCodigo(cdPainel);
            if (painelMOD == null)
            {
                TempData["Modal-Erro"] = "Painel não encontrado!";
                return RedirectToAction("Index");
            }

            var listaPainelTipo = await _repositorioPainelTipo.BuscarAtivos();
            ViewBag.ListaPainelTipo = new SelectList(listaPainelTipo, "CdPainelTipo", "NoPainelTipo");

            var viewMOD = painelMOD.Adapt<PainelViewMOD>();
            return View(viewMOD);
        }

        [HttpPost]
        public IActionResult Editar(PainelViewMOD dadosTela)
        {
            var painelMOD = dadosTela.Adapt<PainelMOD>();
            painelMOD.NoPainel = dadosTela.NoPainel;
            painelMOD.TxUrlPainel = dadosTela.TxUrlPainel;
            painelMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            painelMOD.DtAlteracao = DateTime.Now;
            painelMOD.CdPainelTipo = dadosTela.CdPainelTipo;
            painelMOD.CdPainel = dadosTela.CdPainel;

            var editou = _repositorioPainel.Editar(painelMOD);

            if (editou)
            {
                TempData["Modal-Sucesso"] = "Painel alterado com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao alterar Painel!";
                return View(dadosTela);
            }
        }
        #endregion

        #region AlterarStatus
        [HttpPost]
        public async Task<IActionResult> AlterarStatus(int cdPainel)
        {
            var painelMOD = await _repositorioPainel.BuscarPorCodigo(cdPainel);
            if (painelMOD == null)
            {
                TempData["Modal-Erro"] = "Painel não encontrado!";
                return RedirectToAction("Index");
            }

            painelMOD.SnAtivo = painelMOD.SnAtivo == "S" ? "N" : "S";
            painelMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            painelMOD.DtAlteracao = DateTime.Now;

            var alterouStatus = _repositorioPainel.AlterarStatus(painelMOD);

            if (alterouStatus)
                TempData["Modal-Sucesso"] = $"Painel {(painelMOD.SnAtivo == "S" ? "ativado" : "desativado")} com sucesso!";
            else
                TempData["Modal-Erro"] = "Erro ao alterar status do Painel. Tente novamente.";

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

        #region CarregarPainelTipo
        public async Task CarregarPainelTipo(int? cdPainelTipo)
        {
            var listaPainelTipo = await _repositorioPainelTipo.BuscarAtivos();
            ViewBag.ListaPainelTipo = new SelectList(listaPainelTipo, "CdPainelTipo", "NoPainelTipo", cdPainelTipo);
        }
        #endregion

        #endregion

        #endregion
    }
}