extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Models
{
    public enum MonthlyReportStatus
    {
        Draft = 1,
        Submitted = 845280001,
        Closed = 845280002
    }
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class MonthlyReportExtension
    {
        public static MonthlyReport ToViewModel(this MicrosoftDynamicsCRMadoxioCannabismonthlyreport dynamicsMonthlyReport, IDynamicsClient dynamicsClient, bool expandInventoryReports)
        {
            if (dynamicsMonthlyReport == null)
            {
                return null;
            }

            MonthlyReport monthlyReportVM = new MonthlyReport
            {
                licenseId = dynamicsMonthlyReport._adoxioLicenceidValue,
                licenseNumber = dynamicsMonthlyReport.AdoxioLicencenumber,
                reportingPeriodMonth = dynamicsMonthlyReport.AdoxioReportingperiodmonth,
                reportingPeriodYear = dynamicsMonthlyReport.AdoxioReportingperiodyear,
                statusCode = dynamicsMonthlyReport.Statuscode,
                employeesManagement = dynamicsMonthlyReport.AdoxioEmployeesmanagement,
                employeesAdministrative = dynamicsMonthlyReport.AdoxioEmployeesadministrative,
                employeesSales = dynamicsMonthlyReport.AdoxioEmployeessales,
                employeesProduction = dynamicsMonthlyReport.AdoxioEmployeesproduction,
                employeesOther = dynamicsMonthlyReport.AdoxioEmployeesother,
                inventorySalesReports = new List<InventorySalesReport>()
            };

            monthlyReportVM.monthlyReportId = dynamicsMonthlyReport.AdoxioCannabismonthlyreportid;

            // fetch the establishment and get name and address
            /*Guid? adoxioEstablishmentId = null;
            if (!string.IsNullOrEmpty(dynamicsMonthlyReport._adoxioEstablishmentidValue))
            {
                adoxioEstablishmentId = Guid.Parse(dynamicsMonthlyReport._adoxioEstablishmentidValue);
            }
            if (adoxioEstablishmentId != null)
            {
                var select = new List<string>() { "adoxio_establishmentid", "adoxio_name", "adoxio_addresscity", "adoxio_addresspostalcode" };
                var establishment = dynamicsClient.Establishments.GetByKey(adoxioEstablishmentId.ToString(), select: select);
                monthlyReportVM.establishmentName = establishment.AdoxioName;
                monthlyReportVM.establishmentAddressCity = establishment.AdoxioAddresscity;
                monthlyReportVM.establishmentAddressPostalCode = establishment.AdoxioAddresspostalcode;
            }*/
            if(dynamicsMonthlyReport.AdoxioEstablishmentId != null)
            {
                monthlyReportVM.establishmentName = dynamicsMonthlyReport.AdoxioEstablishmentId.AdoxioName;
                monthlyReportVM.establishmentAddressCity = dynamicsMonthlyReport.AdoxioEstablishmentId.AdoxioAddresscity;
                monthlyReportVM.establishmentAddressPostalCode = dynamicsMonthlyReport.AdoxioEstablishmentId.AdoxioAddresspostalcode;
            }
            if (expandInventoryReports)
            {
                IEnumerable<MicrosoftDynamicsCRMadoxioCannabisinventoryreport> inventoryReports = dynamicsClient.GetInventoryReportsForMonthlyReport(dynamicsMonthlyReport.AdoxioCannabismonthlyreportid);
                foreach (var inventoryReport in inventoryReports)
                {
                    /*var select = new List<string>() { "adoxio_cannabisproductadminid", "adoxio_name", "adoxio_description", "adoxio_displayorder" };
                    MicrosoftDynamicsCRMadoxioCannabisproductadmin product = dynamicsClient.Cannabisproductadmins.GetByKey(inventoryReport._adoxioProductidValue, select: select);
                    */
                    InventorySalesReport inv = new InventorySalesReport
                    {
                        product = inventoryReport.AdoxioProductId.AdoxioName,
                        ProductDescription = inventoryReport.AdoxioProductId.AdoxioDescription,
                        ProductDisplayOrder = inventoryReport.AdoxioProductId.AdoxioDisplayorder,
                        inventoryReportId = inventoryReport.AdoxioCannabisinventoryreportid,
                        openingInventory = inventoryReport.AdoxioOpeninginventory,
                        domesticAdditions = inventoryReport.AdoxioQtyreceiveddomestic,
                        returnsAdditions = inventoryReport.AdoxioQtyreceivedreturns,
                        otherAdditions = inventoryReport.AdoxioQtyreceivedother,
                        domesticReductions = inventoryReport.AdoxioQtyshippeddomestic,
                        returnsReductions = inventoryReport.AdoxioQtyshippedreturned,
                        destroyedReductions = inventoryReport.AdoxioQtydestroyed,
                        lostReductions = inventoryReport.AdoxioQtyloststolen,
                        otherReductions = inventoryReport.AdoxioOtherreductions,
                        closingNumber = inventoryReport.AdoxioClosinginventory,
                        closingValue = (inventoryReport.AdoxioValueofclosinginventory != null) ? inventoryReport.AdoxioValueofclosinginventory.Value : 0,
                        totalSalesToConsumerQty = Convert.ToInt32(inventoryReport.AdoxioPackagedunitsnumber),
                        totalSalesToConsumerValue = (inventoryReport.AdoxioTotalvalue != null) ? inventoryReport.AdoxioTotalvalue.Value : 0,
                        totalSalesToRetailerQty = Convert.ToInt32(inventoryReport.AdoxioPackagedunitsnumberretailer),
                        totalSalesToRetailerValue = (inventoryReport.AdoxioTotalvalueretailer != null) ? inventoryReport.AdoxioTotalvalueretailer.Value : 0,
                        otherDescription = inventoryReport.AdoxioOtherdescription
                    };
                    if (inventoryReport.AdoxioProductId.AdoxioName != "Seeds" && inventoryReport.AdoxioProductId.AdoxioName != "Vegetative Cannabis")
                    {
                        inv.closingWeight = (inventoryReport.AdoxioWeightofclosinginventory != null) ? inventoryReport.AdoxioWeightofclosinginventory.Value : 0;
                    }
                    if (inventoryReport.AdoxioProductId.AdoxioName == "Seeds")
                    {
                        inv.totalSeeds = inventoryReport.AdoxioTotalnumberseeds;
                    }
                    monthlyReportVM.inventorySalesReports.Add(inv);
                }
            }

            return monthlyReportVM;
        }

        // ---- Xrm.Sdk adoxio_cannabismonthlyreport extensions ----

        public static MonthlyReport ToViewModel(this adoxio_cannabismonthlyreport report, IList<adoxio_cannabisinventoryreport>? inventoryReports = null)
        {
            if (report == null) return null;
            var vm = new MonthlyReport
            {
                monthlyReportId = report.adoxio_cannabismonthlyreportId?.ToString(),
                licenseId = report.adoxio_LicenceId?.Id.ToString(),
                licenseNumber = report.adoxio_LicenceNumber,
                reportingPeriodMonth = report.adoxio_ReportingPeriodMonth,
                reportingPeriodYear = report.adoxio_ReportingPeriodYear,
                statusCode = (int?)report.statuscode,
                employeesManagement = report.adoxio_EmployeesManagement,
                employeesAdministrative = report.adoxio_EmployeesAdministrative,
                employeesSales = report.adoxio_EmployeesSales,
                employeesProduction = report.adoxio_EmployeesProduction,
                employeesOther = report.adoxio_EmployeesOther,
                establishmentName = report.adoxio_EstablishmentNameText,
                establishmentAddressCity = report.adoxio_City,
                establishmentAddressPostalCode = report.adoxio_PostalCode,
                inventorySalesReports = new List<InventorySalesReport>(),
            };

            if (inventoryReports != null)
            {
                foreach (var inv in inventoryReports)
                {
                    var productName = inv.adoxio_ProductId?.Name;
                    var invVm = new InventorySalesReport
                    {
                        inventoryReportId = inv.adoxio_cannabisinventoryreportId?.ToString(),
                        product = productName,
                        openingInventory = inv.adoxio_OpeningInventory,
                        domesticAdditions = inv.adoxio_QtyReceivedDomestic,
                        returnsAdditions = inv.adoxio_QtyReceivedReturns,
                        otherAdditions = inv.adoxio_QtyReceivedOther,
                        domesticReductions = inv.adoxio_QtyShippedDomestic,
                        returnsReductions = inv.adoxio_QtyShippedReturned,
                        destroyedReductions = inv.adoxio_QtyDestroyed,
                        lostReductions = inv.adoxio_QtyLostStolen,
                        otherReductions = inv.adoxio_OtherReductions,
                        closingNumber = inv.adoxio_ClosingInventory,
                        closingValue = inv.adoxio_ValueofClosingInventory?.Value,
                        totalSalesToConsumerQty = inv.adoxio_PackagedUnitsNumber.HasValue ? Convert.ToInt32(inv.adoxio_PackagedUnitsNumber.Value) : (int?)null,
                        totalSalesToConsumerValue = inv.adoxio_TotalValue?.Value,
                        totalSalesToRetailerQty = inv.adoxio_PackagedUnitsNumberRetailer.HasValue ? Convert.ToInt32(inv.adoxio_PackagedUnitsNumberRetailer.Value) : (int?)null,
                        totalSalesToRetailerValue = inv.adoxio_TotalValueRetailer?.Value,
                        otherDescription = inv.adoxio_OtherDescription,
                    };
                    if (productName != "Seeds" && productName != "Vegetative Cannabis")
                        invVm.closingWeight = inv.adoxio_WeightofClosingInventory;
                    if (productName == "Seeds")
                        invVm.totalSeeds = inv.adoxio_TotalNumberSeeds;
                    vm.inventorySalesReports.Add(invVm);
                }
            }
            return vm;
        }
    }
}
