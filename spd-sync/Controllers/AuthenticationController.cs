using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Gov.Lclb.Cllb.SpdSync.Controllers
{
    [Route("api/authentication")]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration Configuration;

        public AuthenticationController(IConfiguration configuration) 
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Returns a security token.
        /// </summary>
        /// <param name="secret"></param>
        /// <returns>A new JWT</returns>
        [HttpGet("token")]
        [AllowAnonymous]
        public string GetToken(string secret)
        {
            string result = "Invalid secret.";
            string configuredSecret = Configuration["Tokens:Key"];
            if (configuredSecret.Equals(secret))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    Configuration["Tokens:Issuer"],
                    Configuration["Tokens:Issuer"],
                    expires: DateTime.UtcNow.AddYears(5),
                    signingCredentials: creds
                    );
                result = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken) + " Expires:" + jwtSecurityToken.ValidTo.ToShortTimeString();
            }

            return result;
        }
    }
}
