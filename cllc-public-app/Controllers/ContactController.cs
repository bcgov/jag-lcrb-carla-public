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
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Client;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;

        public ContactController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = distributedCache;
        }
        [HttpPost()]
        public async Task<JsonResult> CreateContact([FromBody] Contexts.Microsoft.Dynamics.CRM.Contact contact)
        {
            var myself = await _system.WhoAmI().GetValueAsync();

            var owner = await _system.Systemusers.ByKey(myself.UserId).GetValueAsync();
                        
            contact.Contactid = Guid.NewGuid();
            contact.Ownerid = owner;
            contact.Owninguser = owner;

            contact.Createdby = owner;
            
            _system.AddToContacts(contact);
           
            await _system.SaveChangesAsync();

            return Json(contact);
        }
    }
}
