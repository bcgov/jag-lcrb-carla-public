export class InventorySalesReport {
  inventoryReportId: string;
  product: string;
  openingInventory: number;
  domesticAdditions: number;
  returnsAdditions: number;
  otherAdditions: number;
  domesticReductions: number;
  returnsReductions: number;
  destroyedReductions: number;
  lostReductions: number;
  otherReductions: number;
  closingNumber: number;
  closingValue: number;
  closingWeight: number;
  totalSeeds: number;
  totalSalesToConsumerQty: number;
  totalSalesToConsumerValue: number;
  totalSalesToRetailerQty: number;
  totalSalesToRetailerValue: number;

  productDisplayOrder: number;
  productDiscription: string;
}
