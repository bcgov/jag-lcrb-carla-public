using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Gov.Lclb.Cllb.SpdSync.Controllers
{
    [Route("api/authentication")]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Returns a security token.
        /// </summary>
        /// <param name="secret"></param>
        /// <returns>A new JWT</returns>
        [HttpGet("token/{secret}")]
        [AllowAnonymous]
        public string GetToken(string secret)
        {
            string result = "Invalid secret.";
            string configuredSecret = _configuration["JWT_TOKEN_KEY"];
            if (configuredSecret.Equals(secret))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_TOKEN_KEY"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["JWT_VALID_ISSUER"],
                    _configuration["JWT_VALID_AUDIENCE"],
                    expires: DateTime.UtcNow.AddYears(5),
                    signingCredentials: creds
                    );
                result = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken) + " Expires:" + jwtSecurityToken.ValidTo.ToShortDateString();
            }

            return result;
        }
    }
}
