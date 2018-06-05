using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Gov.Lclb.Cllb.Public.Models;
using System;
using System.IO;
using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.Extensions.Caching.Distributed;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;

namespace Gov.Lclb.Cllb.Public.Seeders
{
    public class DynamicsSystemFormseeder : Seeder<AppDbContext>
    {
        private readonly string[] _profileTriggers = { AllProfiles };
        private readonly IDistributedCache _distributedCache;
        private readonly Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM.System _system;

        public DynamicsSystemFormseeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory, Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache) 
            : base(configuration, env, loggerFactory)
        {
            _distributedCache = distributedCache;            
            _system = system;
        }

        protected override IEnumerable<string> TriggerProfiles => _profileTriggers;

        protected override void Invoke(AppDbContext context)
        {
            if (_distributedCache != null)
            {
                UpdateDynamicsSystemForms();
            }
        }

        public async void UpdateDynamicsSystemForms()
        {
            // only get the dynamics data if we have a dynamics config.
            if (! string.IsNullOrEmpty(Configuration["DYNAMICS_ODATA_URI"]) && _distributedCache != null)
            {
                var systemForms = await _system.Systemforms.ExecuteAsync();                

                foreach (var item in systemForms)
                {
                    if (item.Formidunique != null)
                    {
                        string id = item.Formidunique.ToString();
                        string entityKey = "SystemForm_" + id + "_Entity";
                        string nameKey = "SystemForm_" + id + "_Name";
                        string xmlKey = "SystemForm_" + id + "_FormXML";

                        await _distributedCache.SetStringAsync(entityKey, item.Objecttypecode);
                        await _distributedCache.SetStringAsync(nameKey, item.Name);
                        await _distributedCache.SetStringAsync(xmlKey, item.Formxml);
                    }
                }
            }                        
        }        
    }
}
