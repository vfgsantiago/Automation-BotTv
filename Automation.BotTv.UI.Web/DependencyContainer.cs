using Automation.BotTv.Data;
using Automation.BotTv.Repository;

namespace Automation.BotTv.UI.Web
{
    public class DependencyContainer
    {
        public static void RegisterContainers(IServiceCollection services)
        {
            #region Acesso
            services.AddScoped<AcessoREP>();
            #endregion

            #region Painel
            services.AddScoped<PainelCampoREP>();
            services.AddScoped<PainelREP>();
            services.AddScoped<PainelTipoREP>();
            services.AddScoped<PainelMaquinaREP>();
            services.AddScoped<PainelAcessoREP>();
            #endregion

            #region Campo
            services.AddScoped<CampoREP>();
            services.AddScoped<CampoTipoREP>();
            services.AddScoped<CampoAcaoREP>();
            #endregion

            #region Maquina
            services.AddScoped<MaquinaREP>();
            #endregion

            #region Common
            services.AddScoped<HttpClient>();
            services.AddScoped<AcessaDados>();
            #endregion

            #region Login
            services.AddScoped<LoginREP>();
            services.AddScoped<UsuarioREP>();
            services.AddScoped<SistemaREP>();
            #endregion
        }
    }
}