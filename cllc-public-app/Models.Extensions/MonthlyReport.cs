extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces;
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
