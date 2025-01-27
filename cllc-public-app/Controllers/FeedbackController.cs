using System;
using System.Net.Http;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using System.Net.Mail;
using System.Web;
using System.Text.RegularExpressions;

namespace Gov.Lclb.Cllb.Public.Controllers
{


    public class FeedbackModel {

        public string feedback { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class FeedbackController : ControllerBase
    {

        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IBCEPService _bcep;


        public FeedbackController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, IBCEPService bcep,
            IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(ApplicationsController));
            _env = env;
            _bcep = bcep;
        }
        [HttpPost("save-feedback")]
        public async Task<IActionResult> SaveFeedback([FromBody] FeedbackModel feedbackModel)
        {

            try
            {

                if (feedbackModel == null || string.IsNullOrWhiteSpace(feedbackModel.feedback))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "feedback  cannot be empty");

                }

                string email = _configuration["FEEDBACK_EMAIL"];
                 

                /* send the user an email confirmation. */
               
                // send the email.
                SmtpClient client = new SmtpClient(_configuration["SMTP_HOST"]);

                // Specify the message content.
                MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
                message.Subject = "CARLA Feedback";
                string sanitizedBody = HttpUtility.HtmlEncode(feedbackModel.feedback);
                if(sanitizedBody.Trim().Length < 5){
                   return StatusCode(StatusCodes.Status500InternalServerError, "Feedback too short");
                }
                message.Body = "<p>Feedback from CARLA\n: " + sanitizedBody + "</p>";
                  
                message.IsBodyHtml = true;

                try
                {
                    client.Send(message);
                     
                     return Ok(new {message = "success"});

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in SaveFeedback", ex.ToString());
                    return StatusCode(StatusCodes.Status500InternalServerError, ex);

                }
            }
            catch (HttpRequestException ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }
 
    }
}


