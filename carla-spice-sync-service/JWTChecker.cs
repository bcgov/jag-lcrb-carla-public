using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public static class JwtChecker
    {
        public static bool Check (string token, IConfiguration Configuration)
        {
            bool result = false;
            // first check the bearer.
            string secret = Configuration["JWT_TOKEN_KEY"];
            var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            try
            {
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            

            return result;
        }
    }
}
