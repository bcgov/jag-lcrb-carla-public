
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Microsoft.Extensions.Caching.Distributed;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class DynamicsExtensions
    {
        
        public static Contact GetContact(this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, string siteminderId)
        {
            Contact result = null;
            string key = "Contact_" + siteminderId;
            // first look in the cache.
            string temp = distributedCache.GetString(key);
            if (! string.IsNullOrEmpty(temp))
            {
                result = JsonConvert.DeserializeObject<Contact>(temp);
            }
            else
            {

            }

            return result;
        }
    }
}
