using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Automation.BotTv.Model;

namespace Automation.BotTv.Repository
{
    public class TokenService
    {
        #region Servicos
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TokenService> _logger;
        #endregion

        #region Construtor
        public TokenService(IConfiguration configuration, HttpClient httpClient, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_configuration.GetSection("BaseUrl").Value);
            _logger = logger;
        }
        #endregion

        #region Metodos

        #region GerarToken
        /// <summary>
        /// Buscar todos os produtos da base VSM
        /// </summary>
        /// <returns></returns>
        public async Task<JwtToken> GerarToken()
        {
            JwtToken token = new JwtToken();
            string route = $"/Token/Connect";
            string url = _httpClient.BaseAddress + route;

            Login loginRequest = new Login()
            {
                Username = "AUTOMATIONBOTTV",
                Password = "INTEGRACAOAUTOMATIONBOTTV"
            };
            try
            {
                using (var response = _httpClient.PostAsJsonAsync(url, loginRequest).Result)
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = response.StatusCode;
                        _logger.LogInformation($"\n[ERRO HTTP] Falha ao se conectar a API. Status Code: {response.StatusCode}");
                        _logger.LogInformation($"[DETALHES] URL: {url}");
                        _logger.LogInformation($"[RESPOSTA API] {errorContent}");
                        return new JwtToken();
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<JwtToken>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"\n[ERRO GERAL] Exceção na autenticação da API: {ex.Message}");
                return new JwtToken();
            }
            _logger.LogInformation($"\n[SUCESSO] API autenticada!");
            return token;
        }
        #endregion

        #endregion
    }
}