extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Mail;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _encryptionKey;
        private readonly IDataverseClient _dataverse;

        public NewsletterController(IConfiguration configuration, IDataverseClient dataverse)
        {
            _configuration = configuration;
            _encryptionKey = _configuration["ENCRYPTION_KEY"];
            _dataverse = dataverse;
        }

        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> GetNewsletter(string slug)
        {
            var list = await _dataverse.GetMarketingListByNameAsync(slug);
            if (list == null) return new NotFoundResult();

            var newsletter = new Models.Newsletter
            {
                Id = list.Id,
                Slug = list.ListName,
                Title = list.Purpose,
                Description = list.Description
            };
            return new JsonResult(newsletter);
        }

        [HttpPost("{slug}/subscribe")]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<JsonResult> Subscribe(string slug, [FromQuery] string email)
        {
            string confirmationEmailLink = GetConfirmationLink(slug, email);
            string bclogo = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/assets/bc-logo.svg";
            string body = "<img src='" + bclogo + "'/><br><h2>Confirm your email address</h2><p>Thank you for signing up to receive updates about cannabis stores in B.C. We'll send you updates as new rules and regulations are released about selling cannabis.</p>"
                + "<p>To confirm your request and begin receiving updates by email, click here:</p>"
                + "<a href='" + confirmationEmailLink + "'>" + confirmationEmailLink + "</a>";

            SmtpClient client = new SmtpClient(_configuration["SMTP_HOST"]);
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
            var newsletterConfirmation = new NewsletterConfirmation { email = email, slug = slug };
            string json = JsonConvert.SerializeObject(newsletterConfirmation);
            result += System.Net.WebUtility.UrlEncode(EncryptionUtility.EncryptString(json, _encryptionKey));
            return result;
        }

        [HttpGet("{slug}/verifycode")]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<JsonResult> Verify(string slug, string code)
        {
            string result = "Error";
            string decrypted = EncryptionUtility.DecryptString(code, _encryptionKey);
            if (decrypted != null)
            {
                var newsletterConfirmation = JsonConvert.DeserializeObject<NewsletterConfirmation>(decrypted);
                if (slug.Equals(newsletterConfirmation.slug))
                {
                    var list = await _dataverse.GetMarketingListByNameAsync(slug);
                    if (list != null)
                    {
                        var email = newsletterConfirmation.email.ToLower();
                        var lead = await _dataverse.GetLeadByEmailAsync(email);
                        if (lead == null)
                        {
                            var newLead = new Lead { EMailAddress1 = email, FirstName = email };
                            var leadId = await _dataverse.CreateLeadAsync(newLead);
                            await _dataverse.AddLeadToMarketingListAsync(list.Id.ToString(), leadId.ToString());
                        }
                        else
                        {
                            await _dataverse.AddLeadToMarketingListAsync(list.Id.ToString(), lead.Id.ToString());
                        }
                        result = "Success";
                    }
                }
            }
            return new JsonResult(result);
        }

        [HttpPost("{slug}/unsubscribe")]
        [AllowAnonymous]
        public JsonResult UnSubscribe(string slug, [FromQuery] string email)
        {
            return new JsonResult("Ok");
        }
    }
}
