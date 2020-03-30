using CsvHelper;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;
using Hangfire;
using Google.Protobuf;
using Gov.Lclb.Cllb.Services.FileManager;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using Hangfire.Console;
using Hangfire.Server;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gov.Lclb.Cllb.FederalReportingService
{
    public class FederalReportingController
    {
        private readonly string DOCUMENT_LIBRARY = "adoxio_federalreportexport";
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IConfiguration _configuration;
        private readonly FileManagerClient _fileManagerClient;

        private readonly ILogger _logger;
        

        public FederalReportingController(IConfiguration configuration, ILoggerFactory loggerFactory, FileManagerClient fileClient)
        {
            _configuration = configuration;
            if (_configuration["DYNAMICS_ODATA_URI"] != null)
            {
                _dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            }
            _fileManagerClient = fileClient;
            _logger = loggerFactory.CreateLogger(typeof(FederalReportingController));
        }

        public async Task ExportFederalReports(PerformContext hangfireContext)
        {
            // Get any new exports that have been created
            string filter = "adoxio_exportcompleted eq null";
            MicrosoftDynamicsCRMadoxioFederalreportexportCollection exports = _dynamicsClient.Federalreportexports.Get(filter: filter);
            if (exports.Value.Count > 0)
            {
                MicrosoftDynamicsCRMadoxioFederalreportexport export = exports.Value.FirstOrDefault();
                string exportId = export.AdoxioFederalreportexportid;
                MicrosoftDynamicsCRMadoxioFederalreportexport patchExport = new MicrosoftDynamicsCRMadoxioFederalreportexport();
                patchExport.AdoxioExporttriggered = DateTime.UtcNow;
                _dynamicsClient.Federalreportexports.Update(exportId, patchExport);
                try
                {
                    // Gather submitted reports
                    filter = $"statuscode eq {(int)MonthlyReportStatus.Submitted}";
                    var dynamicsMonthlyReports = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter);
                    List<FederalReportingMonthlyExport> monthlyReports = new List<FederalReportingMonthlyExport>();
                    foreach (MicrosoftDynamicsCRMadoxioCannabismonthlyreport report in dynamicsMonthlyReports.Value)
                    {
                        FederalReportingMonthlyExport exportVM = new FederalReportingMonthlyExport()
                        {
                            ReportingPeriodMonth = report.AdoxioReportingperiodmonth,
                            ReportingPeriodYear = report.AdoxioReportingperiodyear,
                            RetailerDistributor = report.AdoxioRetailerdistributor?.ToString() ?? "1",
                            CompanyName = report.AdoxioEstablishmentnametext,
                            SiteID = report.AdoxioSiteidnumber,
                            City = report.AdoxioCity,
                            PostalCode = report.AdoxioPostalcode,
                            ManagementEmployees = report.AdoxioEmployeesmanagement ?? 0,
                            AdministrativeEmployees = report.AdoxioEmployeesadministrative ?? 0,
                            SalesEmployees = report.AdoxioEmployeessales ?? 0,
                            ProductionEmployees = report.AdoxioEmployeesproduction ?? 0,
                            OtherEmployees = report.AdoxioEmployeesother ?? 0
                        };

                        // Get inventory reports for those submitted reports
                        filter = $"_adoxio_monthlyreportid_value eq {report.AdoxioCannabismonthlyreportid}";
                        var invResp = _dynamicsClient.Cannabisinventoryreports.Get(filter: filter);
                        foreach (MicrosoftDynamicsCRMadoxioCannabisinventoryreport inventoryReport in invResp.Value)
                        {
                            MicrosoftDynamicsCRMadoxioCannabisproductadmin product = _dynamicsClient.Cannabisproductadmins.GetByKey(inventoryReport._adoxioProductidValue);
                            exportVM.PopulateProduct(inventoryReport, product);
                        }
                        monthlyReports.Add(exportVM);

                        MicrosoftDynamicsCRMadoxioCannabismonthlyreport patchRecord = new MicrosoftDynamicsCRMadoxioCannabismonthlyreport()
                        {
                            AdoxioFederalReportExportIdODateBind = _dynamicsClient.GetEntityURI("adoxio_federalreportexports", exportId),
                            Statuscode = (int)MonthlyReportStatus.Closed
                        };
                        _dynamicsClient.Cannabismonthlyreports.Update(report.AdoxioCannabismonthlyreportid, patchRecord);

                        hangfireContext.WriteLine($"Found {monthlyReports.Count} monthly reports to export.");
                        _logger.LogInformation($"Found {monthlyReports.Count} monthly reports to export.");
                        if (monthlyReports.Count > 0)
                        {
                            string filePath = "";
                            using (var mem = new MemoryStream())
                            using (var writer = new StreamWriter(mem))
                            using (var csv = new CsvWriter(writer))
                            {
                                csv.Configuration.RegisterClassMap<FederalReportingMonthlyExportMap>();
                                csv.WriteRecords(monthlyReports);

                                writer.Flush();
                                mem.Position = 0;
                                string filename = $"{export.AdoxioExportnumber}_{DateTime.Now.ToString("yyy-MM-dd")}-CannabisTrackingReport.csv";
                                // Sanitize file name
                                Regex illegalInFileName = new Regex(@"[#%*<>?{}~¿""]");
                                filename = illegalInFileName.Replace(filename, "");
                                illegalInFileName = new Regex(@"[&:/\\|]");
                                filename = illegalInFileName.Replace(filename, "-");

                                string folderName = null;
                                MicrosoftDynamicsCRMsharepointdocumentlocation? documentLocation = null;
                                if (export.AdoxioFederalreportexportSharePointDocumentLocations != null)
                                {
                                    documentLocation = export.AdoxioFederalreportexportSharePointDocumentLocations.FirstOrDefault();
                                    folderName = documentLocation.Relativeurl;
                                }

                                if (folderName == null)
                                {
                                    folderName = export.GetDocumentFolderName();

                                    CreateFederalReportDocumentLocation(export, DOCUMENT_LIBRARY, folderName);
                                }
                                // string sharepointFilename = await _sharepoint.UploadFile(filename, DOCUMENT_LIBRARY, "", mem, "text/csv");
                                // string url = _sharepoint.GetServerRelativeURL(DOCUMENT_LIBRARY, "");
                                byte[] data = mem.ToArray();
                                // call the web service
                                var uploadRequest = new UploadFileRequest()
                                {
                                    ContentType = "text/csv",
                                    Data = ByteString.CopyFrom(data),
                                    EntityName = "federal_report",
                                    FileName = filename,
                                    FolderName = folderName
                                };

                                var uploadResult = _fileManagerClient.UploadFile(uploadRequest);
                            }
                            hangfireContext.WriteLine($"Successfully exported Federal Reporting CSV {export.AdoxioExportnumber}.");
                            _logger.LogInformation($"Successfully exported Federal Reporting CSV {export.AdoxioExportnumber}.");
                        }
                    }
                    patchExport.AdoxioExportcompleted = DateTime.UtcNow;
                    _dynamicsClient.Federalreportexports.Update(exportId, patchExport);
                }
                catch (HttpOperationException httpOperationException)
                {
                    hangfireContext.WriteLine("Error creating federal tracking CSV");
                    _logger.LogError(httpOperationException, "Error creating federal tracking CSV");
                }
            }
        }

        private async Task CreateFederalReportDocumentLocation(MicrosoftDynamicsCRMadoxioFederalreportexport federalReport, string folderName, string name)
        {

            // now create a document location to link them.

            // Create the SharePointDocumentLocation entity
            MicrosoftDynamicsCRMsharepointdocumentlocation mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation()
            {
                Relativeurl = folderName,
                Description = "Federal Report Files",
                Name = name
            };


            try
            {
                mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, "Error creating SharepointDocumentLocation");
                mdcsdl = null;
            }
            if (mdcsdl != null)
            {

                // set the parent document library.
                string parentDocumentLibraryReference = GetDocumentLocationReferenceByRelativeURL("adoxio_federalreportexport", name);

                string exportUri = _dynamicsClient.GetEntityURI("adoxio_federalreportexports", federalReport.AdoxioFederalreportexportid);
                // add a regardingobjectid.
                var patchSharePointDocumentLocationIncident = new MicrosoftDynamicsCRMsharepointdocumentlocation()
                {
                    RegardingobjectIdFederalReportExportODataBind = exportUri,
                    ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                    Relativeurl = name,
                    Description = "Federal Report Files",
                };

                try
                {
                    _dynamicsClient.Sharepointdocumentlocations.Update(mdcsdl.Sharepointdocumentlocationid, patchSharePointDocumentLocationIncident);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference SharepointDocumentLocation to federal report");
                }

                string sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", mdcsdl.Sharepointdocumentlocationid);

                Odataid oDataId = new Odataid()
                {
                    OdataidProperty = sharePointLocationData
                };

                try
                {
                    _dynamicsClient.Federalreportexports.AddReference(federalReport.AdoxioFederalreportexportid, "adoxio_federalreportexport_SharePointDocumentLocations", oDataId);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference to SharepointDocumentLocation");
                }
            }
        }

        private string GetDocumentLocationReferenceByRelativeURL(string relativeUrl, string name)
        {
            string result = null;
            string sanitized = relativeUrl.Replace("'", "''");
            // first see if one exists.
            var locations = _dynamicsClient.Sharepointdocumentlocations.Get(filter: "relativeurl eq '" + sanitized + "'");

            var location = locations.Value.FirstOrDefault();

            if (location == null)
            {
                MicrosoftDynamicsCRMsharepointdocumentlocation newRecord = new MicrosoftDynamicsCRMsharepointdocumentlocation()
                {
                    Relativeurl = relativeUrl,
                    Name = name
                };
                // create a new document location.
                try
                {
                    location = _dynamicsClient.Sharepointdocumentlocations.Create(newRecord);
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError(httpOperationException, "Error creating document location");
                }
            }

            if (location != null)
            {
                result = location.Sharepointdocumentlocationid;
            }

            return result;
        }
    }
}
