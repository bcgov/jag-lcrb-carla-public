using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using Gov.Lclb.Cllb.Public.Authentication;           // UserSettings lives here
using UserModel = Gov.Lclb.Cllb.Public.Models.User;  // Alias for claim constants (PermissionClaim, AccountidClaim, etc.)

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        private readonly IHttpContextAccessor _http;

        public AuthController(IConfiguration cfg, IHttpContextAccessor http)
        {
            _cfg = cfg;
            _http = http;
        }

        /// Issues a short-lived JWT that the Orchestrator can present back to the Portal API
        /// without needing SM cookies. Caller must already satisfy the Business-User policy.
        [HttpPost("delegation-token")]
        [Authorize(Policy = "Business-User")]
        public IActionResult IssueDelegationToken()
        {
            // Pull current SM-authenticated session context
            var us = UserSettings.CreateFromHttpContext(_http);
            if (us == null || !us.UserAuthenticated || string.IsNullOrEmpty(us.AccountId))
            {
                return Unauthorized();
            }

            // Read signing config
            var sec = _cfg.GetSection("DelegationJwt");
            var signingKey = sec["SigningKey"];
            if (string.IsNullOrWhiteSpace(signingKey))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "DelegationJwt:SigningKey is not configured.");
            }

            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build claims (fully-qualify Claim to avoid type shadowing)
            var claims = new[]
            {
                new System.Security.Claims.Claim("scope", "portal.delegation"),

                // Context the Orchestrator/Portal can use
                new System.Security.Claims.Claim(UserModel.AccountidClaim,               us.AccountId),
                new System.Security.Claims.Claim(UserModel.UseridClaim,                  us.UserId ?? string.Empty),
                new System.Security.Claims.Claim(UserModel.SiteMinderGuidClaim,          us.SiteMinderGuid ?? string.Empty),
                new System.Security.Claims.Claim(UserModel.SiteMinderBusinessGuidClaim,  us.SiteMinderBusinessGuid ?? string.Empty),
                new System.Security.Claims.Claim(UserModel.UserTypeClaim,                us.UserType ?? "Business")
            };

            var minutes = int.TryParse(sec["LifetimeMinutes"], out var m) ? m : 10;

            var jwt = new JwtSecurityToken(
                issuer:            sec["Issuer"],
                audience:          sec["Audience"],
                claims:            claims,
                notBefore:         DateTime.UtcNow,
                expires:           DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new
            {
                access_token = token,
                token_type   = "Bearer",
                expires_in   = minutes * 60
            });
        }
    }
}
