using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class SoapHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger logger;

        public SoapHeaderMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            
            
            if (context.Request.Path.Value.Equals("/receiveFromHub"))
            {
                string soapAction = context.Request.Headers["SOAPAction"];
                if (string.IsNullOrEmpty(soapAction) || soapAction.Equals("\"\""))
                {
                    context.Request.Headers["SOAPAction"] = "http://tempuri.org/IReceiveFromHubService/receiveFromHub";
                }
                
                
            }

            var requestLog = await FormatRequest(context.Request);
            logger.LogInformation(requestLog);

            await _next(context);
        }


        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;

            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableRewind();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
            request.Body = body;

            var headers = "";

            foreach(var kvPair in request.Headers.AsEnumerable())
            {
                headers += $"{kvPair.Key}: {kvPair.Value}\n";
            }

            return $">>>>>> START REQUEST lOG: \n Scheme: {request.Scheme} \n Path: {request.Host}{request.Path} \n QueryString: {request.QueryString} \n Body: {bodyAsText} \n Headers: {headers} \n <<<<<< END REQUEST lOG";
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
