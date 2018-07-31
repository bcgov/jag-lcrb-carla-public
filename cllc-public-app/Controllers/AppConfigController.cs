using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AppConfigController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger _logger;

        public AppConfigController(IConfiguration configuration,  ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _logger = loggerFactory.CreateLogger(this.GetType());
        }

        [HttpGet]
        public IActionResult ShowLogin()
        {
            var result = new ClientConfig{
                IsLiteVersion = !String.IsNullOrEmpty(Configuration["IS_LITE_VERSION"]),
                ShowLogin = !String.IsNullOrEmpty(Configuration["SHOW_LOGIN"])
            };
            return Json(result);
        }

    }
    public class ClientConfig {
        public bool IsLiteVersion { get; set; }
        public bool ShowLogin { get; set; }
    }
}