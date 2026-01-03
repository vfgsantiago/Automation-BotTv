using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Automation.BotTv.Model;
using Automation.BotTv.Repository;
using Automation.BotTv.UI.Web.Models;
using FuncoesParaProgramar;

namespace Automation.BotTv.UI.Web.Controllers
{
    [Authorize(Roles = "Admin,Comum")]
    public class HomeController : Controller
    {
        #region Repositorios
        private readonly PainelREP _repositorioPainel;
        private readonly PainelTipoREP _repositorioPainelTipo;
        private readonly PainelCampoREP _repositorioPainelCampo;
        private readonly AcessoREP _repositorioAcesso;
        private readonly PainelMaquinaREP _repositorioPainelMaquina;
        private readonly CampoREP _repositorioCampo;
        private readonly CampoAcaoREP _repositorioCampoAcao;
        private readonly CampoTipoREP _repositorioCampoTipo;
        private readonly MaquinaREP _repositorioMaquina;
        #endregion

        #region Contrutores
        public HomeController(PainelREP repositorioPainel,
            PainelTipoREP repositorioPainelTipo,
            PainelCampoREP repositorioPainelCampo,
            AcessoREP repositorioAcesso,
            PainelMaquinaREP repositorioPainelMaquina,
            CampoREP repositorioCampo,
            CampoAcaoREP repositorioCampoAcao,
            CampoTipoREP repositorioCampoTipo,
            MaquinaREP repositorioMaquina)
        {
            _repositorioPainel = repositorioPainel;
            _repositorioPainelTipo = repositorioPainelTipo;
            _repositorioPainelCampo = repositorioPainelCampo;
            _repositorioAcesso = repositorioAcesso;
            _repositorioPainelMaquina = repositorioPainelMaquina;
            _repositorioCampo = repositorioCampo;
            _repositorioCampoAcao = repositorioCampoAcao;
            _repositorioCampoTipo = repositorioCampoTipo;
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
        [HttpGet]
        public async Task<IActionResult> Index(string noPainel, int? cdPainelTipo)
        {
            var paineis = await _repositorioPainel.BuscarTodosComFiltro(noPainel, cdPainelTipo);
            var contagem = await _repositorioPainel.ContarComFiltro(noPainel, cdPainelTipo);
            var tiposPainel = await _repositorioPainelTipo.BuscarAtivos();

            var viewModel = new HomeViewMOD
            {
                Paineis = paineis.ToList(),
                TiposDePainel = tiposPainel.ToList(),
                NoPainelPesquisa = noPainel,
                CdPainelTipoPesquisa = cdPainelTipo ?? 0,
                QtdTotalDeRegistros = contagem
            };

            ViewBag.Titulo = "Painéis";
            return View(viewModel);
        }
        #endregion

        #region Detalhe
        public async Task<IActionResult> Detalhe(int cdPainel)
        {
            var painel = await _repositorioPainel.BuscarPorCodigo(cdPainel);
            if (painel == null)
            {
                TempData["Modal-Erro"] = "Painel não encontrado!";
                return RedirectToAction("Index");
            }

            var painelCampos = await _repositorioPainelCampo.BuscarCamposPorPainel(cdPainel);
            var painelMaquinas = await _repositorioPainelMaquina.BuscarPorPainel(cdPainel);
            var acesso = await _repositorioAcesso.BuscarPorPainel(cdPainel);

            var tiposDePainel = await _repositorioPainelTipo.BuscarAtivos();
            var camposDisponiveis = await _repositorioCampo.BuscarAtivos();
            var maquinasDisponiveis = await _repositorioMaquina.BuscarNaoVinculadas();
            var acoesDisponiveis = await _repositorioCampoAcao.BuscarAtivos();
            var tiposDeCampoDisponiveis = await _repositorioCampoTipo.BuscarAtivos();

            var viewModel = new DetalheViewMOD
            {
                Painel = painel,
                PainelCampos = painelCampos,
                PainelMaquinas = painelMaquinas,
                Acesso = acesso,
                TiposDePainelDisponiveis = tiposDePainel,
                CamposDisponiveis = camposDisponiveis,
                MaquinasDisponiveis = maquinasDisponiveis,
                TiposDeCampoDisponiveis = tiposDeCampoDisponiveis.ToList(),
                AcoesDeCampoDisponiveis = acoesDisponiveis.ToList()
            };

            return View(viewModel);
        }
        #endregion

        #region EditarPainel
        [HttpPost]
        public IActionResult EditarPainel(DetalheViewMOD dadosTela)
        {
            var painelMOD = new PainelMOD();
            painelMOD.NoPainel = dadosTela.Painel.NoPainel;
            painelMOD.TxUrlPainel = dadosTela.Painel.TxUrlPainel;
            painelMOD.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            painelMOD.DtAlteracao = DateTime.Now;
            painelMOD.CdPainelTipo = dadosTela.Painel.CdPainelTipo;
            painelMOD.CdPainel = dadosTela.Painel.CdPainel;

            var editou = _repositorioPainel.Editar(painelMOD);

            if (editou)
            {
                TempData["Modal-Sucesso"] = "Painel alterado com sucesso!";
                return RedirectToAction("Detalhe", "Home", new {cdPainel = painelMOD.CdPainel});
            }
            else
            {
                TempData["Modal-Erro"] = "Erro ao alterar Painel!";
                return View(dadosTela);
            }
        }
        #endregion

        #region Campo

        #region AdicionarCampo
        [HttpPost]
        public async Task<JsonResult> AdicionarCampo(PainelCampoMOD dados)
        {
            try
            {
                if (dados.NrOrdem <= 0)
                {
                    return Json(new { sucesso = false, mensagem = "O número da ordem deve ser maior que zero." });
                }
                await _repositorioPainelCampo.AjustarOrdem(dados.CdPainel, dados.NrOrdem);
                var cdCampoCriado = await _repositorioCampo.Salvar(dados.Campo);
                if (cdCampoCriado > 0)
                {
                    dados.SnCampoValidacao = "S";
                    var vinculoSucesso = await _repositorioPainelCampo.Vincular(cdCampoCriado, dados.CdPainel, dados.NrOrdem, dados.SnCampoValidacao);
                    if (vinculoSucesso)
                    {
                        TempData["Modal-Sucesso"] = "Campo cadastrado e vinculado com sucesso!";
                        return Json(new { sucesso = true, mensagem = "Campo cadastrado e vinculado com sucesso!" });
                    }
                }
                TempData["Modal-Erro"] = "Erro ao adicionar o campo. Tente novamente.";
                return Json(new { sucesso = false, mensagem = "Erro ao adicionar o campo. Tente novamente." });
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = "Ocorreu um erro no servidor. Verifique os logs.";
                return Json(new { sucesso = false, mensagem = "Ocorreu um erro no servidor. Verifique os logs." });
            }
        }
        #endregion

        #region SalvarCampo
        [HttpPost]
        public async Task<JsonResult> SalvarCampo(PainelCampoMOD dados)
        {
            try
            {
                var sucessoCampo = await _repositorioCampo.Salvar(dados.Campo);
                if (sucessoCampo != 0)
                {
                    dados.SnCampoValidacao = "S";
                    var sucessoVinculo = await _repositorioPainelCampo.Salvar(dados);

                    TempData["Modal-Sucesso"] = "Campo atualizado com sucesso!";
                    return Json(new { sucesso = true, mensagem = "Campo atualizado com sucesso!" });
                }
                TempData["Modal-Erro"] = "Erro ao atualizar o campo. Tente novamente.";
                return Json(new { sucesso = false, mensagem = "Erro ao atualizar o campo. Tente novamente." });
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = "Ocorreu um erro no servidor. Verifique os logs.";
                return Json(new { sucesso = false, mensagem = "Ocorreu um erro no servidor. Verifique os logs." });
            }
        }
        #endregion

        #region RemoverCampo
        [HttpPost]
        public async Task<JsonResult> RemoverCampo(int cdPainelCampo)
        {
            try
            {
                var dadosCampo = await _repositorioPainelCampo.BuscarPorCodigo(cdPainelCampo);
                if (dadosCampo == null)
                {
                    return Json(new { sucesso = false, mensagem = "Campo não encontrado." });
                }

                var sucessoRemocao = await _repositorioPainelCampo.Remover(cdPainelCampo);
                var sucessoExclusao = await _repositorioCampo.Remover(dadosCampo.CdCampo);

                if (sucessoRemocao)
                {
                    await _repositorioPainelCampo.RecalcularOrdem(dadosCampo.CdPainel, dadosCampo.NrOrdem);
                    TempData["Modal-Sucesso"] = "Campo removido com sucesso!";
                    return Json(new { sucesso = true, mensagem = "Campo removido com sucesso!" });
                }
                TempData["Modal-Erro"] = "Erro ao remover o campo. Tente novamente.";
                return Json(new { sucesso = false, mensagem = "Erro ao remover o campo. Tente novamente." });
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = "Ocorreu um erro no servidor. Verifique os logs!";
                return Json(new { sucesso = false, mensagem = "Ocorreu um erro no servidor. Verifique os logs." });
            }
        }
        #endregion

        #region ReordenarCampos
        [HttpPost]
        public async Task<JsonResult> ReordenarCampos(int painelId, List<int> campoIds)
        {
            try
            {
                var sucesso = await _repositorioPainelCampo.Reordenar(painelId, campoIds);
                if (sucesso)
                {
                    return Json(new { sucesso = true, mensagem = "Campos reordenados com sucesso!" });
                }
                return Json(new { sucesso = false, mensagem = "Erro ao reordenar os campos. Nenhuma alteração foi realizada." });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = "Ocorreu um erro no servidor. Verifique os logs." });
            }
        }
        #endregion

        #endregion

        #region Acesso

        #region SalvarAcesso
        [HttpPost]
        public async Task<IActionResult> SalvarAcesso(AcessoMOD dadosTela, int cdPainel)
        {
            try
            {
                dadosTela.TxSenhaCifrada = FuncaoCriptografia.Criptografar(dadosTela.TxSenhaCifrada);

                if (dadosTela.CdAcesso == 0)
                {
                    dadosTela.CdUsuarioCadastrou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
                }
                else
                {
                    dadosTela.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
                }

                var cdAcessoSalvo = await _repositorioAcesso.Salvar(dadosTela);
                var cdUsuario = (dadosTela.CdUsuarioCadastrou != null ? dadosTela.CdUsuarioCadastrou : dadosTela.CdUsuarioAlterou);

                if (cdAcessoSalvo > 0)
                {
                    var vinculoSucesso = await _repositorioAcesso.Vincular(cdPainel, cdAcessoSalvo, cdUsuario.Value);

                    TempData["Modal-Sucesso"] = "Credenciais de acesso salvas com sucesso!";
                    return RedirectToAction("Detalhe", new { cdPainel });
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao salvar as credenciais. Tente novamente.";
                    return RedirectToAction("Detalhe", new { cdPainel });
                }
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = "Ocorreu um erro ao salvar as credenciais. Verifique os logs.";
                return RedirectToAction("Detalhe", new { cdPainel });
            }
        }
        #endregion

        #endregion

        #region Maquina

        #region VincularMaquina
        [HttpPost]
        public async Task<JsonResult> VincularMaquina(int cdPainel, int cdMaquina)
        {
            try
            {
                var cdUsuario = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
                var sucesso = await _repositorioPainelMaquina.Vincular(cdPainel, cdMaquina, cdUsuario);

                if (sucesso)
                {
                    TempData["Modal-Sucesso"] = "Máquina vinculada com sucesso!";
                    return Json(new { sucesso = true, mensagem = "Máquina vinculada com sucesso!" });
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao vinculada máquina. Tente novamente!";
                    return Json(new { sucesso = false, mensagem = "Erro ao vincular máquina. Tente novamente." });
                }
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = "Erro ao vinculada máquina. Tente novamente!";
                return Json(new { sucesso = false, mensagem = "Ocorreu um erro no servidor. Verifique os logs." });
            }
        }
        #endregion

        #region DesvincularMaquina
        [HttpPost]
        public async Task<JsonResult> DesvincularMaquina(int cdPainel, int cdMaquina)
        {
            try
            {
                var sucesso = await _repositorioPainelMaquina.Desvincular(cdPainel, cdMaquina);

                if (sucesso)
                {
                    TempData["Modal-Sucesso"] = "Máquina desvinculada com sucesso!";
                    return Json(new { sucesso = true, mensagem = "Máquina desvinculada com sucesso!" });
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao desvincular máquina. Tente novamente!";
                    return Json(new { sucesso = false, mensagem = "Erro ao desvincular máquina. Tente novamente." });
                }
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = "Erro ao desvincular máquina. Tente novamente!";
                return Json(new { sucesso = false, mensagem = "Ocorreu um erro no servidor. Verifique os logs." });
            }
        }
        #endregion

        #region SalvarMaquina
        [HttpPost]
        public async Task<IActionResult> SalvarMaquina(MaquinaMOD dadosTela, int cdPainel)
        {
            try
            {
                var cdUsuario = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);

                if (dadosTela.CdMaquina > 0)
                {
                    dadosTela.CdUsuarioAlterou = cdUsuario;
                }
                else
                {
                    dadosTela.CdUsuarioCadastrou = cdUsuario;
                }

                var cdMaquinaSalva = await _repositorioMaquina.Salvar(dadosTela);

                if (cdMaquinaSalva > 0)
                {
                    TempData["Modal-Sucesso"] = "Máquina salva com sucesso!";
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao salvar a máquina. Tente novamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = "Ocorreu um erro ao salvar a máquina. Verifique os logs.";
            }
            return RedirectToAction("Detalhe", new { cdPainel = cdPainel });
        }
        #endregion

        #endregion

        #endregion
    }
}