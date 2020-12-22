using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stubble.Core.Builders;
using Wkhtmltopdf.NetCore;
using Wkhtmltopdf.NetCore.Options;

namespace PDF.Controllers
{
    public class JSONResponse
    {
        public string type;
        public byte[] data;
    }
    [Route("api/[controller]")]
    public class PDFController : Controller
    {
        readonly IGeneratePdf _generatePdf;
        private readonly IConfiguration Configuration;
        protected ILogger _logger;

        public PDFController(IConfiguration configuration, ILoggerFactory loggerFactory, IGeneratePdf generatePdf)
        {
            _generatePdf = generatePdf;
            Configuration = configuration;
            _logger = loggerFactory.CreateLogger(typeof(PDFController));
        }

        [HttpPost]
        [Route("GetPDF/{template}")]
        [Produces("application/pdf")]
        [ProducesResponseType(200, Type = typeof(FileContentResult))]
        public async Task<IActionResult> GetPDF([FromBody] Dictionary<string, object> rawdata, string template)
        {
            // first do a mustache merge.
            var stubble = new StubbleBuilder().Build();
            string filename = $"Templates/{template}.mustache";

            if (System.IO.File.Exists(filename))
            {
                string format = System.IO.File.ReadAllText(filename);
                var html = stubble.Render(format, rawdata);

                _generatePdf.SetConvertOptions(new ConvertOptions
                {
                    PageSize = Size.Letter,
                    PageMargins = new Margins(5, 5, 5, 5)
                });

                var pdf = await _generatePdf.GetPdfViewInHtml(html);
                return pdf;
            }

            return new NotFoundResult();
        }

        [HttpPost]
        [Route("GetHash/{template}")]
        public async Task<IActionResult> GetHash([FromBody] Dictionary<string, object> rawdata, string template)
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
