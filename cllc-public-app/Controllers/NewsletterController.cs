using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class NewsletterController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly AppDbContext db;
        private readonly string _encryptionKey;

        public NewsletterController(AppDbContext db, IConfiguration configuration)
        {
            Configuration = configuration;
            this.db = db;
            this._encryptionKey = Configuration["ENCRYPTION_KEY"];
        }
        [HttpGet("{slug}")]
        [AllowAnonymous]
        public ActionResult GetNewsletter(string slug)
        {
            
            Newsletter newsletter = null;
            if (!string.IsNullOrEmpty(Configuration["DB_USER"]))
            {
                newsletter = db.GetNewsletterBySlug(slug);
            }
                
            if (newsletter == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return Json(newsletter);
            }
            
        }

        [HttpPost("{slug}/subscribe")]
        [AllowAnonymous]
        public JsonResult Subscribe(string slug, [FromQuery] string email)
        {
            string confirmationEmailLink = GetConfirmationLink(slug, email);
            string bclogo = Configuration["BASE_URI"] + Configuration["BASE_PATH"] + "/assets/bc-logo.svg";
            /* send the user an email confirmation. */
            string body = "<img src='" + bclogo + "'/><br><h2>Confirm your email address</h2><p>Thank you for signing up to receive updates about cannabis stores in B.C. We’ll send you updates as new rules and regulations are released about selling cannabis.</p>"
                + "<p>To confirm your request and begin receiving updates by email, click here:</p>"
                + "<a href='" + confirmationEmailLink + "'>" + confirmationEmailLink + "</a>";

            // send the email.
            SmtpClient client = new SmtpClient(Configuration["SMTP_HOST"]);

            // Specify the message content.
            MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
            message.Subject = "BC LCLB Cannabis Licensing Newsletter Subscription Confirmation";
            message.Body = body;
            message.IsBodyHtml = true;
            client.Send(message);

            return Json("Ok");
        }

        private string GetConfirmationLink(string slug, string email)
        {
            string result = Configuration["BASE_URI"] + Configuration["BASE_PATH"];
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
                if (slug.Equals (newsletterConfirmation.slug))
                {
                    db.AddNewsletterSubscriber(slug, newsletterConfirmation.email);
                    result = "Success";
                }                                
            }
            return Json(result);
        }

        [HttpPost("{slug}/unsubscribe")]
        [AllowAnonymous]
        public JsonResult UnSubscribe(string slug, [FromQuery] string email)
        {
            db.RemoveNewsletterSubscriber(slug, email);
            return Json("Ok");
        }

    }
}
