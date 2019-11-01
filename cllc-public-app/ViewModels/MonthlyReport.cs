using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class MonthlyReport
    {
        // shouldn't change
        public string monthlyReportId { get; set; }
        public string licenseId { get; set; }
        public string licenseNumber { get; set; }
        public string establishmentName { get; set; }
        public string establishmentAddressCity { get; set; }
        public string establishmentAddressPostalCode { get; set; }
        public string reportingPeriodYear { get; set; }
        public string reportingPeriodMonth { get; set; }

        // will change
        public int? statusCode { get; set; }
        public int? employeesManagement { get; set; }
        public int? employeesAdministrative { get; set; }
        public int? employeesSales { get; set; }
        public int? employeesProduction { get; set; }
        public int? employeesOther { get; set; }
        public List<InventorySalesReport> inventorySalesReports { get; set; }
    }
}
