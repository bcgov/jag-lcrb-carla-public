using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Public.Middleware
{
    /// <summary>
    /// Back-end-for-front-end between the embedded chat UI and the LCRB agentic platform.
    ///
    /// <para><b>Why a proxy at all.</b> The chat UI is a public-facing page in this portal, but the
    /// platform authenticates callers with a bearer token. Handing a token to the browser would put
    /// a platform credential in public hands. Instead the browser calls this application on its own
    /// origin with nothing but its ordinary SiteMinder session cookie, and this middleware attaches
    /// a freshly minted, short-lived token server-side. No platform credential reaches the client.</para>
    ///
    /// <para><b>Why middleware and not a controller.</b> This project runs legacy MVC
    /// (<c>EnableEndpointRouting = false</c>) and registers NWebsec's header attributes as global
    /// action filters. Those filters set response headers in OnActionExecuted, which throws
    /// "Headers are read-only" against a streamed response because the first chunk has long since
    /// been sent. Running as middleware sidesteps the filter pipeline entirely.</para>
    ///
    /// <para><b>Ordering.</b> Must be registered AFTER UseSession() and UseAuthentication() -- it
    /// reads the session to identify the user -- and BEFORE UseMvc(), which is terminal.</para>
    ///
    /// <para><b>The agent is pinned server-side.</b> The upstream path is built from configuration,
    /// never from the request. A caller cannot reach a different agent by editing the URL.</para>
    /// </summary>
    public class AgenticPlatformProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AgenticPlatformProxyMiddleware> _logger;

        // Everything this middleware is willing to forward. Anything else under these prefixes is
        // refused, so adding a call in the UI is a deliberate change here rather than an accident.
        private const string PrefixAgentFramework = "/api/agent-framework";
        private const string PrefixAdminSrv = "/api/admin-srv";
        private const string PrefixEvalSrv = "/api/eval-srv";

        // Response headers that describe the upstream connection rather than the payload. Copying
        // these corrupts the response Kestrel is building for our own client.
        private static readonly HashSet<string> HopByHopHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "transfer-encoding", "content-length", "connection", "keep-alive",
            "proxy-authenticate", "proxy-authorization", "te", "trailer", "upgrade"
        };

        public AgenticPlatformProxyMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<AgenticPlatformProxyMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IAgenticPlatformTokenService tokenService)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (!path.StartsWith(PrefixAgentFramework, StringComparison.OrdinalIgnoreCase)
                && !path.StartsWith(PrefixAdminSrv, StringComparison.OrdinalIgnoreCase)
                && !path.StartsWith(PrefixEvalSrv, StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var baseUrl = _configuration["AGENTIC_PLATFORM_BASE_URL"];
            var agentId = _configuration["AGENTIC_PLATFORM_AGENT_ID"];

            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(agentId))
            {
                _logger.LogError("Agentic proxy is not configured: AGENTIC_PLATFORM_BASE_URL and AGENTIC_PLATFORM_AGENT_ID are both required.");
                await WriteProblem(context, StatusCodes.Status500InternalServerError, "chat_not_configured");
                return;
            }

            var upstreamPath = ResolveUpstreamPath(context.Request.Method, path, agentId);
            if (upstreamPath == null)
            {
                // Deliberately logged: during UAT this is how we discover a UI call we have not
                // allowed for yet, rather than it failing silently in the browser.
                _logger.LogWarning("Agentic proxy refused un-allowlisted route {Method} {Path}", context.Request.Method, path);
                await WriteProblem(context, StatusCodes.Status404NotFound, "route_not_allowed");
                return;
            }

            // CreateFromHttpContext is the accessor the rest of this application uses (94 call
            // sites). It reads the session and falls back to claims; that fallback cannot supply
            // UserType, so a session-less request is refused below rather than silently minting a
            // token the platform would reject.
            var userSettings = UserSettings.CreateFromHttpContext(httpContextAccessor);
            var token = tokenService.MintToken(userSettings, out var failureReason);
            if (token == null)
            {
                _logger.LogInformation("Agentic proxy rejected a request: {Reason}", failureReason);
                await WriteProblem(context, StatusCodes.Status401Unauthorized, "not_signed_in");
                return;
            }

            await ForwardAsync(context, httpClientFactory, baseUrl, upstreamPath, token);
        }

        /// <summary>
        /// Maps an incoming request onto the upstream path, or returns null to refuse it.
        /// The agent id is taken from configuration and never from the caller.
        /// </summary>
        private static string ResolveUpstreamPath(string method, string path, string agentId)
        {
            var isGet = HttpMethods.IsGet(method);
            var isPost = HttpMethods.IsPost(method);

            // POST /api/agent-framework/agents/{anything}/invoke/stream -> pinned agent
            if (isPost
                && path.StartsWith(PrefixAgentFramework + "/agents/", StringComparison.OrdinalIgnoreCase)
                && path.EndsWith("/invoke/stream", StringComparison.OrdinalIgnoreCase))
            {
                return "/api/agent-framework/agents/" + agentId + "/invoke/stream";
            }

            // Conversation reads are deliberately NOT forwarded. The agent behind this proxy is
            // configured to store nothing (HISTORY_PERSISTENCE_DISABLED_AGENTS on the agent
            // framework), so there is never a stored transcript to fetch. The embedded UI holds the
            // conversation in the browser and sends it with each request instead. Refusing the
            // route here means the commitment does not rest on the UI behaving: the portal will not
            // ask for a transcript even if something asks it to.

            // GET /api/admin-srv/agents/{anything}/summary -> pinned agent. The full agent record
            // is staff-only and must never be proxied: it contains the system prompt and every
            // prompt version. Only the narrow public summary is exposed.
            if (isGet
                && path.StartsWith(PrefixAdminSrv + "/agents/", StringComparison.OrdinalIgnoreCase)
                && path.EndsWith("/summary", StringComparison.OrdinalIgnoreCase))
            {
                return "/api/admin-srv/agents/" + agentId + "/summary";
            }

            // GET /api/admin-srv/config/public/{id} -- the logo and the legal text the chat shows.
            // Only the public read is forwarded; admin-srv restricts it to a fixed set of display
            // settings, and listing or updating configuration stays staff-only and unreachable here.
            if (isGet && TryTrailingSegment(path, PrefixAdminSrv + "/config/public/", out var configId))
            {
                return "/api/admin-srv/config/public/" + Uri.EscapeDataString(configId);
            }

            // POST /api/eval-srv/feedback -- thumbs up/down on an answer.
            if (isPost && path.Equals(PrefixEvalSrv + "/feedback", StringComparison.OrdinalIgnoreCase))
            {
                return "/api/eval-srv/feedback";
            }

            return null;
        }

        /// <summary>
        /// Extracts a single trailing path segment after the prefix, refusing anything containing a
        /// further slash so a crafted id cannot walk to another route.
        /// </summary>
        private static bool TryTrailingSegment(string path, string prefix, out string segment)
        {
            segment = null;
            if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return false;

            var remainder = path.Substring(prefix.Length).Trim('/');
            if (remainder.Length == 0 || remainder.Contains("/")) return false;

            segment = remainder;
            return true;
        }

        private async Task ForwardAsync(
            HttpContext context,
            IHttpClientFactory httpClientFactory,
            string baseUrl,
            string upstreamPath,
            string token)
        {
            var url = baseUrl.TrimEnd('/') + upstreamPath;
            var cancellationToken = context.RequestAborted;

            using var upstreamRequest = new HttpRequestMessage(new HttpMethod(context.Request.Method), url);

            if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method))
            {
                upstreamRequest.Content = new StreamContent(context.Request.Body);
                if (!string.IsNullOrWhiteSpace(context.Request.ContentType)
                    && MediaTypeHeaderValue.TryParse(context.Request.ContentType, out var contentType))
                {
                    upstreamRequest.Content.Headers.ContentType = contentType;
                }
            }

            // The minted token replaces anything the caller sent. A browser on this origin cannot
            // influence which identity the platform sees.
            upstreamRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            upstreamRequest.Headers.Accept.ParseAdd("text/event-stream, application/json");

            var correlationId = context.Request.Headers["X-Correlation-Id"].ToString();
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                upstreamRequest.Headers.TryAddWithoutValidation("X-Correlation-Id", correlationId);
            }

            var client = httpClientFactory.CreateClient("AgenticPlatform");

            try
            {
                // ResponseHeadersRead returns as soon as the headers arrive rather than buffering
                // the whole body. Without it a streamed reply cannot be relayed progressively.
                using var upstreamResponse = await client.SendAsync(
                    upstreamRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                context.Response.StatusCode = (int)upstreamResponse.StatusCode;

                foreach (var header in upstreamResponse.Headers.Concat(upstreamResponse.Content.Headers))
                {
                    if (HopByHopHeaders.Contains(header.Key)) continue;
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                // Turn off every layer of buffering we control, so a streamed reply reaches the
                // browser as it arrives. X-Accel-Buffering is for the nginx route in front of this
                // application in the deployed environments.
                context.Response.Headers["X-Accel-Buffering"] = "no";
                context.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();

                using var upstreamBody = await upstreamResponse.Content.ReadAsStreamAsync();

                var buffer = new byte[8192];
                int read;
                while ((read = await upstreamBody.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await context.Response.Body.WriteAsync(buffer, 0, read, cancellationToken);
                    // Flush per chunk -- this is what makes it a stream rather than a slow download.
                    await context.Response.Body.FlushAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // The user navigated away mid-answer. Ordinary, not an error.
                _logger.LogDebug("Agentic proxy client disconnected from {Path}", upstreamPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Agentic proxy failed forwarding to {Path}", upstreamPath);
                if (!context.Response.HasStarted)
                {
                    await WriteProblem(context, StatusCodes.Status502BadGateway, "chat_unavailable");
                }
            }
        }

        /// <summary>
        /// Writes a small JSON error. Codes are stable, non-descriptive strings: the browser gets
        /// enough to render a message, and nothing about the platform internals.
        /// </summary>
        private static async Task WriteProblem(HttpContext context, int statusCode, string code)
        {
            if (context.Response.HasStarted) return;
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"" + code + "\"}");
        }
    }

    public static class AgenticPlatformProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseAgenticPlatformProxy(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AgenticPlatformProxyMiddleware>();
        }
    }
}
