using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Models
{
    public static  class DataAccessExtensions
    {

        /// <summary>
        /// Create jurisdictions from a (json) file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jurisdictionJsonPath"></param>
        public static void AddInitialJurisdictionsFromFile(this DataAccess context, string jurisdictionJsonPath)
        {
            if (!string.IsNullOrEmpty(jurisdictionJsonPath) && File.Exists(jurisdictionJsonPath))
            {
                string jurisdictionJson = File.ReadAllText(jurisdictionJsonPath);
                context.AddInitialJurisdictions(jurisdictionJson);
            }
        }

        private static void AddInitialJurisdictions(this DataAccess context, string jurisdictionJson)
        {
            List<Jurisdiction> jurisdictions = JsonConvert.DeserializeObject<List<Jurisdiction>>(jurisdictionJson);

            if (jurisdictions != null)
            {
                context.AddInitialJurisdictions(jurisdictions);
            }
        }

        private static void AddInitialJurisdictions(this DataAccess context, List<Jurisdiction> jurisdictions)
        {
            jurisdictions.ForEach(context.AddInitialJurisdiction);
        }

        /// <summary>
        /// Adds a jurisdiction to the system, only if it does not exist.
        /// </summary>
        private static void AddInitialJurisdiction(this DataAccess context, Jurisdiction initialJurisdiction)
        {
            Jurisdiction jurisdiction = context.GetJurisdictionByName(initialJurisdiction.Name);
            if (jurisdiction != null)
            {
                return;
            }

            jurisdiction = new Jurisdiction
            (
                initialJurisdiction.Id,
                initialJurisdiction.Name,
                initialJurisdiction.SelectMessage
            );

            context.AddJurisdiction(jurisdiction);            
        }


        /// <summary>
        /// Update region
        /// </summary>
        /// <param name="context"></param>
        /// <param name="regionInfo"></param>
        public static void UpdateSeedJurisdictionInfo(this DataAccess context, Jurisdiction jurisdictionInfo)
        {
            Jurisdiction jurisdiction = context.GetJurisdictionByName(jurisdictionInfo.Name);
            if (jurisdiction == null)
            {
                context.AddJurisdiction(jurisdictionInfo);
            }
            else
            {
                jurisdiction.Name = jurisdictionInfo.Name;
                jurisdiction.SelectMessage = jurisdictionInfo.SelectMessage;
                context.UpdateJurisdiction(jurisdiction);

            }
        }

    }
}
