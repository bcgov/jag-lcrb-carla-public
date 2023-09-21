using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Aspose.Words;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using Microsoft.AspNetCore.Authorization;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFConverterController : ControllerBase
    {
        private readonly BCeIDBusinessQuery _bceid;
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IOrgBookClient _orgBookclient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly FileManagerClient _fileManagerClient;
        private readonly IWebHostEnvironment _env;

        public PDFConverterController(IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IOrgBookClient orgBookClient,
            BCeIDBusinessQuery bceid,
            ILoggerFactory loggerFactory,
            IDynamicsClient dynamicsClient,
            FileManagerClient fileManagerClient,
            IWebHostEnvironment env)
        {
            _configuration = configuration;
            _bceid = bceid;
            _dynamicsClient = dynamicsClient;
            _env = env;
            _orgBookclient = orgBookClient;
            _httpContextAccessor = httpContextAccessor;
            _fileManagerClient = fileManagerClient;
            _logger = loggerFactory.CreateLogger(typeof(PDFConverterController));
        }
        [HttpPost("convert-file-pdf")]
        [AllowAnonymous]
        public IActionResult ConvertFilePDF([FromBody] string base64File)
        {
            var base64PDF=string.Empty;
            var originalFileBytes=Convert.FromBase64String(base64File);
            using (var originalStream=new MemoryStream(originalFileBytes))
            {
                var doc = new Document(originalStream);
                using (var pdfStream = new MemoryStream())
                {
                    doc.Save(pdfStream,SaveFormat.Pdf);
                    var pdfBytes=pdfStream.ToArray();
                    base64PDF=Convert.ToBase64String(pdfBytes);
                }
            }
            return new JsonResult(base64PDF);
        }
       
    }
}
