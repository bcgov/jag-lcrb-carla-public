extern alias DV;
using CsvHelper;
using DV::Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;
using Hangfire;
using Google.Protobuf;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using Gov.Lclb.Cllb.Services.FileManager;
using Hangfire.Console;
using Hangfire.Server;
using System.Linq;
using System.Text.RegularExpressions;


namespace Gov.Lclb.Cllb.FederalReportingService
{
    public class FederalReportingController
    {
        private readonly string DOCUMENT_LIBRARY = "adoxio_federalreportexport";
        private readonly IDataverseClient _dataverseClient;
        private readonly IConfiguration _configuration;
        private readonly FileManagerClient _fileManagerClient;
        private readonly ILogger _logger;

        public FederalReportingController(IConfiguration configuration, ILoggerFactory loggerFactory, FileManagerClient fileClient, IDataverseClient dataverseClient)
        {
            _configuration = configuration;
            _dataverseClient = dataverseClient;
            _fileManagerClient = fileClient;
            _logger = loggerFactory.CreateLogger(typeof(FederalReportingController));
        }

        public async Task ExportFederalReports(PerformContext hangfireContext)
        {
            try
            {
                var exports = await _dataverseClient.GetPendingFederalReportExportsAsync();
                if (exports.Count > 0)
                {
                    var export = exports.First();

                    var patchExport = new adoxio_federalreportexport { Id = export.Id };
                    patchExport.adoxio_ExportTriggered = DateTime.UtcNow;
                    await _dataverseClient.UpdateFederalReportExportAsync(patchExport);

                    var dynamicsMonthlyReports = await _dataverseClient.GetSubmittedCannabisMonthlyReportsAsync();
                    var monthlyReports = new List<FederalReportingMonthlyExport>();
                    hangfireContext.WriteLine($"Found {dynamicsMonthlyReports.Count} monthly reports to export.");
                    _logger.LogInformation($"Found {dynamicsMonthlyReports.Count} monthly reports to export.");

                    foreach (var report in dynamicsMonthlyReports)
                    {
                        var exportVM = new FederalReportingMonthlyExport()
                        {
                            ReportingPeriodMonth = report.adoxio_ReportingPeriodMonth,
                            ReportingPeriodYear = report.adoxio_ReportingPeriodYear,
                            RetailerDistributor = report.adoxio_RetailerDistributor?.ToString() ?? "1",
                            CompanyName = report.adoxio_EstablishmentNameText,
                            SiteID = report.adoxio_SiteIDNumber,
                            City = report.adoxio_City,
                            PostalCode = report.adoxio_PostalCode,
                            ManagementEmployees = report.adoxio_EmployeesManagement ?? 0,
                            AdministrativeEmployees = report.adoxio_EmployeesAdministrative ?? 0,
                            SalesEmployees = report.adoxio_EmployeesSales ?? 0,
                            ProductionEmployees = report.adoxio_EmployeesProduction ?? 0,
                            OtherEmployees = report.adoxio_EmployeesOther ?? 0
                        };

                        var invResp = await _dataverseClient.GetInventoryReportsByMonthlyReportIdAsync(report.Id.ToString());
                        foreach (var inventoryReport in invResp)
                        {
                            if (inventoryReport.adoxio_ProductId != null)
                            {
                                string? productName = await _dataverseClient.GetCannabisProductAdminNameByIdAsync(inventoryReport.adoxio_ProductId.Id.ToString());
                                if (productName != null)
                                    exportVM.PopulateProduct(inventoryReport, productName);
                            }
                        }
                        monthlyReports.Add(exportVM);

                        var patchRecord = new adoxio_cannabismonthlyreport { Id = report.Id };
                        patchRecord.adoxio_FederalReportExportId = new EntityReference(adoxio_federalreportexport.EntityLogicalName, export.Id);
                        patchRecord.statuscode = adoxio_cannabismonthlyreport_statuscode.Closed;
                        await _dataverseClient.UpdateCannabisMonthlyReportAsync(patchRecord);
                    }

                    if (monthlyReports.Count > 0)
                    {
                        string filename = $"{export.adoxio_ExportNumber}_{DateTime.Now:yyy-MM-dd}-CannabisTrackingReport.csv";
                        Regex illegalInFileName = new Regex(@"[#%*<>?{}~¿""]");
                        filename = illegalInFileName.Replace(filename, "");
                        illegalInFileName = new Regex(@"[&:/\\|]");
                        filename = illegalInFileName.Replace(filename, "-");
                        using (var mem = new MemoryStream())
                        using (var writer = new System.IO.StreamWriter(mem))
                        using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
                        {
                            csv.Context.RegisterClassMap<FederalReportingMonthlyExportMap>();
                            csv.WriteRecords(monthlyReports);

                            writer.Flush();
                            mem.Position = 0;

                            string? folderName = null;
                            var documentLocation = await _dataverseClient.GetSharePointDocLocByObjectIdAsync(export.Id.ToString());
                            if (documentLocation != null)
                            {
                                folderName = documentLocation.RelativeUrl;
                            }

                            if (folderName == null)
                            {
                                string entityIdCleaned = export.Id.ToString().ToUpper().Replace("-", "");
                                folderName = SanatiseRelativeUrl($"{export.adoxio_name}_{entityIdCleaned}");
                                await CreateFederalReportDocumentLocation(export, DOCUMENT_LIBRARY, folderName);
                            }

                            byte[] data = mem.ToArray();
                            var uploadRequest = new Services.FileManager.UploadFileRequest()
                            {
                                ContentType = "text/csv",
                                Data = ByteString.CopyFrom(data),
                                EntityName = "federal_report",
                                FileName = filename,
                                FolderName = SanatiseRelativeUrl(folderName)
                            };
                            bool folderResult = CreateFolder(folderName);
                            if (folderResult)
                            {
                                var uploadResult = _fileManagerClient.UploadFile(uploadRequest);
                            }
                            else
                            {
                                hangfireContext.WriteLine($"Failed to create sharepoint folder for federal report.");
                                _logger.LogInformation($"Failed to create sharepoint folder for federal report.");
                            }
                        }
                        hangfireContext.WriteLine($"Successfully exported Federal Reporting CSV {export.adoxio_ExportNumber}.");
                        _logger.LogInformation($"Successfully exported Federal Reporting CSV {export.adoxio_ExportNumber}.");
                    }

                    patchExport.adoxio_ExportCompleted = DateTime.UtcNow;
                    await _dataverseClient.UpdateFederalReportExportAsync(patchExport);
                }
            }
            catch (Exception ex)
            {
                hangfireContext.WriteLine("Error creating federal tracking CSV");
                _logger.LogError(ex, "Error creating federal tracking CSV");
            }
        }

        private async Task CreateFederalReportDocumentLocation(adoxio_federalreportexport federalReport, string folderName, string name)
        {
            var mdcsdl = new SharePointDocumentLocation
            {
                RelativeUrl = folderName,
                Description = "Federal Report Files",
                Name = name
            };

            Guid docLocId;
            try
            {
                docLocId = await _dataverseClient.CreateSharePointDocLocAsync(mdcsdl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating SharepointDocumentLocation");
                return;
            }

            Guid? parentDocLocId = await GetSharePointDocLocIdByRelativeUrlAsync("adoxio_federalreportexport", name);

            if (parentDocLocId.HasValue)
            {
                var patchDocLoc = new SharePointDocumentLocation { Id = docLocId };
                patchDocLoc.RegardingObjectId = new EntityReference(adoxio_federalreportexport.EntityLogicalName, federalReport.Id);
                patchDocLoc.ParentSiteOrLocation = new EntityReference(SharePointDocumentLocation.EntityLogicalName, parentDocLocId.Value);
                patchDocLoc.RelativeUrl = name;
                patchDocLoc.Description = "Federal Report Files";

                try
                {
                    await _dataverseClient.UpdateSharePointDocLocAsync(patchDocLoc);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding reference SharepointDocumentLocation to federal report");
                }
            }

            try
            {
                await _dataverseClient.AssociateFederalReportExportWithDocLocAsync(federalReport.Id.ToString(), docLocId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding reference to SharepointDocumentLocation");
            }
        }

        private async Task<Guid?> GetSharePointDocLocIdByRelativeUrlAsync(string relativeUrl, string name)
        {
            var locations = await _dataverseClient.GetSharePointDocLocsByRelativeUrlAndNameAsync(relativeUrl, name);
            var location = locations.FirstOrDefault();

            if (location == null)
            {
                var newRecord = new SharePointDocumentLocation
                {
                    RelativeUrl = relativeUrl,
                    Name = name
                };
                try
                {
                    var id = await _dataverseClient.CreateSharePointDocLocAsync(newRecord);
                    return id;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating document location");
                    return null;
                }
            }

            return location.Id;
        }

        private bool CreateFolder(string folderName)
        {
            try
            {
                var createFolderRequest = new CreateFolderRequest()
                {
                    EntityName = "federal_report",
                    FolderName = folderName
                };

                var createFolderResult = _fileManagerClient.CreateFolder(createFolderRequest);

                if (createFolderResult.ResultStatus == ResultStatus.Fail)
                {
                    _logger.LogError($"Error creating folder for federal report. Error is {createFolderResult.ErrorDetail}");
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error creating folder for federal report. Error is {e.Message}");
            }
            return false;
        }

        private string SanatiseRelativeUrl(string s)
        {
            var illegalInFileName = new Regex(@"[&:/\\|.]");
            s = illegalInFileName.Replace(s, "-");
            s = Regex.Replace(s, @"\s+", " ");
            return s;
        }
    }
}
