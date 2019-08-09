using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Gov.Lclb.Cllb.Public.Models;
using System;
using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Public.Authentication
{
    /// <summary>
    /// Object to track and manage the authenticated user session
    /// </summary>
    public class UserSettings
    {
        /// <summary>
        /// True if user is authenticated
        /// </summary>
        public bool UserAuthenticated { get; set; }

        /// <summary>
        /// SiteMinder User Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// SiteMinder Guid
        /// </summary>
        public string SiteMinderGuid { get; set; }
        public string SiteMinderBusinessGuid { get; set; }

        public string UserDisplayName { get; set; }
        public string BusinessLegalName { get; set; }
        public string UserType { get; set; }

        /// <summary>
        /// AuthenticatedUser User Model
        /// </summary>
        public User AuthenticatedUser { get; set; }

        public bool IsNewUserRegistration { get; set; }

        public string ContactId { get; set; }
        public string AccountId { get; set; }


        /// <summary>
        /// Worker qualification requires new contact info.
        /// </summary>
        public ViewModels.Contact NewContact { get; set; }

        /// <summary>
        /// Worker qualification requires some additional fields.
        /// </summary>
        public ViewModels.Worker NewWorker { get; set; }

        /// <summary>
        /// Check the UserSettings object and throw an exception if it is invalid.
        /// </summary>
        public void Validate()
        {
            if (AccountId == null && ContactId == null)
            {
                throw new Exception("UserSettings Validation Error:  AccountId is null and ContactID is null");
            }

            if (AccountId == null && UserType == "Business")
            {
                throw new Exception("UserSettings Validation Error:  AccountId is null");
            }

            if (ContactId == null)
            {
                throw new Exception("UserSettings Validation Error:  ContactId is null");
            }
        }

        /// <summary>
        /// Serializes UserSettings as a Json String
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            // write metadata
            string json = JsonConvert.SerializeObject(this, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
                );

            return json;
        }

        /// <summary>
        /// Create UserSettings object from a Serialized Json String
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static UserSettings CreateFromJson(string json)
        {
            UserSettings temp = JsonConvert.DeserializeObject<UserSettings>(json);
            return temp;
        }

        /// <summary>
        /// Save UserSettings to Session
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="context"></param>
        public static void SaveUserSettings(UserSettings userSettings, HttpContext context)
        {
            string temp = userSettings.GetJson();
            context.Session.SetString("UserSettings", temp);
        }

        /// <summary>
        /// Retrieve UserSettings from Session
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static UserSettings ReadUserSettings(HttpContext context)
        {
            UserSettings userSettings = new UserSettings();

            if (context.Session.GetString("UserSettings") == null) return userSettings;

            string settingsTemp = context.Session.GetString("UserSettings");

            return !string.IsNullOrEmpty(settingsTemp) ? CreateFromJson(settingsTemp) : userSettings;
        }

        public void CopyContactSettingsOver(MicrosoftDynamicsCRMcontact contact)
        {
            contact.Address1Line1 = NewContact.address1_line1;
            contact.Address1Postalcode = NewContact.address1_postalcode;
            contact.Address1City = NewContact.address1_city;
            contact.Address1Stateorprovince = NewContact.address1_stateorprovince;
            contact.Address1Country = NewContact.address1_country;

            contact.Firstname = NewContact.firstname;
            contact.Middlename = NewContact.middlename;
            contact.Lastname = NewContact.lastname;

            contact.Emailaddress1 = NewContact.emailaddress1;
            contact.Gendercode = (int?)NewContact.Gender;
            contact.Birthdate = NewContact.Birthdate;
        }
    }
}

