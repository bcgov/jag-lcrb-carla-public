using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class TokenTestController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly AppDbContext db;
        private readonly string _encryptionKey;

        public TokenTestController(AppDbContext db, IConfiguration configuration)
        {
            Configuration = configuration;
            this.db = db;
            this._encryptionKey = Configuration["ENCRYPTION_KEY"];
        }
        [HttpGet()]
        [AllowAnonymous]
        public ActionResult GetToken()
        {
            string result = Configuration["BASE_URI"] + Configuration["BASE_PATH"];
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
