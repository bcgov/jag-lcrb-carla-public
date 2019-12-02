using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Mail;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Public API
    public class NewsletterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _encryptionKey;
        private readonly IDynamicsClient _dynamicsClient;

        public NewsletterController(IConfiguration configuration, IDynamicsClient dynamicsClient)
        {
            _configuration = configuration;
            _encryptionKey = _configuration["ENCRYPTION_KEY"];
            _dynamicsClient = dynamicsClient;
        }
        [HttpGet("{slug}")]
        [AllowAnonymous]
        public ActionResult GetNewsletter(string slug)
        {

            Newsletter newsletter = null;
            newsletter = _dynamicsClient.GetNewsletterBySlug(slug);
            
            if (newsletter == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new JsonResult(newsletter);
            }

        }

        [HttpPost("{slug}/subscribe")]
        [AllowAnonymous]
        public JsonResult Subscribe(string slug, [FromQuery] string email)
        {
            string confirmationEmailLink = GetConfirmationLink(slug, email);
            string bclogo = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/assets/bc-logo.svg";
            /* send the user an email confirmation. */
            string body = "<img src='" + bclogo + "'/><br><h2>Confirm your email address</h2><p>Thank you for signing up to receive updates about cannabis stores in B.C. We’ll send you updates as new rules and regulations are released about selling cannabis.</p>"
                + "<p>To confirm your request and begin receiving updates by email, click here:</p>"
                + "<a href='" + confirmationEmailLink + "'>" + confirmationEmailLink + "</a>";

            // send the email.
            SmtpClient client = new SmtpClient(_configuration["SMTP_HOST"]);

            // Specify the message content.
            MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
            message.Subject = "BC LCLB Cannabis Licensing Newsletter Subscription Confirmation";
            message.Body = body;
            message.IsBodyHtml = true;
            client.Send(message);

            return new JsonResult("Ok");
        }

        private string GetConfirmationLink(string slug, string email)
        {
            string result = _configuration["BASE_URI"] + _configuration["BASE_PATH"];
            result += "/newsletter-confirm/" + slug + "?code=";

            // create a newsletter confirmation object.

            ViewModels.NewsletterConfirmation newsletterConfirmation = new ViewModels.NewsletterConfirmation();
            newsletterConfirmation.email = email;
            newsletterConfirmation.slug = slug;

            // convert it to a json string.
            string json = JsonConvert.SerializeObject(newsletterConfirmation);

            // encrypt that using two way encryption.

            result += System.Net.WebUtility.UrlEncode(EncryptionUtility.EncryptString(json, _encryptionKey));

            return result;
        }

        [HttpGet("{slug}/verifycode")]
        [AllowAnonymous]
        public JsonResult Verify(string slug, string code)
        {
            string result = "Error";
            // validate the code.

            string decrypted = EncryptionUtility.DecryptString(code, _encryptionKey);
            if (decrypted != null)
            {
                // convert the json back to an object.
                ViewModels.NewsletterConfirmation newsletterConfirmation = JsonConvert.DeserializeObject<ViewModels.NewsletterConfirmation>(decrypted);
                // check that the slugs match.
                if (slug.Equals(newsletterConfirmation.slug))
                {
                    _dynamicsClient.AddNewsletterSubscriber(slug, newsletterConfirmation.email.ToLower());
                    result = "Success";
                }
            }
            return new JsonResult(result);
        }

        [HttpPost("{slug}/unsubscribe")]
        [AllowAnonymous]
        public JsonResult UnSubscribe(string slug, [FromQuery] string email)
        {
            _dynamicsClient.RemoveNewsletterSubscriber(slug, email);
            return new JsonResult("Ok");
        }

    }
}
