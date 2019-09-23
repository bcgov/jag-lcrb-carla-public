using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Gov.Lclb.Cllb.Geocoder.Controllers
{
    [Route("api/authentication")]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration Configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // test point for authorization.  Returns OK if authorized.
        [HttpGet("test")]
        [Authorize]
        public ActionResult Test()
        {
            return new OkResult();
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
            string configuredSecret = Configuration["JWT_TOKEN_KEY"];
            if (configuredSecret.Equals(secret))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    Configuration["JWT_VALID_ISSUER"],
                    Configuration["JWT_VALID_AUDIENCE"],
                    expires: DateTime.UtcNow.AddYears(5),
                    signingCredentials: creds
                    );
                result = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken) + " Expires:" + jwtSecurityToken.ValidTo.ToShortTimeString();
            }

            return result;
        }
    }
}

