using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
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
        private string encryptionKey;

        public NewsletterController(AppDbContext db, IConfiguration configuration)
        {
            Configuration = configuration;
            this.db = db;
            this.encryptionKey = Configuration["ENCRYPTION_KEY"];
        }
        [HttpGet("{slug}")]
        public JsonResult Subscribe(string slug)
        {
            Newsletter newsletter = db.GetNewsletterBySlug(slug);
            return Json(newsletter);
        }

        [HttpPost("{slug}/subscribe")]
        public JsonResult Subscribe(string slug, [FromQuery] string email)
        {
            string confirmationEmailLink = GetConfirmationLink(slug, email);
            string bclogo = Configuration["BASE_URI"] + Configuration["BASE_PATH"] + "assets/bc-logo.svg";
            /* send the user an email confirmation. */
            string body = "<img src=" + bclogo + "/><br><h2>Confirm your email address</h2><p>Thank you for signing up to receive updates about cannabis stores in B.C. We’ll send you updates as new rules and regulations are released about selling cannabis.</p>"
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
            result += "newsletter-confirm/" + slug + "?code="; 


            // create a newsletter confirmation object.

            ViewModels.NewsletterConfirmation newsletterConfirmation = new ViewModels.NewsletterConfirmation();
            newsletterConfirmation.email = email;
            newsletterConfirmation.slug = slug;

            // convert it to a json string.
            string json = JsonConvert.SerializeObject(newsletterConfirmation);

            // encrypt that using two way encryption.

            result += System.Net.WebUtility.UrlEncode(EncryptString(json, encryptionKey));

            return result;
        }

        [HttpGet("{slug}/verifycode")]
        public JsonResult Verify(string slug, string code)
        {
            string result = "Error";
            // validate the code.
            
            string decrypted = DecryptString(code, encryptionKey);
            if (decrypted != null)
            {
                // convert the json back to an object.
                ViewModels.NewsletterConfirmation newsletterConfirmation = JsonConvert.DeserializeObject<ViewModels.NewsletterConfirmation>(decrypted);
                // check that the slugs match.
                if (slug.Equals (newsletterConfirmation.slug))
                {
                    db.AddNewsletterSubscriber(slug, newsletterConfirmation.email);
                    result = "Ok";
                }                                
            }
            return Json(result);
        }

        [HttpPost("{slug}/unsubscribe")]
        public JsonResult UnSubscribe(string slug, [FromQuery] string email)
        {
            db.RemoveNewsletterSubscriber(slug, email);
            return Json("Ok");
        }

        /// <summary>
        /// Encrypt a string using AES
        /// </summary>
        /// <param name="text">The string to encrypt</param>
        /// <param name="keyString">The secret key</param>
        /// <returns></returns>
        private string EncryptString(string text, string keyString)
        {
            string result = null;

            using (Aes aes = Aes.Create())
            {
                byte[] key = Encoding.UTF8.GetBytes(keyString.Substring(0, aes.Key.Length));

                using (var encryptor = aes.CreateEncryptor(key, aes.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aes.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        byte[] byteResult = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, byteResult, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, byteResult, iv.Length, decryptedContent.Length);

                        result =  Convert.ToBase64String(byteResult);
                    }
                }
            }
            return result;
        }

        private string DecryptString(string cipherText, string keyString)
        {
            string result = null;
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            
            using (var aesAlg = Aes.Create())
            {
                var key = Encoding.UTF8.GetBytes(keyString.Substring(0, aesAlg.Key.Length));
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }                    
                }
            }
            return result;
        }
    }
}
