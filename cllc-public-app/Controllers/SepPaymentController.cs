using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;

using Gov.Lclb.Cllb.Public.Models;

using System.Web;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    public class SepPayment
    {
        public string PermitNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string EventName { get; set; }
        public string DrinkServings { get; set; }
        public string DrinkPrice { get; set; }
        public string DrinkRevenue { get; set; }
        public string DrinkCost { get; set; }
        public string DrinkCorrectedCost { get; set; }
        public string DrinkCostDiff { get; set; }
        public string DrinkCostPSTDiff { get; set; }
        public string SEPId { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SepPaymentController : ControllerBase
    {
        private readonly string _Password;
 
        private  string _encryptedJson;
        private readonly IConfiguration _configuration;
        
        private readonly IWebHostEnvironment _env;

        public SepPaymentController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
            _Password = _configuration["SEP_FILE_PASSWORD"];

        }

        // [HttpGet("encrypted")]
        // public IActionResult GetEncrypted()  {
        //     var filePath = Path.Combine(_env.ContentRootPath, "sep.json");
        //     if (!System.IO.File.Exists(filePath))
        //         throw new FileNotFoundException($"Could not find sep.json at '{filePath}'.");
        //      var plainJson = System.IO.File.ReadAllText(filePath);

        //   var   _encFileJson = CryptoHelper.Encrypt(plainJson, _Password);
        //     return Ok(_encFileJson);
        // }



        [HttpGet]
        public IActionResult Get([FromQuery] string sepId = null, [FromQuery] string txnId = null)
        {
            
            if (string.IsNullOrWhiteSpace(txnId))
                return NotFound(new { message = "Required paramter is missing." });

            if (string.IsNullOrWhiteSpace(sepId))
                return NotFound(new { message = "Required paramter is missing." });
            //load the encrypted JSON file from the content root path
            // var encPath = Path.Combine(_env.ContentRootPath, "permitdata");
            // if (!System.IO.File.Exists(encPath))
            //     throw new FileNotFoundException($"Could not find encrypted data.");
           // _encryptedJson = System.IO.File.ReadAllText(encPath).Trim();
            var plainJson = CryptoHelper.Decrypt(PermitCache.PermitData, _Password);

            var payments = JsonConvert.DeserializeObject<List<SepPayment>>(plainJson)
                           ?? new List<SepPayment>();

            var match = payments.FirstOrDefault(p =>
                         string.Equals(p.SEPId, sepId, StringComparison.OrdinalIgnoreCase));

            if (match == null)
                return NotFound(new { message = $"No data found." });
              string email = _configuration["SEP_TRANSACTION_EMAIL"];
                 

                /* send the user an email confirmation. */
               
                // send the email.
                SmtpClient client = new SmtpClient(_configuration["SMTP_HOST"]);

                // Specify the message content.
                MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
                message.Subject = "Payment Information for SEP Permit number" + match.PermitNumber;
                message.Body = "<p>A payment has been made for SEP Permit number " +  match.PermitNumber + "</p>  " +
                               "<p>Contact Name: " + match.ContactName + "</p>" +
                               "<p>Contact Email: " + match.ContactEmail + "</p>" +
                               "<p>Contact Phone: " + match.ContactPhone + "</p>" +
                               "<p>Event Name: " + match.EventName + "</p>" +
                               "<p>Additional PST paid: " + match.DrinkCostPSTDiff + "</p>" +
                               "<p>Transaction ID: " + txnId + "</p>";
                  
                message.IsBodyHtml = true;

                try
                {
                    client.Send(message);
                    

                }
                catch (Exception ex)
                {
                   

                }
            return Ok(match);
        }

              private static class CryptoHelper
        {
            public static string Encrypt(string plainText, string password)
            {
                var salt = RandomNumberGenerator.GetBytes(16);
                var key  = new Rfc2898DeriveBytes(password, salt, 100_000,
                              HashAlgorithmName.SHA256).GetBytes(32);

                using var aes = Aes.Create();
                aes.KeySize = 256;
                aes.Key     = key;
                aes.Mode    = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();

                using var enc = aes.CreateEncryptor();
                var plainBytes  = Encoding.UTF8.GetBytes(plainText);
                var cipherBytes = enc.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                var packed = salt                  // 16
                             .Concat(aes.IV)       // 16
                             .Concat(cipherBytes)  // n
                             .ToArray();

                return Convert.ToBase64String(packed);
            }

            public static string Decrypt(string cipherText, string password)
            {
                var packed = Convert.FromBase64String(cipherText);

                var salt   = packed.AsSpan(0, 16).ToArray();
                var iv     = packed.AsSpan(16, 16).ToArray();
                var cipher = packed.AsSpan(32).ToArray();

                var key = new Rfc2898DeriveBytes(password, salt, 100_000,
                              HashAlgorithmName.SHA256).GetBytes(32);

                using var aes = Aes.Create();
                aes.KeySize = 256;
                aes.Key     = key;
                aes.Mode    = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV      = iv;

                using var dec = aes.CreateDecryptor();
                var plainBytes = dec.TransformFinalBlock(cipher, 0, cipher.Length);

                return Encoding.UTF8.GetString(plainBytes);
            }
        }
    }
}
