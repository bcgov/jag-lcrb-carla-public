using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using System.Text.RegularExpressions;
using static Gov.Lclb.Cllb.Interfaces.SharePointFileManager;

namespace DataTool
{
    class FixSharePointNaming
    {
        public void Execute(IConfiguration config, bool doRename)
        {  
            // get a connection to Dynamics.
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(config);

            // get the list of application files.
            SharePointFileManager sharePoint = new SharePointFileManager(config);

            string[] expand = {"adoxio_licences_SharePointDocumentLocations"};
            //var licences = dynamicsClient.Licenceses.Get(expand: expand).Value;
            

            var customHeaders = new Dictionary<string, List<string>>();
            var preferHeader = new List<string>();
            preferHeader.Add($"odata.maxpagesize=5000");

            customHeaders.Add("Prefer", preferHeader);
            var odataVersionHeader = new List<string>();
            odataVersionHeader.Add("4.0");

            customHeaders.Add("OData-Version", odataVersionHeader);
            customHeaders.Add("OData-MaxVersion", odataVersionHeader);
            var licencesQuery = dynamicsClient.Licenceses.GetWithHttpMessagesAsync( expand: expand, customHeaders: customHeaders, count: true).GetAwaiter().GetResult();
            bool firstTime = true;
            int totalCount = 5000;
            int currentCount = 0;
            while (currentCount < totalCount)
            {
                Console.Out.WriteLine($"Currently on licence {currentCount} of {totalCount}");
                var licences = licencesQuery.Body.Value;
                if (firstTime)
                {
                    firstTime = false;
                    //totalCount = 0;
                }

                

                foreach (var licence in licences)
                {
                    currentCount++;
                    string folderName = licence.GetDocumentFolderName();
                    if (licence.AdoxioLicencesSharePointDocumentLocations != null &&
                        licence.AdoxioLicencesSharePointDocumentLocations.Count > 0 &&
                        licence.AdoxioLicencesSharePointDocumentLocations[0].Relativeurl != null)
                    {
                        folderName = licence.AdoxioLicencesSharePointDocumentLocations[0].Relativeurl;
                    }

                    List<FileDetailsList> fileList = null;
                    try
                    {
                        fileList = sharePoint.GetFileDetailsListInFolder(SharePointFileManager.LicenceDocumentListTitle,
                                folderName, null)
                            .GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Folder not found [{folderName}]");
                    }

                    if (fileList != null && fileList.Count > 0)
                    {
                        Console.WriteLine($"Found {fileList.Count} Files.");
                        foreach (var file in fileList)
                        {

                            // see if the file matches the regex.
                            Regex regex = new Regex("^[0-9]+_[0-9]+_([a-zA-Z]+(_[a-zA-Z]+)+) 1\\.[a-zA-Z]+$",
                                RegexOptions.IgnoreCase);
                            if (regex.IsMatch(file.Name))
                            {
                                var matches = regex.Matches(file.Name);
                                string newName =
                                    $"{matches[2].Value} {matches[3].Value}__{matches[5].Value}_{matches[4].Value}_{matches[1].Value}_{matches[0].Value}.{matches[6].Value}";
                                Console.WriteLine($"{file.Name} will be renamed to {newName}");
                            }
                        }
                    }
                }

                // get the next window.
                string odataNextLink = licencesQuery.Body.OdataNextLink;
                if (odataNextLink != null)
                {
                    licencesQuery = dynamicsClient.Licenceses.GetNextLink(odataNextLink, customHeaders);

                    totalCount += int.Parse(licencesQuery.Body.Count);
                }
               
                Console.Out.WriteLine($"Licence count is {totalCount}");
            }
        }
    }
}
