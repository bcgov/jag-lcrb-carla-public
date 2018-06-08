using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using Gov.Lclb.Cllb.Public.Utility;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AdoxioLegalEntityController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Interfaces.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;
        private readonly SharePointFileManager _sharePointFileManager;
        private readonly string _encryptionKey;

        public AdoxioLegalEntityController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache, SharePointFileManager sharePointFileManager)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = null; // distributedCache;
            this._sharePointFileManager = sharePointFileManager;
            this._encryptionKey = Configuration["ENCRYPTION_KEY"];
        }

        /// <summary>
        /// Get all Legal Entities
        /// </summary>
        /// <param name="shareholder"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLegalEntities() //bool? shareholder
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<Adoxio_legalentity> legalEntities = null;
            legalEntities = await _system.Adoxio_legalentities.ExecuteAsync();

            //if (shareholder == null)
            //{
            //    legalEntities = await _system.Adoxio_legalentities.ExecuteAsync();
            //}
            //else
            //{
            //    // Shareholders have an adoxio_position value of x.
            //    legalEntities = await _system.Adoxio_legalentities
            //        .AddQueryOption("$filter", "adoxio_position eq 1") // 1 is a Shareholder
            //        .ExecuteAsync();
            //}

            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }

            return Json(result);
        }

        /// <summary>
        /// Get all Legal Entities where the position matches the parameter received
        /// </summary>
        /// <param name="positionType"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("position/{positionType}")]
        public async Task<JsonResult> GetDynamicsLegalEntitiesByPosition(string positionType)
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<Adoxio_legalentity> legalEntities = null;
            String filter = null;

            if (positionType == null)
            {
                legalEntities = await _system.Adoxio_legalentities.ExecuteAsync();
            }
            else
            {   /*
                Partner     = adoxio_position value of 0
                Shareholder = adoxio_position value of 1
                Trustee     = adoxio_position value of 2
                Director    = adoxio_position value of 3
                Officer     = adoxio_position value of 4
                Owner       = adoxio_position value of 5 
                */
                positionType = positionType.ToLower();
                switch (positionType)
                {
                    case "partner":
                        filter = "adoxio_position eq 0";
                        break;
                    case "shareholder":
                        filter = "adoxio_position eq 1";
                        break;
                    case "trustee":
                        filter = "adoxio_position eq 2";
                        break;
                    case "director":
                        filter = "adoxio_position eq 3";
                        break;
                    case "officer":
                        filter = "adoxio_position eq 4";
                        break;
                    case "owner":
                        filter = "adoxio_position eq 5";
                        break;
                    case "directorofficer":
                        filter = "adoxio_position eq 3 or adoxio_position eq 4";
                        break;
                }

                // Execute query if filter is valid
                if (filter != null)
                {
                    legalEntities = await _system.Adoxio_legalentities
                    .AddQueryOption("$filter", filter)
                    .ExecuteAsync();
                }
            }

            if (legalEntities != null)
            {
                foreach (var legalEntity in legalEntities)
                {
                    result.Add(legalEntity.ToViewModel());
                }
            }

            return Json(result);
        }

        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDynamicsLegalEntity(string id)
        {
            ViewModels.AdoxioLegalEntity result = null;
            // query the Dynamics system to get the legal entity record.

            Guid? adoxio_legalentityid = new Guid(id);
            Adoxio_legalentity legalEntity = null;
            if (adoxio_legalentityid != null)
            {
                try
                {
                    legalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid: adoxio_legalentityid).GetValueAsync();
                    result = legalEntity.ToViewModel();
                }
                catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                {
                    return new NotFoundResult();
                }
            }

            return Json(result);
        }

        // get a list of files associated with this legal entity.
        [HttpGet("{id}/attachments")]
        public async Task<IActionResult> GetFiles([FromRoute] string id)
        {
            List<ViewModels.FileSystemItem> result = new List<ViewModels.FileSystemItem>();
            // get the LegalEntity.
            Adoxio_legalentity legalEntity = null;

            if (id != null)
            {
                Guid adoxio_legalentityid = new Guid(id);
                try
                {
                    legalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid: adoxio_legalentityid).GetValueAsync();
                    string sanitized = legalEntity.Adoxio_name.Replace(" ", "_");
                    string folder_name = "LegalEntity_Files_" + sanitized;
                    // Get the folder contents for this Legal Entity.
                    List<MS.FileServices.FileSystemItem> items = await _sharePointFileManager.GetFilesInFolder("Documents",folder_name);
                    foreach (MS.FileServices.FileSystemItem item in items)
                    {
                        result.Add(item.ToViewModel());
                    }
                }
                catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                {
                    return new NotFoundResult();
                }
            }

            return Json(result);
        }

        [HttpPost("{id}/attachments")]
        public async Task<IActionResult> UploadFile([FromRoute] string id, [FromForm]IFormFile file, [FromForm] string documentType)
        {
            ViewModels.FileSystemItem result = null;
            // get the LegalEntity.
            Adoxio_legalentity legalEntity = null;

            if (id != null)
            {
                Guid adoxio_legalentityid = new Guid(id);
                try
                {
                    legalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid: adoxio_legalentityid).GetValueAsync();
                    // process the upload.
                    string fileName = FileSystemItemExtensions.CombineNameDocumentType(file.FileName, documentType);
                    string sanitized = legalEntity.Adoxio_name.Replace(" ", "_");
                    string folderName = "LegalEntity_Files_" + sanitized;

                    await _sharePointFileManager.AddFile(folderName, fileName, file.OpenReadStream() , file.ContentType);
                }
                catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                {
                    return new NotFoundResult();
                }
            }
            return Json(result);
        }

        [HttpGet("{id}/attachments/{fileId}")]
        public async Task<IActionResult> DownloadFile([FromRoute] string id, [FromRoute] string fileId)
        {
            // get the file.
            if (fileId == null)
            {
                return BadRequest();
            }
            else
            {
                _sharePointFileManager.GetFileById(fileId);
            }
            string filename = "";
            byte[] fileContents = new byte[10];
            return new FileContentResult(fileContents, "application/octet-stream")
            {
                FileDownloadName = filename
            };
        }

        /// <summary>
        /// Create a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateDynamicsLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item)
        {
            // create a new legal entity.
            Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity adoxioLegalEntity = new Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity();

            // create a DataServiceCollection to add the record
            DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity> LegalEntityCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity>(_system);

            // add a new contact.
            //LegalEntityCollection.Add(adoxioLegalEntity);
            // force a primary key to be generated by dynamics.
            item.id = null;
                      
            adoxioLegalEntity.CopyValues(item);
            _system.AddToAdoxio_legalentities(adoxioLegalEntity);
            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            DataServiceResponse dsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithIndependentOperations);            
            foreach (OperationResponse result in dsr)
            {
                if (result.StatusCode == 500) // error
                {
                    return StatusCode(500, result.Error.Message);
                }                
            }
            // get the primary key assigned by Dynamics.
            adoxioLegalEntity.Adoxio_legalentityid = dsr.GetAssignedId();

            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Update a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDynamicsLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item, string id)
        {
            if (id != item.id)
            {
                return BadRequest();
            }
            DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity> LegalEntityCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity>(_system);

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            Adoxio_legalentity adoxioLegalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid).GetValueAsync();

            _system.UpdateObject(adoxioLegalEntity);
            // copy values over from the data provided
            adoxioLegalEntity.CopyValues(item);
           
            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            DataServiceResponse dsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithIndependentOperations); // SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset
            foreach (OperationResponse result in dsr)
            {
                if (result.StatusCode == 500) // error
                {
                    return StatusCode(500, result.Error.Message);
                }
            }
            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Generate a link to be sent to an email address.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="individualId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private string GetConsentLink(string email, string individualId, string parentId)
        {
            string result = Configuration["BASE_URI"] + Configuration["BASE_PATH"];
            
            result += "/security-consent/" + parentId + "/" + individualId +"?code=";

            // create a newsletter confirmation object.

            ViewModels.SecurityConsentConfirmation securityConsentConfirmation = new ViewModels.SecurityConsentConfirmation()
            {
                email = email,
                parentid = parentId,
                individualid = individualId
            };            

            // convert it to a json string.
            string json = JsonConvert.SerializeObject(securityConsentConfirmation);

            // encrypt that using two way encryption.

            result += System.Net.WebUtility.UrlEncode(EncryptionUtility.EncryptString(json, _encryptionKey));

            return result;
        }

        [HttpGet("{id}/verifyconsentcode/{individualid}")]        
        public JsonResult VerifyConsentCode (string id, string individualid, string code)
        {
            string result = "Error";
            // validate the code.

            string decrypted = EncryptionUtility.DecryptString(code, _encryptionKey);
            if (decrypted != null)
            {
                // convert the json back to an object.
                ViewModels.SecurityConsentConfirmation consentConfirmation = JsonConvert.DeserializeObject<ViewModels.SecurityConsentConfirmation>(decrypted);
                // check that the keys match.
                if (id.Equals (consentConfirmation.parentid) && individualid.Equals(consentConfirmation.individualid) )
                {
                    // update the appropriate dynamics record here.
                    result = "Success";
                }
            }
            return Json(result);
        }


        /// <summary>
        /// send consent requests to the supplied list of legal entities.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost("{id}/sendconsentrequests")]
        public async Task<IActionResult> SendConsentRequests(string id, [FromBody] List<string> recipientIds)
        {
            // start by getting the record for the current legal entity.

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            try
            {
                Adoxio_legalentity adoxioLegalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid).GetValueAsync();
                
                // now get each of the supplied ids and send an email to them.

                foreach (string recipientId in recipientIds)
                {
                    Guid recipientIdGuid = new Guid(recipientId);
                    try
                    {
                        Adoxio_legalentity recipientEntity = await _system.Adoxio_legalentities.ByKey(recipientIdGuid).GetValueAsync();
                        string email = recipientEntity.Adoxio_email;
                        string firstname = recipientEntity.Adoxio_firstname;
                        string lastname = recipientEntity.Adoxio_lastname;

                        string confirmationEmailLink = GetConsentLink(email, recipientId, id);
                        string bclogo = Configuration["BASE_URI"] + Configuration["BASE_PATH"] + "/assets/bc-logo.svg";
                        /* send the user an email confirmation. */
                        string body = "<img src='" + bclogo + "'/><br><h2>Security Check Consent</h2>"
                            + "<p>Please confirm your security consent by clicking this link:</p>"
                            + "<a href='" + confirmationEmailLink + "'>" + confirmationEmailLink + "</a>";

                        // send the email.
                        SmtpClient client = new SmtpClient(Configuration["SMTP_HOST"]);

                        // Specify the message content.
                        MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
                        message.Subject = "BC LCLB Cannabis Licensing Security Consent";
                        message.Body = body;
                        message.IsBodyHtml = true;
                        //client.Send(message);
                    }
                    catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                    {
                        // ignore any not found errors.
                    }

                }

            }
            catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
            {
                return new NotFoundResult();
            }

            return NoContent(); // 204
        }

        /// <summary>
        /// Delete a legal entity.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteDynamicsLegalEntity(string id)
        {
            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            try
            {
                Adoxio_legalentity adoxioLegalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid).GetValueAsync();
                _system.DeleteObject(adoxioLegalEntity);
                DataServiceResponse dsr = await _system.SaveChangesAsync();
                foreach (OperationResponse result in dsr)
                {
                    if (result.StatusCode == 500) // error
                    {
                        return StatusCode(500, result.Error.Message);
                    }
                }
            }
            catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
            {
                return new NotFoundResult();
            }            

            return NoContent(); // 204
        }
    }
}
