using Gov.Lclb.Cllb.Public.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;        
        private readonly string _encryptionKey;

        public TokenTestController(IConfiguration configuration)
        {
            _configuration = configuration;            
            _encryptionKey = _configuration["ENCRYPTION_KEY"];
        }
        [HttpGet()]
        [AllowAnonymous]
        public ActionResult GetToken()
        {
            string result = _configuration["BASE_URI"] + _configuration["BASE_PATH"];
            result += "/bcservice?code="; 

            // generate a payload.
            string payload = "Sample token generated " + DateTime.Now.ToLongDateString();

            // encrypt that using two way encryption.
            result += System.Net.WebUtility.UrlEncode(EncryptionUtility.EncryptString(payload, _encryptionKey));

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = $"<html><body><h1>Token Generation Test</h1><p><a href=\"{result}\">{result}</body></html>"
            };
        }

    }
}
