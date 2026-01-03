using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Automation.BotTv.Model;
using Automation.BotTv.API;

namespace Automation.BotTv.API.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class TokenController : ControllerBase
    {
        #region Repositorios
        private readonly JwtOptions _jwtOptions;
        #endregion

        #region Construtor
        public TokenController(JwtOptions jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }
        #endregion

        #region Metodos

        #region Connect
        [HttpPost("Connect")]
        public IActionResult Connect([FromBody] LoginRequest request)
        {
            if (request.Username.ToUpper() != "AUTOMATIONBOTTV" || request.Password.ToUpper() != "INTEGRACAOAUTOMATIONBOTTV")
                return Unauthorized("Credenciais inválidas");

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, request.Username),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(_jwtOptions.ExpirationSeconds),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                Token = tokenString,
                Expiration = DateTime.UtcNow.AddSeconds(_jwtOptions.ExpirationSeconds)
            });
        }
        #endregion

        #endregion
    }
}