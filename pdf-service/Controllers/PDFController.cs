using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Microsoft.EntityFrameworkCore;
using PDF.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.Hosting;

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
        private readonly IHostingEnvironment _env;
        protected ILogger _logger;

        public PDFController(IConfiguration configuration, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            _logger = loggerFactory.CreateLogger(typeof(PDFController));
        }

        [HttpPost]
        [Route("GetPDF")]
        [Produces("application/pdf")]
        [ProducesResponseType(200, Type = typeof(FileContentResult))]

        public async Task<IActionResult> GetPDF([FromServices] INodeServices nodeServices, [FromBody]  Object rawdata )
        {
            JSONResponse result = null;
            var options = new { format="letter", orientation= "portrait" };            

            // execute the Node.js component
            result = await nodeServices.InvokeAsync<JSONResponse>("./pdf", "cannabis_licence", rawdata, options); 
                        
            return new FileContentResult(result.data, "application/pdf");
        }        
       
        [HttpGet]
        [Route("GetTestPDF")]

        public async Task<IActionResult> GetTestPDF([FromServices] INodeServices nodeServices)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");

            JSONResponse result = null;
            var options = new { format="letter", orientation= "portrait" };            

            var testObject = new Dictionary <string, string>();
            testObject.Add("title", "test title");
            testObject.Add("licenceNumber", "123456");
            testObject.Add("businessName", "test biz name");
            testObject.Add("addressLine1", "address 1");
            testObject.Add("addressLine2", "address 2");
            testObject.Add("companyName", "Test Inc.");
            // testObject.Add("permitIssueDate", "date 123");
            // testObject.Add("restrictionsText", "restrictions");

            // execute the Node.js component
            result = await nodeServices.InvokeAsync<JSONResponse>("./pdf", "cannabis_licence", testObject, options); 
                        
            return new FileContentResult(result.data, "application/pdf");
        }
    }
}
