using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Automation.BotTv.Model;
using Automation.BotTv.Repository;

namespace Automation.BotTv.Console
{
    public class AutomationService
    {
        #region Repositories and Services
        private readonly ApiService _serviceAPI;

        private IWebDriver? _driver;
        private WebDriverWait? _wait;
        #endregion

        #region Constructor
        public AutomationService(
            ApiService serviceAPI)
        {
            _serviceAPI = serviceAPI;
        }
        #endregion

        #region Methods
        public async Task Execute()
        {
            var paineis = await _serviceAPI.BuscarPaineisAtivos();

            var maquinaAtualId = Environment.MachineName.ToUpper();
            System.Console.WriteLine();
            System.Console.WriteLine($"ID da máquina atual: {maquinaAtualId}");
            System.Console.WriteLine();
            System.Console.WriteLine(@"  ____           ________      ___|\/\/\/|__");
            System.Console.WriteLine(@" | __ )  ___  __|___ __\ \    / |   _   _  |");
            System.Console.WriteLine(@" |  _ \ / _ \|__ __| |  \ \  / /|  |_| |_| |");
            System.Console.WriteLine(@" | |_) | (_) | | | | |   \ \/ / |    \_/   |");
            System.Console.WriteLine(@" |____/ \___/  |_| |_|    \__/   \________/  CARREGANDO...");

            foreach (var painel in paineis)
            {
                try
                {
                    var painelMaquina = await _serviceAPI.BuscarMaquinaPorPainelEMaquina(painel.CdPainel, maquinaAtualId);
                    if (painelMaquina == null)
                    {
                        System.Console.WriteLine();
                        System.Console.WriteLine($"O painel '{painel.NoPainel}' não está configurado para rodar nesta máquina. Pulando...");
                        continue;
                    }

                    bool sessaoValida = false;
                    if (_driver != null)
                    {
                        sessaoValida = await SessionValidationAsync(painel);
                    }

                    if (!sessaoValida)
                    {
                        System.Console.WriteLine();
                        System.Console.WriteLine($"Sessão do painel '{painel.NoPainel}' expirada ou não encontrada. Reiniciando o processo de automação...");
                        QuitBrowser();

                        await LoginAsync(painel, painelMaquina);
                    }
                    else
                    {
                        System.Console.WriteLine();
                        System.Console.WriteLine($"Sessão do painel '{painel.NoPainel}' está ativa.");
                    }
                }
                catch (WebDriverException ex)
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine($"A janela do Chrome foi fechada inesperadamente: {ex.Message}. Reiniciando o processo...");
                    QuitBrowser();
                    await Execute();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine($"Um erro fatal ocorreu para o painel '{painel.NoPainel}': {ex.Message}");
                    QuitBrowser();
                }
            }
        }
        #endregion

        #region Helpers

        #region SessionValidationAsync
        private async Task<bool> SessionValidationAsync(PainelMOD painel)
        {
            if (_driver == null)
            {
                return false;
            }

            try
            {
                var urlAtual = _driver.Url;
                var urlEsperada = painel.TxUrlPainel;

                System.Console.WriteLine();
                System.Console.WriteLine($"Validando URL do painel: '{painel.NoPainel}'");
                System.Console.WriteLine($"URL atual: {urlAtual}");
                System.Console.WriteLine($"URL esperada: {urlEsperada}");

                if (urlAtual.Contains(urlEsperada, StringComparison.OrdinalIgnoreCase))
                {
                    System.Console.WriteLine("URL de validação encontrada. Sessão ativa.");
                    return true;
                }

                System.Console.WriteLine("Validação falhou. Sessão inválida.");
                return false;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine();
                System.Console.WriteLine($"Erro durante a validação da sessão: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region LoginAsync
        private async Task LoginAsync(PainelMOD painel, PainelMaquinaMOD painelMaquina)
        {
            if (_driver == null)
            {
                System.Console.WriteLine("Iniciando browser Google Chrome...");

                var options = new ChromeOptions();
                options.AddArgument("--start-fullscreen");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-extensions");
                options.AddUserProfilePreference("credentials_enable_service", false);
                options.AddUserProfilePreference("profile.password_manager_enabled", false);
                options.AddExcludedArguments("enable-automation");
                options.AddAdditionalOption("useAutomationExtension", false);

                _driver = new ChromeDriver(options);
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
            }

            try
            {
                _driver.Navigate().GoToUrl(painel.TxUrlPainel);

                var painelCampos = await _serviceAPI.BuscarCamposPorPainel(painel.CdPainel);
                var acesso = await _serviceAPI.BuscarAcessoPorPainel(painelMaquina.CdPainel);
                if (acesso == null)
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine("Acesso não encontrado. Login não pode ser realizado.");
                    return;
                }

                foreach (var painelCampo in painelCampos)
                {
                    var campo = await _serviceAPI.BuscarCampoPorCodigo(painelCampo.CdCampo);
                    if (campo == null) continue;

                    IWebElement elemento = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(campo.TxPath)));

                    if (campo.CdAcao == 1 && campo.CdCampoTipo == 1)
                    {
                        System.Console.WriteLine();
                        System.Console.WriteLine($"Clicando em '{campo.NoCampo}'");
                        elemento.Click();
                    }
                    else if (campo.CdAcao == 2)
                    {
                        string valor = string.Empty;
                        if (campo.CdCampoTipo == 2)
                        {
                            valor = acesso.TxLogin;
                        }
                        else if (campo.CdCampoTipo == 3)
                        {
                            valor = FuncoesParaProgramar.FuncaoCriptografia.Descriptografar(acesso.TxSenhaCifrada);
                        }
                        System.Console.WriteLine();
                        System.Console.WriteLine($"Preenchendo '{campo.NoCampo}'");
                        elemento.SendKeys(valor);
                    }
                }
                if(painel.CdPainelTipo == 2)
                {
                    TableauIframeValidation();
                }
                System.Console.WriteLine();
                System.Console.WriteLine($"Login do painel '{painel.NoPainel}' realizado com sucesso!");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine();
                System.Console.WriteLine($"Erro durante o login do painel '{painel.NoPainel}': {ex.Message}");
                System.Console.WriteLine();
                QuitBrowser();
                throw;
            }
        }
        #endregion

        #region QuitBrowser
        public void QuitBrowser()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver.Dispose();
                _driver = null;
            }
        }
        #endregion

        #region TableauIframeValidation
        private void TableauIframeValidation()
        {
            System.Console.WriteLine("[IMPORTANTE] Painel do tipo Tableau identificado. Iniciando troca de contexto.");
            const string IframeXPath = "//*[@id='viz']/iframe";
            const string FullscreenXPath = "//*[@id='fullscreen']";

            try
            {
                _wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.XPath(IframeXPath)));
                System.Console.WriteLine("[SUCESSO] Trocado para o IFrame do Tableau.");

                IWebElement fullscreenButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(FullscreenXPath)));
                System.Console.WriteLine("Botão 'Fullscreen' está visível dentro do IFrame. Tentando clicar...");

                OpenQA.Selenium.Interactions.Actions actions = new OpenQA.Selenium.Interactions.Actions(_driver);
                actions.MoveToElement(fullscreenButton).Click().Perform();
                System.Console.WriteLine("[SUCESSO] Clicando no botão 'Fullscreen' dentro do IFrame.");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[ERRO] Problema ao interagir com IFrame do Tableau: {ex.Message}");
            }
        }
        #endregion

        #endregion

    }
}