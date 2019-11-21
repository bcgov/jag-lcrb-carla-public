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
        public static MonthlyReport ToViewModel(this MicrosoftDynamicsCRMadoxioCannabismonthlyreport dynamicsMonthlyReport, IDynamicsClient dynamicsClient)
        {
            if (dynamicsMonthlyReport == null)
            {
                return null;
            }

            MonthlyReport monthlyReportVM = new MonthlyReport()
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
            Guid? adoxioEstablishmentId = null;
            if (!string.IsNullOrEmpty(dynamicsMonthlyReport._adoxioEstablishmentidValue))
            {
                adoxioEstablishmentId = Guid.Parse(dynamicsMonthlyReport._adoxioEstablishmentidValue);
            }
            if (adoxioEstablishmentId != null)
            {
                var establishment = dynamicsClient.Establishments.GetByKey(adoxioEstablishmentId.ToString());
                monthlyReportVM.establishmentName = establishment.AdoxioName;
                monthlyReportVM.establishmentAddressCity = establishment.AdoxioAddresscity;
                monthlyReportVM.establishmentAddressPostalCode = establishment.AdoxioAddresspostalcode;
            }

            IEnumerable<MicrosoftDynamicsCRMadoxioCannabisinventoryreport> inventoryReports = dynamicsClient.GetInventoryReportsForMonthlyReport(dynamicsMonthlyReport.AdoxioCannabismonthlyreportid);
            foreach (var inventoryReport in inventoryReports)
            {
                MicrosoftDynamicsCRMadoxioCannabisproductadmin product = dynamicsClient.Cannabisproductadmins.GetByKey(inventoryReport._adoxioProductidValue);
                InventorySalesReport inv = new InventorySalesReport()
                {
                    product = product.AdoxioName,
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
                    totalSalesToRetailerValue = (inventoryReport.AdoxioTotalvalueretailer != null) ? inventoryReport.AdoxioTotalvalueretailer.Value : 0
                };
                if (product.AdoxioName != "Seeds" && product.AdoxioName != "Vegetative Cannabis")
                {
                    inv.closingWeight = (inventoryReport.AdoxioWeightofclosinginventory != null) ? inventoryReport.AdoxioWeightofclosinginventory.Value : 0;
                }
                if (product.AdoxioName == "Seeds")
                {
                    inv.totalSeeds = inventoryReport.AdoxioTotalnumberseeds;
                }
                monthlyReportVM.inventorySalesReports.Add(inv);
            }

            return monthlyReportVM;
        }
    }
}
