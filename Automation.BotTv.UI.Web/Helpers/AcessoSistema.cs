using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Automation.BotTv.Model;
using Automation.BotTv.Repository;

namespace Automation.BotTv.UI.Web.Helpers
{
    public class AcessoSistema(SistemaREP _repositorioSistema,
    UsuarioREP _repositorioUsuario,
    ILogger<AcessoSistema> _logger,
    IConfiguration _configuration) : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var remoteIp = filterContext.HttpContext.Connection.RemoteIpAddress;
            string referrer = filterContext.HttpContext.Request.Host.Value;

            var CdUsuario = Convert.ToInt32(filterContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("ID")).Value);
            var url = _configuration.GetSection("LinkIntranet").Value;
            var id = _configuration.GetSection("CodigoSistema").Value;

            Controller controller = filterContext.Controller as Controller;

            if (CdUsuario > 0)
            {
                SistemaMOD sistema = await _repositorioSistema.BuscarPorCodigo(Convert.ToInt32(id));
                UsuarioSistemaMOD usuarioSistema = await _repositorioUsuario.BuscarAcessoUsuarioSistema(CdUsuario, sistema.CdSistema);

                if (usuarioSistema.Usuario.CdUsuario > 0)
                {
                    if (sistema.TpVisibilidade == "I")
                    {
                        if (!IsIpAddressInternal(remoteIp.ToString()))
                        {
                            filterContext.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                            _logger.LogWarning($"Acesso bloqueado do IP: {remoteIp}");
                            controller.HttpContext.Response.Redirect(url);
                        }
                    }
                    return;
                }
                else
                {
                    filterContext.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                    _logger.LogWarning($"Acesso bloqueado, usuario sem acesso: {remoteIp}");
                    controller.HttpContext.Response.Redirect(url);
                }
            }
            else
            {
                filterContext.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                _logger.LogWarning($"Acesso bloqueado, usuario sem acesso: {remoteIp}");
                controller.HttpContext.Response.Redirect(url);
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary> 
        /// Verifica se o IP é interno ou externo
        /// </summary> 
        /// <param name="ipAddress">String contendo o IP do Usuário</param>
        /// <returns></returns> 
        public static bool IsIpAddressInternal(string ipAddress)
        {
            string[] incomingOctets = ipAddress.Trim().Split(new char[] { '.' });
            string addresses = "::1, 127.0.0.1, 10.0.*.*, 172.*.*.*, 192.*.*.*";
            string[] validIpAddresses = addresses.Trim().Split(new char[] { ',' });
            foreach (var validIpAddress in validIpAddresses)
            {
                if (validIpAddress.Trim() == ipAddress)
                {
                    return true;
                }
                string[] validOctets = validIpAddress.Trim().Split(new char[] { '.' });
                bool matches = true;
                for (int index = 0; index < validOctets.Length; index++)
                {
                    if (validOctets[index] != "*")
                    {
                        if (validOctets[index] != incomingOctets[index])
                        {
                            matches = false;
                            break;
                        }
                    }
                }
                if (matches)
                {
                    return true;
                }
            }
            return false;
        }

    }

}
