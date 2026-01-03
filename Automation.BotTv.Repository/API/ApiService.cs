using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class ApiService
    {
        #region Servicos
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        #endregion

        #region Construtor
        public ApiService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("AutomationBotTvHttpClient");
            _httpClient.BaseAddress = new Uri(_configuration.GetSection("BaseUrl").Value);
            _logger = logger;
        }
        #endregion

        #region Metodos

        #region Painel

        #region BuscarPaineisAtivos
        /// <summary>
        /// Busca todos os paineis ativos
        /// </summary>
        /// <returns>List<PainelMOD>></returns>
        public async Task<List<PainelMOD>> BuscarPaineisAtivos()
        {
            List<PainelMOD> paineis = new List<PainelMOD>();
            string route = $"/Paineis";
            string url = _httpClient.BaseAddress + route;

            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = response.StatusCode;
                        _logger.LogInformation($"\n[ERRO HTTP] Falha na busca de painéis. Status Code: {response.StatusCode}");
                        _logger.LogInformation($"[DETALHES] URL: {url}");
                        _logger.LogInformation($"[RESPOSTA API] {errorContent}");
                        return new List<PainelMOD>();
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    paineis = JsonConvert.DeserializeObject<List<PainelMOD>>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"\n[ERRO GERAL] Exceção na chamada de painéis: {ex.Message}");
                return new List<PainelMOD>();
            }
            _logger.LogInformation($"\n[SUCESSO] {paineis.Count} painel(eis) recebido(s) da API.");
            return paineis;
        }
        #endregion

        #endregion

        #region Campo

        #region BuscarCampoPorCodigo
        /// <summary>
        /// Busca o campo pelo codigo
        /// </summary>
        /// <param name="cdCampo"></param>
        /// <returns>CampoMOD</returns>
        public async Task<CampoMOD> BuscarCampoPorCodigo(int cdCampo)
        {
            CampoMOD campo = new CampoMOD();
            string route = $"/Campos/{cdCampo}";
            string url = _httpClient.BaseAddress + route;

            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = response.StatusCode;
                        _logger.LogInformation($"\n[ERRO HTTP] Falha na busca do campo {cdCampo}. Status Code: {response.StatusCode}");
                        _logger.LogInformation($"[DETALHES] URL: {url}");
                        _logger.LogInformation($"[RESPOSTA API] {errorContent}");
                        return new CampoMOD();
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    campo = JsonConvert.DeserializeObject<CampoMOD>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"\n[ERRO GERAL] Exceção na chamada do campo {campo.NoCampo}: {ex.Message}");
                return new CampoMOD();
            }
            _logger.LogInformation($"\n[SUCESSO] Campo {campo.NoCampo} recebido da API.");
            return campo;
        }
        #endregion

        #region BuscarCamposPorPainel
        /// <summary>
        /// Busca todos os campos por painel
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <returns>List<PainelCampoMOD>></returns>
        public async Task<List<PainelCampoMOD>> BuscarCamposPorPainel(int cdPainel)
        {
            List<PainelCampoMOD> campos = new List<PainelCampoMOD>();
            string route = $"/Campos/Painel/{cdPainel}";
            string url = _httpClient.BaseAddress + route;

            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = response.StatusCode;
                        _logger.LogInformation($"\n[ERRO HTTP] Falha na busca dos campos do painel {cdPainel}. Status Code: {response.StatusCode}");
                        _logger.LogInformation($"[DETALHES] URL: {url}");
                        _logger.LogInformation($"[RESPOSTA API] {errorContent}");
                        return new List<PainelCampoMOD>();
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    campos = JsonConvert.DeserializeObject<List<PainelCampoMOD>>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"\n[ERRO GERAL] Exceção na chamada dos campos do painel {cdPainel}: {ex.Message}");
                return new List<PainelCampoMOD>();
            }
            _logger.LogInformation($"\n[SUCESSO] {campos.Count} campo(s) do painel {cdPainel} recebido(s) da API.");
            return campos;
        }
        #endregion

        #endregion

        #region Acesso

        #region BuscarAcessoPorPainel
        /// <summary>
        /// Busca o acesso pelo painel
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <returns>AcessoMOD</returns>
        public async Task<AcessoMOD> BuscarAcessoPorPainel(int cdPainel)
        {
            AcessoMOD acesso = new AcessoMOD();
            string route = $"/Acessos/Painel/{cdPainel}";
            string url = _httpClient.BaseAddress + route;

            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = response.StatusCode;
                        _logger.LogInformation($"\n[ERRO HTTP] Falha na busca do acesso do painel {cdPainel}. Status Code: {response.StatusCode}");
                        _logger.LogInformation($"[DETALHES] URL: {url}");
                        _logger.LogInformation($"[RESPOSTA API] {errorContent}");
                        return new AcessoMOD();
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    acesso = JsonConvert.DeserializeObject<AcessoMOD>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"\n[ERRO GERAL] Exceção na chamada do acesso do painel {cdPainel}: {ex.Message}");
                return new AcessoMOD();
            }
            _logger.LogInformation($"\n[SUCESSO] Acesso do painel {cdPainel} recebido da API.");
            return acesso;
        }
        #endregion

        #endregion

        #region Maquina

        #region BuscarMaquinaPorPainelEMaquina
        /// <summary>
        /// Busca o acesso pelo painel e a identificação da máquina
        /// </summary>
        /// <param name="cdPainel"></param>
        /// <param name="txIdMaquina"></param>
        /// <returns>MaquinaMOD</returns>
        public async Task<PainelMaquinaMOD> BuscarMaquinaPorPainelEMaquina(int cdPainel, string txIdMaquina)
        {
            PainelMaquinaMOD painelMaquina = new PainelMaquinaMOD();
            string route = $"/Maquinas/{txIdMaquina}/Painel/{cdPainel}";
            string url = _httpClient.BaseAddress + route;
            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    if(!response.IsSuccessStatusCode)
                    {
                        var errorContent = response.StatusCode;
                        _logger.LogInformation($"\n[ERRO HTTP] Falha na busca da máquina '{txIdMaquina}' para o painel {cdPainel}. Status Code: {response.StatusCode}");
                        _logger.LogInformation($"[DETALHES] URL: {url}");
                        _logger.LogInformation($"[RESPOSTA API] {errorContent}");
                        return null;
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    painelMaquina = JsonConvert.DeserializeObject<PainelMaquinaMOD>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"\n[ERRO GERAL] Exceção na chamada da máquina '{txIdMaquina}' para o painel {cdPainel}: {ex.Message}");
                return null;
            }
            _logger.LogInformation($"\n[SUCESSO] Máquina '{txIdMaquina}' para o painel {cdPainel} recebida da API.");
            return painelMaquina;
        }
        #endregion

        #endregion

        #endregion
    }
}
