import { InventorySalesReport } from './inventory-sales-report.model';

export class MonthlyReport {
  monthlyReportId: string;
  licenseId: string;
  licenseNumber: string;
  establishmentName: string;
  city: string;
  postalCode: string;
  reportingPeriodYear: string;
  reportingPeriodMonth: string;

  employeesManagement: number;
  employeesAdministrative: number;
  employeesSales: number;
  employeesProduction: number;
  employeesOther: number;

  inventorySalesReports: InventorySalesReport[];
}
