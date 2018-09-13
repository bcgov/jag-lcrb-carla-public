using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;

namespace BCeIDTool
{
    /// <summary>
    /// Utility Program to backfill data in the event of a problem with BCeID
    /// This tool can also be used to generate an export of active users
    /// </summary>
    class Program
    {
        // the one parameter is the BCeID guid for an individual.
        static void Main(string[] args)
        {
            bool isBackfill = false;
            bool isSingleQuery = false;
            string singleQuery = null;


            if (args.Length > 0)
            {
                string arg = args[0];
                if (!string.IsNullOrEmpty(arg))
                {
                    if (arg.ToLower().Equals("backfill"))
                    {
                        isBackfill = true;
                        Console.Out.WriteLine("Backfill enabled.");
                    }
                    else
                    {
                        isSingleQuery = true;
                        singleQuery = arg;
                        Console.Out.WriteLine("Single query enabled.");
                    }
                }
            }

                // get the environment settings.
                string bceidServiceSvcid = Environment.GetEnvironmentVariable("BCEID_SERVICE_SVCID");
                string bceidServiceUser = Environment.GetEnvironmentVariable("BCEID_SERVICE_USER");
                string bceidServicePasswd = Environment.GetEnvironmentVariable("BCEID_SERVICE_PASSWD");
                string bceidServiceUrl = Environment.GetEnvironmentVariable("BCEID_SERVICE_URL");

                string dynamicsOdataUri = Environment.GetEnvironmentVariable("DYNAMICS_ODATA_URI");
                string ssgUsername = Environment.GetEnvironmentVariable("SSG_USERNAME");
                string ssgPassword = Environment.GetEnvironmentVariable("SSG_PASSWORD");
                string dynamicsNativeOdataUri = Environment.GetEnvironmentVariable("DYNAMICS_NATIVE_ODATA_URI");

                BCeIDBusinessQuery bCeIDBusinessQuery = new BCeIDBusinessQuery(bceidServiceSvcid, bceidServiceUser, bceidServicePasswd, bceidServiceUrl);

                var serviceClientCredentials = new BasicAuthenticationCredentials()
                {
                    UserName = ssgUsername,
                    Password = ssgPassword
                };

                var _dynamicsClient = new DynamicsClient(new Uri(dynamicsOdataUri), serviceClientCredentials);

                if (isBackfill)
                {
                    var contacts = _dynamicsClient.Contacts.Get().Value;

                    foreach (MicrosoftDynamicsCRMcontact contact in contacts)
                    {
                        if (string.IsNullOrEmpty(contact.Emailaddress1))
                        {
                            string externalId = contact.AdoxioExternalid;
                            if (!string.IsNullOrEmpty(externalId))
                            {
                                var caller = bCeIDBusinessQuery.ProcessBusinessQuery(externalId);
                                caller.Wait();
                                Gov.Lclb.Cllb.Interfaces.BCeIDBusiness bceidBusiness = caller.Result;

                                if (bceidBusiness != null)
                                {
                                    MicrosoftDynamicsCRMcontact patchRecord = new MicrosoftDynamicsCRMcontact();
                                    if (string.IsNullOrEmpty(contact.Firstname))
                                    {
                                        patchRecord.Firstname = bceidBusiness.individualFirstname;
                                    }
                                    if (string.IsNullOrEmpty(contact.Lastname))
                                    {
                                        patchRecord.Lastname = bceidBusiness.individualSurname;
                                    }
                                    if (string.IsNullOrEmpty(contact.Middlename))
                                    {
                                        patchRecord.Middlename = bceidBusiness.individualMiddlename;
                                    }
                                    if (string.IsNullOrEmpty(contact.Emailaddress1))
                                    {
                                        patchRecord.Emailaddress1 = bceidBusiness.contactEmail;
                                    }
                                    if (string.IsNullOrEmpty(contact.Telephone1))
                                    {
                                        patchRecord.Telephone1 = bceidBusiness.contactPhone;
                                    }

                                    // update the contact.
                                    try
                                    {
                                        _dynamicsClient.Contacts.Update(contact.Contactid, patchRecord);
                                        Console.Out.WriteLine("Updated contact " + contact.Firstname + " " + contact.Lastname);
                                    }
                                    catch (OdataerrorException odee)
                                    {
                                        Console.Out.WriteLine("Error patching contact");
                                        Console.Out.WriteLine("Request:");
                                        Console.Out.WriteLine(odee.Request.Content);
                                        Console.Out.WriteLine("Response:");
                                        Console.Out.WriteLine(odee.Response.Content);
                                    }

                                }
                            }
                        }
                    }

                    IList<MicrosoftDynamicsCRMaccount> accounts = null;
                    // now get a list of all the related accounts.
                    try
                    {
                        accounts = _dynamicsClient.Accounts.Get().Value;
                    }
                    catch (OdataerrorException odee)
                    {
                        Console.Out.WriteLine("Error getting accounts");
                        Console.Out.WriteLine("Request:");
                        Console.Out.WriteLine(odee.Request.Content);
                        Console.Out.WriteLine("Response:");
                        Console.Out.WriteLine(odee.Response.Content);
                    }

                    if (accounts != null)
                    {
                        foreach (var account in accounts)
                        {
                            // get the contact.
                            string contactid = account._primarycontactidValue;
                            // only process accounts with missing email address.
                            if (!string.IsNullOrEmpty(contactid) && string.IsNullOrEmpty(account.Emailaddress1))
                            {
                                var contact = _dynamicsClient.Contacts.GetByKey(contactid);
                                string guid = contact.AdoxioExternalid;

                                if (!string.IsNullOrEmpty(guid))
                                {
                                    var caller = bCeIDBusinessQuery.ProcessBusinessQuery(guid);
                                    caller.Wait();
                                    Gov.Lclb.Cllb.Interfaces.BCeIDBusiness bceidBusiness = caller.Result;

                                    if (bceidBusiness != null)
                                    {
                                        MicrosoftDynamicsCRMaccount accountPatchRecord = new MicrosoftDynamicsCRMaccount();
                                        if (string.IsNullOrEmpty(account.Emailaddress1))
                                        {
                                            accountPatchRecord.Emailaddress1 = bceidBusiness.contactEmail;
                                        }
                                        if (string.IsNullOrEmpty(account.Telephone1))
                                        {
                                            accountPatchRecord.Telephone1 = bceidBusiness.contactEmail;
                                        }
                                        if (string.IsNullOrEmpty(account.Address1City))
                                        {
                                            accountPatchRecord.Address1City = bceidBusiness.addressCity;
                                        }
                                        if (string.IsNullOrEmpty(account.Address1Postalcode))
                                        {
                                            accountPatchRecord.Address1Postalcode = bceidBusiness.addressPostal;
                                        }
                                        if (string.IsNullOrEmpty(account.Address1Line1))
                                        {
                                            accountPatchRecord.Address1Line1 = bceidBusiness.addressLine1;
                                        }
                                        if (string.IsNullOrEmpty(account.Address1Line2))
                                        {
                                            accountPatchRecord.Address1Line2 = bceidBusiness.addressLine2;
                                        }
                                        try
                                        {
                                            _dynamicsClient.Accounts.Update(account.Accountid, accountPatchRecord);

                                            Console.Out.WriteLine("Updated account " + account.Name);

                                        }
                                        catch (OdataerrorException odee)
                                        {
                                            Console.Out.WriteLine("Error patching account");
                                            Console.Out.WriteLine("Request:");
                                            Console.Out.WriteLine(odee.Request.Content);
                                            Console.Out.WriteLine("Response:");
                                            Console.Out.WriteLine(odee.Response.Content);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (isSingleQuery)
                {
                    Console.Out.WriteLine("\"BCEID GUID\",\"Given name\",\"Surname\",\"Email\",\"Phone\",\"Business Number\"");

                    // query BCeID 
                    if (!string.IsNullOrEmpty(singleQuery))
                    {
                        var caller = bCeIDBusinessQuery.ProcessBusinessQuery(singleQuery);
                        caller.Wait();
                        var result = caller.Result;
                        if (result != null)
                        {
                            Console.Out.WriteLine("\"" + singleQuery + "\",\"" + result.individualFirstname + "\",\"" + result.individualSurname + "\",\"" + result.contactEmail + "\",\"" + result.contactPhone + "\",\"" + result.businessNumber + "\"");
                        }
                    }
                }
            
                else // get all contacts.
                {

                    // first parse the contacts file.
                    List<string> stringList = new List<string>();

                    // read from file.
                    /*
                      string jsonString = File.ReadAllText("filename");

                        var jsonData = JsonConvert.DeserializeObject<GetOKResponseModelModelModelModelModelModelModelModel>(jsonString);
                        var contacts = jsonData.Value;
                    */

                    // Get contacts from dynamics.

                    var contacts = _dynamicsClient.Contacts.Get().Value;

                    foreach (var contact in contacts)
                    {
                        stringList.Add(contact.AdoxioExternalid);
                    }

                    Console.Out.WriteLine("\"BCEID GUID\",\"Given name\",\"Surname\",\"Email\",\"Phone\",\"Business Number\"");

                    foreach (string guid in stringList)
                    {
                        // query BCeID 
                        if (!string.IsNullOrEmpty(guid))
                        {
                            var caller = bCeIDBusinessQuery.ProcessBusinessQuery(guid);
                            caller.Wait();
                            var result = caller.Result;
                            if (result != null)
                            {
                                Console.Out.WriteLine("\"" + guid + "\",\"" + result.individualFirstname + "\",\"" + result.individualSurname + "\",\"" + result.contactEmail + "\",\"" + result.contactPhone + "\",\"" + result.businessNumber + "\"");
                            }
                        }
                    }
            }
        }
    }
}
