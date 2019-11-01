import { InventorySalesReport } from './inventory-sales-report.model';

export enum monthlyReportStatus {
    Draft = 1,
    Submitted = 845280001,
    Closed = 845280002
}

export class MonthlyReport {
  monthlyReportId: string;
  licenseId: string;
  licenseNumber: string;
  establishmentName: string;
  city: string;
  postalCode: string;
  reportingPeriodYear: string;
  reportingPeriodMonth: string;

  statusCode: number;
  employeesManagement: number;
  employeesAdministrative: number;
  employeesSales: number;
  employeesProduction: number;
  employeesOther: number;

  inventorySalesReports: InventorySalesReport[];
}
