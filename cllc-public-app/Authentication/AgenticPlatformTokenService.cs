using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gov.Lclb.Cllb.Public.Authentication
{
    /// <summary>
    /// Mints the short-lived bearer token the agentic platform sees on behalf of the signed-in
    /// portal user.
    ///
    /// <para>The platform identifies a caller purely from JWT claims (see CallerContextService in
    /// the agent framework). It distinguishes a staff caller from a public one by the presence of
    /// an <c>oid</c> claim: <c>oid</c> present means staff, <c>UserType</c> present means public.
    /// This service must therefore NEVER emit an <c>oid</c> claim -- doing so would silently
    /// promote a portal user to staff, which bypasses every ownership check on the platform.</para>
    ///
    /// <para>Claim names are deliberately written as literals rather than reusing the
    /// <c>User.*Claim</c> constants elsewhere in this project. Those constants are snake_case
    /// (<c>accountid_claim</c>) and the platform matches on exact PascalCase names. Using them
    /// here produces a token the platform rejects as unauthenticated.</para>
    /// </summary>
    public interface IAgenticPlatformTokenService
    {
        /// <summary>
        /// Builds a signed token for the given session, or returns null when the session does not
        /// describe a user the platform can accept.
        /// </summary>
        string MintToken(UserSettings userSettings, out string failureReason);
    }

    public sealed class AgenticPlatformTokenService : IAgenticPlatformTokenService
    {
        private readonly IConfiguration _configuration;

        public AgenticPlatformTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string MintToken(UserSettings userSettings, out string failureReason)
        {
            failureReason = null;

            if (userSettings == null || !userSettings.UserAuthenticated)
            {
                failureReason = "no authenticated portal session";
                return null;
            }

            // The platform accepts exactly these two values and treats anything else as an
            // unauthenticated caller. Fail here with a clear reason rather than sending a token
            // that comes back as an opaque 401 from the platform.
            if (userSettings.UserType != "Business" && userSettings.UserType != "VerifiedIndividual")
            {
                failureReason = $"unsupported UserType '{userSettings.UserType ?? "(null)"}'";
                return null;
            }

            // Mirrors UserSettings.Validate(): AccountId is required for Business callers only.
            // The platform enforces the same rule and refuses a Business token without one.
            if (userSettings.UserType == "Business" && string.IsNullOrWhiteSpace(userSettings.AccountId))
            {
                failureReason = "Business session has no AccountId";
                return null;
            }

            // Flat, upper-snake configuration keys, matching how the rest of this app reads its
            // settings (see DatabaseTools: DATABASE_SERVICE_NAME, DB_USER and the rest). In
            // OpenShift these are plain environment variables of the same name.
            var signingKey = _configuration["AGENTIC_PLATFORM_SIGNING_KEY"];
            if (string.IsNullOrWhiteSpace(signingKey))
            {
                failureReason = "AGENTIC_PLATFORM_SIGNING_KEY is not configured";
                return null;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("UserType", userSettings.UserType),
                new Claim("UserId", userSettings.UserId ?? string.Empty),
                new Claim("UserDisplayName", userSettings.UserDisplayName ?? string.Empty)
            };

            if (!string.IsNullOrWhiteSpace(userSettings.ContactId))
            {
                claims.Add(new Claim("ContactId", userSettings.ContactId));
            }

            // Only Business callers carry an AccountId. A VerifiedIndividual sends none, by design.
            if (userSettings.UserType == "Business")
            {
                claims.Add(new Claim("AccountId", userSettings.AccountId));
            }

            // Defaults match the APIM global policy's <issuer> and <audience>. Changing either
            // here without changing the policy presents as a 401 on every request.
            var lifetimeMinutes = int.TryParse(_configuration["AGENTIC_PLATFORM_TOKEN_LIFETIME_MINUTES"], out var minutes) ? minutes : 15;

            var jwt = new JwtSecurityToken(
                issuer: _configuration["AGENTIC_PLATFORM_TOKEN_ISSUER"] ?? "carla-portal",
                audience: _configuration["AGENTIC_PLATFORM_TOKEN_AUDIENCE"] ?? "lcrb-agentic-platform",
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(lifetimeMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
