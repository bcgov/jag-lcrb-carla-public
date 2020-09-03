using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Grpc.Core;
using Gov.Lclb.Cllb.Services.FileManager;
using System.IO;
using Google.Protobuf;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using Gov.Lclb.Cllb.Public.ViewModels;
using System.Xml.Linq;
using System.Xml.XPath;
using Gov.Lclb.Cllb.Public.Mapping;
using Microsoft.Extensions.Caching.Memory;

// from https://raw.githubusercontent.com/bcgov/jag-lcrb-carla-public/446c4f15f159c7f569e03ac138abce3d81aa3f92/cllc-public-app/Controllers/SystemFormController.cs


namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FormsController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IDynamicsClient _dynamicsClient;

        public FormsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(FileController));
            _cache = memoryCache;

        }

        [HttpGet("{formid}")]
        public async Task<JsonResult> GetSystemForm(string formid)
        {

            var form = _dynamicsClient.GetSystemformViewModel(_cache, _logger, formid);
            return new JsonResult(form);
        }
    }

}

