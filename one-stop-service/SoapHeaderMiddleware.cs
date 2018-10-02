using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class SoapHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public SoapHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Do work that doesn't write to the Response.
            if (context.Request.Path.Value.Equals("/receiveFromHub") && string.IsNullOrEmpty(context.Request.Headers["SoapAction"]))
            {
                context.Request.Headers["SoapAction"] = "http://tempuri.org/IReceiveFromHubService/receiveFromHub";
            }
            await _next(context);
        }
    }


    public static class SoapHeaderMiddlewareExtensions
    {
        public static IApplicationBuilder UseSoapHeaderMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SoapHeaderMiddleware>();
        }
    }
}
