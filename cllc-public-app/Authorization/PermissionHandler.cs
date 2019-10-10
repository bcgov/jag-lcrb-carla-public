using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Authorization
{
    /// <summary>
    /// Permission Handler Extension
    /// </summary>
    public static class PermissionHandlerExtensions
    {
        /// <summary>
        /// Registers <see cref="PermissionHandler"/> with Dependency Injection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPermissionHandler(this IServiceCollection services)
        {
            return services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        }
    }

    /// <summary>
    /// Permission handler
    /// </summary>
    /// <remarks>
    /// Must be registered with Dependency Injection
    /// </remarks>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly HttpContext _httpContext;
        private readonly IHostingEnvironment _hostingEnv;

        public PermissionHandler(IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnv)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _hostingEnv = hostingEnv;
        }

        /// <summary>
        /// Permission Handler
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // **************************************************
            // Check if we have a Dev Environment Cookie
            // **************************************************
            if (!_hostingEnv.IsProduction())
            {
                string temp = _httpContext.Request.Cookies["DEV-USER"];

                if (string.IsNullOrEmpty(temp))
                {
                    // may be a dev header.
                    temp = _httpContext.Request.Headers["DEV-USER"];
                }


                if (!string.IsNullOrEmpty(temp))
                {
                    // access granted
                    context.Succeed(requirement);

                    await Task.CompletedTask;
                    return;
                }
                else
                {
                    temp = _httpContext.Request.Cookies["DEV-BCSC-USER"];

                    if (string.IsNullOrEmpty(temp))
                    {
                        // may be a dev header.
                        temp = _httpContext.Request.Headers["DEV-BCSC-USER"];
                    }


                    if (!string.IsNullOrEmpty(temp))
                    {
                        // access granted
                        context.Succeed(requirement);

                        await Task.CompletedTask;
                        return;
                    }
                }
            }

            // **************************************************
            // If not - check the users permissions
            // **************************************************
            if (context.User.HasPermissions(requirement.RequiredPermissions.ToArray()))
            {
                // access granted
                context.Succeed(requirement);
            }
            else
            {
                // access denied
                context.Fail();
            }

            await Task.CompletedTask;
        }
    }
}
