using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spire.Doc;
using System;
using System.IO;
using System.Reflection;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;

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
            _logger.LogDebug(LoggingEvents.HttpPost, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);

            var guid = Guid.NewGuid();
            var folderName ="ConvertFiles";
            var docFilName= $@"{folderName}\{guid}.docx";
            var pdfFilName= $@"{folderName}\{guid}-pdf.pdf";
            try
            {

                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                byte[] newBytes = Convert.FromBase64String(base64File);
                System.IO.File.WriteAllBytes(docFilName, newBytes);
                Spire.Doc.Document document = new Spire.Doc.Document();
                document.LoadFromFile(docFilName);
                document.SaveToFile(pdfFilName, FileFormat.PDF);
                var pdfBytes = System.IO.File.ReadAllBytes(pdfFilName);
                DeleteFile(docFilName);
                DeleteFile(pdfFilName);
                return new JsonResult(Convert.ToBase64String(pdfBytes));
            }
            catch (Exception ex)
            {
                DeleteFile(docFilName);
                DeleteFile(pdfFilName);
                _logger.LogError(ex, $"Error while converting word file from PDF, Guid : {guid}");

                throw;
            }
        }

        private  void DeleteFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
        }

    }
    }
