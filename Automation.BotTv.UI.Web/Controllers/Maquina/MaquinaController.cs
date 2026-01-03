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
    public class MaquinaController : Controller
    {
        #region Repositorios
        private readonly MaquinaREP _repositorioMaquina;
        #endregion

        #region Contrutores
        public MaquinaController(MaquinaREP repositorioMaquina)
        {
            _repositorioMaquina = repositorioMaquina;
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

            var resultado = await _repositorioMaquina.BuscarPaginado(numeroPagina, _take, filtro, snAtivo);

            var viewMOD = new MaquinaViewMOD
            {
                ListaMaquina = resultado.Dados,
                QtdTotalDeRegistros = resultado.Paginacao.TotalItens,
                PaginaAtual = resultado.Paginacao.PaginaAtual,
                TotalPaginas = resultado.Paginacao.TotalPaginas
            };

            ViewBag.Filtro = filtro;
            ViewBag.SnAtivo = snAtivo;
            ViewBag.Titulo = "Maquina";
            return View("Index", viewMOD);
        }
        #endregion

        #region Cadastrar
        public IActionResult Cadastrar()
        {
            MaquinaMOD maquinaMOD = new MaquinaMOD();

            var viewMOD = maquinaMOD.Adapt<MaquinaViewMOD>();
            return View(viewMOD);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(MaquinaViewMOD dadosTela)
        {
            var maquinaMOD = dadosTela.Adapt<MaquinaMOD>();
            maquinaMOD.TxIdMaquina = dadosTela.TxIdMaquina.ToUpper();
            maquinaMOD.NoMaquina = dadosTela.NoMaquina;
            maquinaMOD.SnAtivo = "S";
            maquinaMOD.CdUsuarioCadastrou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            maquinaMOD.DtCadastro = DateTime.Now;

            var maquina = await _repositorioMaquina.BuscarPorId(maquinaMOD.TxIdMaquina);
            if(maquina != null)
            {
                TempData["Modal-Erro"] = "Máquina já cadastrada!";
                return View(dadosTela);
            }

            var cadastrou = _repositorioMaquina.Cadastrar(maquinaMOD);

            if (cadastrou)
            {
                TempData["Modal-Sucesso"] = "Máquina cadastrada com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao cadastrar Máquina!";
                return View(dadosTela);
            }
        }
        #endregion

        #region Editar
        public async Task<IActionResult> Editar(int cdMaquina)
        {
            var maquinaMOD = await _repositorioMaquina.BuscarPorCodigo(cdMaquina);
            if (maquinaMOD == null)
            {
                TempData["Modal-Erro"] = "Máquina não encontrada!";
                return RedirectToAction("Index");
            }
            var viewMOD = maquinaMOD.Adapt<MaquinaViewMOD>();
            return View(viewMOD);
        }

        [HttpPost]
        public IActionResult Editar(MaquinaViewMOD dadosTela)
        {
            var maquinaMOD = dadosTela.Adapt<MaquinaMOD>();
            maquinaMOD.TxIdMaquina = dadosTela.TxIdMaquina.ToUpper();
            maquinaMOD.NoMaquina = dadosTela.NoMaquina;
            maquinaMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            maquinaMOD.DtAlteracao = DateTime.Now;
            maquinaMOD.CdMaquina = dadosTela.CdMaquina;

            var editou = _repositorioMaquina.Editar(maquinaMOD);

            if (editou)
            {
                TempData["Modal-Sucesso"] = "Máquina alterado com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao alterar Máquina!";
                return View(dadosTela);
            }
        }
        #endregion

        #region AlterarStatus
        [HttpPost]
        public async Task<IActionResult> AlterarStatus(int cdMaquina)
        {
            var maquinaMOD = await _repositorioMaquina.BuscarPorCodigo(cdMaquina);
            if (maquinaMOD == null)
            {
                TempData["Modal-Erro"] = "Tipo de Painel não encontrado!";
                return RedirectToAction("Index");
            }

            maquinaMOD.SnAtivo = maquinaMOD.SnAtivo == "S" ? "N" : "S";
            maquinaMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            maquinaMOD.DtAlteracao = DateTime.Now;

            var alterouStatus = _repositorioMaquina.AlterarStatus(maquinaMOD);

            if (alterouStatus)
                TempData["Modal-Sucesso"] = $"Máquina {(maquinaMOD.SnAtivo == "S" ? "ativada" : "desativada")} com sucesso!";
            else
                TempData["Modal-Erro"] = "Erro ao alterar status da Máquina. Tente novamente.";

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
