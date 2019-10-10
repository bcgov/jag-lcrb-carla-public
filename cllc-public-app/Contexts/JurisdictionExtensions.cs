using Gov.Lclb.Cllb.Public.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class AppDbContextJurisdictionExtenstions
    {
        public static void AddJurisdiction(this AppDbContext context, Jurisdiction jurisdiction)
        {
            // create a new jurisdiction.           
            context.Jurisdictions.Add(jurisdiction);
            context.SaveChanges();
        }

        public static void UpdateJurisdiction(this AppDbContext context, Jurisdiction jurisdiction)
        {
            Jurisdiction _jurisdiction = context.Jurisdictions.FirstOrDefault<Jurisdiction>(x => x.Id == jurisdiction.Id);
            _jurisdiction.Name = jurisdiction.Name;
            _jurisdiction.SelectMessage = jurisdiction.SelectMessage;
            context.Jurisdictions.Update(_jurisdiction);
            context.SaveChanges();
        }

        public static List<Jurisdiction> GetJurisdictions(this AppDbContext context)
        {
            List<Models.Jurisdiction> jurisdictions =
                context.Jurisdictions.ToList<Jurisdiction>();
            return jurisdictions;
        }

        /// <summary>
        /// Returns a specific jurisdiction
        /// </summary>
        /// <param name="name">The name of the jurisdiction</param>
        /// <returns>The jurisdiction, or null if it does not exist.</returns>
        public static Jurisdiction GetJurisdictionByName(this AppDbContext context, string name)
        {
            Jurisdiction jurisdiction = context.Jurisdictions.FirstOrDefault(x => x.Name == name);
            return jurisdiction;
        }



        /// <summary>
        /// Create jurisdictions from a (json) file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jurisdictionJsonPath"></param>
        public static void AddInitialJurisdictionsFromFile(this AppDbContext context, string jurisdictionJsonPath)
        {
            if (!string.IsNullOrEmpty(jurisdictionJsonPath) && File.Exists(jurisdictionJsonPath))
            {
                string jurisdictionJson = File.ReadAllText(jurisdictionJsonPath);
                context.AddInitialJurisdictions(jurisdictionJson);
            }
        }

        private static void AddInitialJurisdictions(this AppDbContext context, string jurisdictionJson)
        {
            List<Jurisdiction> jurisdictions = JsonConvert.DeserializeObject<List<Jurisdiction>>(jurisdictionJson);

            if (jurisdictions != null)
            {
                context.AddInitialJurisdictions(jurisdictions);
            }
        }

        private static void AddInitialJurisdictions(this AppDbContext context, List<Jurisdiction> jurisdictions)
        {
            jurisdictions.ForEach(context.AddInitialJurisdiction);
        }

        /// <summary>
        /// Adds a jurisdiction to the system, only if it does not exist.
        /// </summary>
        private static void AddInitialJurisdiction(this AppDbContext context, Jurisdiction initialJurisdiction)
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
        public static void UpdateSeedJurisdictionInfo(this AppDbContext context, Jurisdiction jurisdictionInfo)
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
