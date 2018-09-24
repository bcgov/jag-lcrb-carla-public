using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Microsoft.EntityFrameworkCore;
using PDF.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.NodeServices;

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
        [Route("GetPDF")]

        public async Task<IActionResult> GetPDF([FromServices] INodeServices nodeServices, [FromBody]  Object rawdata )
        {
            JSONResponse result = null;
            var options = new { format="letter", orientation= "landscape" };            

            // execute the Node.js component
            result = await nodeServices.InvokeAsync<JSONResponse>("./pdf", "cannabis_licence", rawdata, options); 
                        
            return new FileContentResult(result.data, "application/pdf");
        }        
       
    }
}
