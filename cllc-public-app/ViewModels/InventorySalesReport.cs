namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class InventorySalesReport
    {
        // Won't change
        public string inventoryReportId { get; set; }
        public string product { get; set; }

        // Will change
        public int? openingInventory { get; set; }
        public int? domesticAdditions { get; set; }
        public int? returnsAdditions { get; set; }
        public int? otherAdditions { get; set; }
        public int? domesticReductions { get; set; }
        public int? returnsReductions { get; set; }
        public int? destroyedReductions { get; set; }
        public int? lostReductions { get; set; }
        public int? otherReductions { get; set; }
        public int? closingNumber { get; set; }
        public decimal closingValue { get; set; }
        public decimal closingWeight { get; set; }
        // only used for seeds
        public int? totalSeeds { get; set; }

        // Sales
        public int? totalSalesToConsumerQty { get; set; }
        public decimal totalSalesToConsumerValue { get; set; }
        public int? totalSalesToRetailerQty { get; set; }
        public decimal totalSalesToRetailerValue { get; set; }

        public int? ProductDisplayOrder { get; set; }
        public string ProductDiscription { get; set; }
    }
}
