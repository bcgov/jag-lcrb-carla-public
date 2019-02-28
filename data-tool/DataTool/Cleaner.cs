using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTool
{
    class Cleaner
    {
        public void Clean(DynamicsClient _dynamicsClient)
        {
            // remove all incidents (waiver applications etc)
            var licences = _dynamicsClient.Licenses.Get().Value;

            foreach (var licence in licences)
            {
                try
                {
                    _dynamicsClient.Licenses.Delete(licence.AdoxioLicencesid);
                    Console.Out.WriteLine("Deleted Incident " + licence.AdoxioLicencesid);
                }
                catch (OdataerrorException odee)
                {
                    Console.Out.WriteLine("Error deleting licence");
                    Console.Out.WriteLine("Request:");
                    Console.Out.WriteLine(odee.Request.Content);
                    Console.Out.WriteLine("Response:");
                    Console.Out.WriteLine(odee.Response.Content);
                }
            }
      
            // remove all BusinessContacts.
            var applications = _dynamicsClient.Applications.Get().Value;

            foreach (var application in applications)
            {
                try
                {
                    _dynamicsClient.Applications.Delete(application.AdoxioApplicationid);
                    Console.Out.WriteLine("Deleted Application " + application.AdoxioApplicationid);
                }
                catch (OdataerrorException odee)
                {
                    Console.Out.WriteLine("Error deleting application");
                    Console.Out.WriteLine("Request:");
                    Console.Out.WriteLine(odee.Request.Content);
                    Console.Out.WriteLine("Response:");
                    Console.Out.WriteLine(odee.Response.Content);
                }
            }

            // remove all BusinessContacts.
            var legalEntities = _dynamicsClient.Adoxiolegalentities.Get().Value;

            foreach (var legalEntity in legalEntities)
            {
                try
                {
                    _dynamicsClient.Adoxiolegalentities.Delete(legalEntity.AdoxioLegalentityid);
                    Console.Out.WriteLine("Deleted LegalEntity " + legalEntity.AdoxioLegalentityid);
                }
                catch (OdataerrorException odee)
                {
                    Console.Out.WriteLine("Error deleting application");
                    Console.Out.WriteLine("Request:");
                    Console.Out.WriteLine(odee.Request.Content);
                    Console.Out.WriteLine("Response:");
                    Console.Out.WriteLine(odee.Response.Content);
                }
            }            

            var establishments = _dynamicsClient.Establishments.Get().Value;

            foreach (var item in establishments)
            {
                try
                {
                    _dynamicsClient.Establishments.Delete(item.AdoxioEstablishmentid);
                    Console.Out.WriteLine("Deleted Establishment " + item.AdoxioEstablishmentid);
                }
                catch (OdataerrorException odee)
                {
                    Console.Out.WriteLine("Error deleting Establishment");
                    Console.Out.WriteLine("Request:");
                    Console.Out.WriteLine(odee.Request.Content);
                    Console.Out.WriteLine("Response:");
                    Console.Out.WriteLine(odee.Response.Content);
                }
            }

            // remove all business profiles.
            var accounts = _dynamicsClient.Accounts.Get().Value;

            foreach (var account in accounts)
            {
                try
                {
                    _dynamicsClient.Accounts.Delete(account.Accountid);
                    Console.Out.WriteLine("Deleted BusinessProfile " + account.Accountid);
                }
                catch (OdataerrorException odee)
                {
                    Console.Out.WriteLine("Error deleting business profile");
                    Console.Out.WriteLine("Request:");
                    Console.Out.WriteLine(odee.Request.Content);
                    Console.Out.WriteLine("Response:");
                    Console.Out.WriteLine(odee.Response.Content);
                }
            }


            // remove workers

            var workers = _dynamicsClient.Workers.Get().Value;

            foreach (var item in workers)
            {
                try
                {
                    _dynamicsClient.Workers.Delete(item.AdoxioWorkerid);
                    Console.Out.WriteLine("Deleted worker " + item.AdoxioWorkerid);
                }
                catch (OdataerrorException odee)
                {
                    Console.Out.WriteLine("Error deleting worker");
                    Console.Out.WriteLine("Request:");
                    Console.Out.WriteLine(odee.Request.Content);
                    Console.Out.WriteLine("Response:");
                    Console.Out.WriteLine(odee.Response.Content);
                }
            }

            var aliases = _dynamicsClient.Aliases.Get().Value;

            foreach (var item in aliases)
            {
                try
                {
                    _dynamicsClient.Aliases.Delete(item.AdoxioAliasid);
                    Console.Out.WriteLine("Deleted alias " + item.AdoxioAliasid);
                }
                catch (OdataerrorException odee)
                {
                    Console.Out.WriteLine("Error deleting alias");
                    Console.Out.WriteLine("Request:");
                    Console.Out.WriteLine(odee.Request.Content);
                    Console.Out.WriteLine("Response:");
                    Console.Out.WriteLine(odee.Response.Content);
                }
            }


            // remove contacts
            var contacts = _dynamicsClient.Contacts.Get().Value;

            foreach (var contact in contacts)
            {
                try
                {
                    _dynamicsClient.Contacts.Delete(contact.Contactid);
                    Console.Out.WriteLine("Deleted Contact " + contact.Contactid);
                }
                catch (OdataerrorException odee)
                {
                    Console.Out.WriteLine("Error deleting contact");
                    Console.Out.WriteLine("Request:");
                    Console.Out.WriteLine(odee.Request.Content);
                    Console.Out.WriteLine("Response:");
                    Console.Out.WriteLine(odee.Response.Content);
                }
            }

            // remove invoices
            var invoices = _dynamicsClient.Invoices.Get().Value;

            foreach (var invoice in invoices)
            {
                try
                {
                    _dynamicsClient.Invoices.Delete(invoice.Invoiceid);
                    Console.Out.WriteLine("Deleted Invoice " + invoice.Invoiceid);
                }
                catch (OdataerrorException odee)
                {
                    Console.Out.WriteLine("Error deleting invoice");
                    Console.Out.WriteLine("Request:");
                    Console.Out.WriteLine(odee.Request.Content);
                    Console.Out.WriteLine("Response:");
                    Console.Out.WriteLine(odee.Response.Content);
                }
            }

        }

    }
}
