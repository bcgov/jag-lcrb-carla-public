using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.Hosting;
using Stubble.Core.Builders;
using System.IO;
using RazorLight.Extensions;

using Wkhtmltopdf.NetCore;
using System.Reflection;
using System.Runtime.InteropServices;
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
        private readonly IConfiguration Configuration;        
        protected ILogger _logger;

        public PDFController(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;            
            _logger = loggerFactory.CreateLogger(typeof(PDFController));
        }

        [HttpPost]
        [Route("GetPDF/{template}")]
        [Produces("application/pdf")]
        [ProducesResponseType(200, Type = typeof(FileContentResult))]

        public async Task<IActionResult> GetPDF([FromServices] INodeServices nodeServices, [FromBody] Dictionary<string, object> rawdata, string template )
        {
            // first do a mustache merge.
            var stubble = new StubbleBuilder().Build();
            string filename = $"Templates/{template}.mustache";

            if (System.IO.File.Exists(filename))
            {
                string format = System.IO.File.ReadAllText(filename);
                var html = stubble.Render(format, rawdata);
                

                RotativaConfiguration.RotativaPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "rotativa");
                RotativaConfiguration.IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                var htmlToPdf = new HtmlAsPdf();
                htmlToPdf.PageMargins = new Margins(5,5,5,20);
                htmlToPdf.PageSize = Size.Letter;
                


                var pdf = htmlToPdf.GetPDF(html); 
         
                
                

                return new FileContentResult(pdf, "application/pdf");
            }
            else
            {
                return new NotFoundResult();
            }

        }        
       
    }
}
