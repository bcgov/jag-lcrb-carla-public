using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Stubble.Core.Builders;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace Gov.Jag.Lcrb.PdfService.Controllers
{
    public class JSONResponse
    {
        public string type;
        public byte[] data;
    }
    [Route("api/[controller]")]
    public class PDFController : Controller
    {
        readonly IConverter _generatePdf;
        private readonly IConfiguration Configuration;
        protected ILogger _logger;

        public PDFController(IConfiguration configuration, ILoggerFactory loggerFactory, IConverter generatePdf)
        {
            _generatePdf = generatePdf;
            Configuration = configuration;
            _logger = loggerFactory.CreateLogger(typeof(PDFController));
        }

        [HttpPost]
        [Route("GetPDF/{template}")]
        [Produces("application/pdf")]
        [ProducesResponseType(200, Type = typeof(FileContentResult))]
        public IActionResult GetPDF([FromBody] Dictionary<string, object> rawdata, string template)
        {
            // first do a mustache merge.
            var stubble = new StubbleBuilder().Build();
            string filename = $"Templates/{template}.mustache";

            if (System.IO.File.Exists(filename))
            {
                string format = System.IO.File.ReadAllText(filename);
                var html = stubble.Render(format, rawdata);
                
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        PaperSize = PaperKind.Letter,
                        Orientation = Orientation.Portrait,
                        Margins = new MarginSettings(5.0,5.0,5.0,5.0)
                    },
                    
                    Objects = {
                        new ObjectSettings()
                        {
                            HtmlContent = html
                        }
                    }
                };
                try
                {
                    var pdf = _generatePdf.Convert(doc); 
                    return File(pdf, "application/pdf");
                }
                catch (Exception e)
                {
                    _logger.LogError(e,"ERROR rendering PDF");
                    _logger.LogError(template);
                    _logger.LogError(html);
                }
            }

            return new NotFoundResult();
        }

        [HttpPost]
        [Route("GetHash/{template}")]
        public IActionResult GetHash([FromBody] Dictionary<string, object> rawdata, string template)
        {
            // first do a mustache merge.
            var stubble = new StubbleBuilder().Build();
            string filename = $"Templates/{template}.mustache";

            if (System.IO.File.Exists(filename))
            {
                string format = System.IO.File.ReadAllText(filename);
                var html = stubble.Render(format, rawdata);

                // compute a hash of the template to render as PDF
                var hash = HashUtility.GetSHA256(Encoding.UTF8.GetBytes(html));
                return new JsonResult(new { hash = hash });
            }

            return new NotFoundResult();
        }
    }
}
