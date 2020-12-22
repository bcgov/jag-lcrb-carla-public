using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace SepService.Controllers
{
    [Route("authentication")]
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
            string configuredSecret = Configuration["JWT_TOKEN_KEY"];
            if (configuredSecret.Equals(secret))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    Configuration["JWT_VALID_ISSUER"],
                    Configuration["JWT_VALID_ISSUER"],
                    expires: DateTime.UtcNow.AddYears(5),
                    signingCredentials: creds
                );
                result = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken) + " Expires:" + jwtSecurityToken.ValidTo.ToShortTimeString();
            }

            return result;
        }


        [HttpGet("token/{secret}")]
        [AllowAnonymous]
        // response_type=token&client_id=&redirect_uri=https%3A%2F%2Flocalhost%3A5001%2Fswagger%2Foauth2-redirect.html&state=VHVlIERlYyAwOCAyMDIwIDE0OjE1OjE2IEdNVC0wODAwIChQYWNpZmljIFN0YW5kYXJkIFRpbWUp
        public ActionResult SetToken([FromRoute] string secret, string redirect_uri, string response_type, string state)
        {
            string result = "Invalid secret.";
            string token = "";
            string expires_in = "";
            string configuredSecret = Configuration["JWT_TOKEN_KEY"];
            if (configuredSecret.Equals(secret))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                
                var jwtSecurityToken = new JwtSecurityToken( 
                    Configuration["JWT_VALID_ISSUER"],
                    Configuration["JWT_VALID_ISSUER"],
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
                );
                token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                result = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken) + " Expires:" + jwtSecurityToken.ValidTo.ToShortTimeString();
                
            }

            if (!string.IsNullOrEmpty(redirect_uri))
            {
                Response.Headers.Add("Authorization", $"Bearer {result}");
                string tokenEncoded = UrlEncoder.Default.Encode(token);
                string redirect = redirect_uri +
                                  $"#state={state}&token_type=Bearer&access_token={tokenEncoded}&id_token={tokenEncoded}&access_token={tokenEncoded}&expires_in=99999";
                return new RedirectResult(redirect);
            }
            else
            {
                return Ok(new { Token = result });
            }
            
        }
    }
}
