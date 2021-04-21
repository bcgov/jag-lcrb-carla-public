using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.IO;
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
        private bool IsSuspect(string data)
        {
            bool result = false;
            if (!data.Contains("__"))
            {
                result = true;
            }

            return result;
        }

        private string FixName(string data)
        {
            string result = null;
            // Determine if it is just a Floor_Plan.pdf
            if (data.ToLower().Contains("floor_plan.pdf") || data.ToLower().Contains("floor plan.pdf") || data.ToLower().Contains("floor plans.pdf"))
            {
                // ignore.
            }
            else
            {
                // Remove "Floor_Plan"
                int floorPos = data.ToLower().IndexOf("floor_plan");
                if (floorPos > -1)
                {
                    data = data.Substring(0, floorPos) + data.Substring(floorPos + 11);
                    int extensionPos = data.LastIndexOf(".");
                    string extension = data.Substring(extensionPos);
                    data = data.Substring(0, extensionPos);

                    // sometimes a space is used instead of an underscore.
                    data = data.Replace(" ", "_");

                    var splitData = data.Split("_");
                    // reverse the strings.
                    result = "Floor Plan__";
                    for (var i = splitData.Length; i > 0; i--)
                    {
                        result += splitData[i - 1];
                        if (i > 1)
                        {
                            result += "_";
                        }
                    }

                    result += extension;
                }
                
            }

            return result;
        }

        public void Execute(IConfiguration config, bool doRename)
        {  
            // get a connection to Dynamics.
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(config);

            // get the list of application files.
            SharePointFileManager sharePoint = new SharePointFileManager(config);
            string[] orderby = {"adoxio_licencenumber"};
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
            string odataNextLink = "1";
            bool firstTime = true;
            int totalCount = 5000;
            int currentCount = 0;
            int renameCount = 0;
            HttpOperationResponse<MicrosoftDynamicsCRMadoxioLicencesCollection> licencesQuery = new HttpOperationResponse<MicrosoftDynamicsCRMadoxioLicencesCollection>();
            while (odataNextLink != null)
            {
                if (firstTime)
                {
                    firstTime = false;
                    licencesQuery = dynamicsClient.Licenceses.GetWithHttpMessagesAsync(expand: expand, customHeaders: customHeaders, count: true, orderby: orderby).GetAwaiter().GetResult();
                }
                else
                {
                    odataNextLink = licencesQuery.Body.OdataNextLink;
                    if (odataNextLink != null)
                    {
                        licencesQuery = dynamicsClient.Licenceses.GetNextLink(odataNextLink, customHeaders);

                        totalCount += licencesQuery.Body.Value.Count;
                    }
                    else
                    {
                        licencesQuery = new HttpOperationResponse<MicrosoftDynamicsCRMadoxioLicencesCollection>();
                    }
                }
                Console.Out.WriteLine($"Currently on licence {currentCount} of {totalCount}");
                if (licencesQuery?.Body?.Value != null)
                {
                    var licences = licencesQuery.Body.Value;


                    foreach (var licence in licences)
                    {
                        bool isInRange = false;
                        int licenceNumber = -1;
                        int.TryParse(licence.AdoxioLicencenumber, out licenceNumber);

                        if (licenceNumber >= 1114 && licenceNumber <= 18573)
                        {
                            isInRange = true;
                            //Console.Out.WriteLine($"Licence #{licenceNumber}");
                        }
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
                            fileList = sharePoint.GetFileDetailsListInFolder(SharePointFileManager.LicenceDocumentUrlTitle,
                                    folderName, null)
                                .GetAwaiter().GetResult();
                        }
                        catch (Exception e)
                        {
                            // Console.WriteLine($"Folder not found [{folderName}]");
                        }

                        if (fileList != null && fileList.Count > 0)
                        {
                            //Console.WriteLine($"Found {fileList.Count} Files.");
                            foreach (var file in fileList)
                            {
                                if (isInRange)
                                {
                                    // Console.Out.WriteLine($"Current filename: {file.Name}");
                                }

                                if (IsSuspect(file.Name))
                                {

                                    string newName = FixName(file.Name);
                                    if (newName != null)
                                    {
                                        Console.Out.WriteLine($"Filename {file.Name} is suspect.");
                                        Console.Out.WriteLine($"New name is {newName}");

                                        string oldFileName =
                                            $"/{SharePointFileManager.LicenceDocumentUrlTitle}/{folderName}/{file.Name}";
                                        string newFileName = $"{SharePointFileManager.LicenceDocumentUrlTitle}/{folderName}/{newName}";
                                        Console.Out.WriteLine($"Rename File {oldFileName} to {newFileName}");
                                        byte[] data = sharePoint.DownloadFile(oldFileName).GetAwaiter().GetResult();
                                        if (data != null)
                                        {
                                            var success = sharePoint.UploadFile(newName,
                                                    SharePointFileManager.LicenceDocumentUrlTitle, folderName, data, "application/pdf").GetAwaiter().GetResult();
                                            if (success != null)
                                            {
                                                // cleanup the old file.
                                                sharePoint.DeleteFile(oldFileName).GetAwaiter().GetResult();
                                                Console.Out.WriteLine($"Rename File Complete");
                                            }

                                        }

                                        renameCount++;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            Console.Out.WriteLine($"Licence count is {totalCount}");
            Console.Out.WriteLine($"Rename count is {renameCount}");
        }
    }
}
