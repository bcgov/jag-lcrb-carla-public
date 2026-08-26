extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.FederalReportingService
{
    public class FederalReportingMonthlyExport
    {
        public string ReportingPeriodYear;
        public string ReportingPeriodMonth;
        public string RetailerDistributor;
        public string CompanyName;
        public string SiteID;
        public string City;
        public string PostalCode;
        public int ManagementEmployees;
        public int AdministrativeEmployees;
        public int SalesEmployees;
        public int ProductionEmployees;
        public int OtherEmployees;

        //Seeds
        public int SeedsPackagedOpeningInventory = 0;
        public int SeedsPackagedAdditionsReceivedDomestic = 0;
        public int SeedsPackagedAdditionsReceivedReturned = 0;
        public int SeedsPackagedAdditionsOther = 0;
        public int SeedsPackagedReductionsShippedDomestic = 0;
        public int SeedsPackagedReductionsShippedReturned = 0;
        public int SeedsPackagedReductionsDestroyed = 0;
        public int SeedsPackagedReductionsLostStolen = 0;
        public int SeedsPackagedReductionsOther = 0;
        public int SeedsPackagedClosingInventoryTotal = 0;
        public double SeedsPackagedClosingInventoryTotalValue = 0;
        public int SeedsPackagedClosingTotalSeeds = 0;
        public double BCSeedsPackagedUnitsSold = 0;
        public double BCSeedsTotalValueSold = 0;
        public double SeedsPackagedUnitsSold = 0;
        public double SeedsTotalValueSold = 0;

        //Vegetative Cannabis
        public int VegetativeCannabisPackagedOpeningInventory = 0;
        public int VegetativeCannabisPackagedAdditionsReceivedDomestic = 0;
        public int VegetativeCannabisPackagedAdditionsReceivedReturned = 0;
        public int VegetativeCannabisPackagedAdditionsOther = 0;
        public int VegetativeCannabisPackagedReductionsShippedDomestic = 0;
        public int VegetativeCannabisPackagedReductionsShippedReturned = 0;
        public int VegetativeCannabisPackagedReductionsDestroyed = 0;
        public int VegetativeCannabisPackagedReductionsLostStolen = 0;
        public int VegetativeCannabisPackagedReductionsOther = 0;
        public int VegetativeCannabisPackagedClosingInventoryTotal = 0;
        public double VegetativeCannabisPackagedClosingInventoryTotalValue = 0;
        public double BCVegetativeCannabisPackagedUnitsSold = 0;
        public double BCVegetativeCannabisTotalValueSold = 0;
        public double VegetativeCannabisPackagedUnitsSold = 0;
        public double VegetativeCannabisTotalValueSold = 0;

        // Dried Cannabis
        public int DriedCannabisPackagedOpeningInventory = 0;
        public int DriedCannabisPackagedAdditionsReceivedDomestic = 0;
        public int DriedCannabisPackagedAdditionsReceivedReturned = 0;
        public int DriedCannabisPackagedAdditionsOther = 0;
        public int DriedCannabisPackagedReductionsShippedDomestic = 0;
        public int DriedCannabisPackagedReductionsShippedReturned = 0;
        public int DriedCannabisPackagedReductionsDestroyed = 0;
        public int DriedCannabisPackagedReductionsLostStolen = 0;
        public int DriedCannabisPackagedReductionsOther = 0;
        public int DriedCannabisPackagedClosingInventory = 0;
        public double DriedCannabisPackagedClosingInventoryValue = 0;
        public double DriedCannabisPackagedClosingInventoryWeight = 0;
        public double BCDriedCannabisPackagedUnitsSold = 0;
        public double BCDriedCannabisTotalValueSold = 0;
        public double DriedCannabisPackagedUnitsSold = 0;
        public double DriedCannabisTotalValueSold = 0;

        // Fresh Cannabis
        public int FreshCannabisPackagedOpeningInventory = 0;
        public int FreshCannabisPackagedAdditionsReceivedDomestic = 0;
        public int FreshCannabisPackagedAdditionsReceivedReturned = 0;
        public int FreshCannabisPackagedAdditionsOther = 0;
        public int FreshCannabisPackagedReductionsShippedDomestic = 0;
        public int FreshCannabisPackagedReductionsShippedReturned = 0;
        public int FreshCannabisPackagedReductionsDestroyed = 0;
        public int FreshCannabisPackagedReductionsLostStolen = 0;
        public int FreshCannabisPackagedReductionsOther = 0;
        public int FreshCannabisPackagedClosingInventory = 0;
        public double FreshCannabisPackagedClosingInventoryValue = 0;
        public double FreshCannabisPackagedClosingInventoryWeight = 0;
        public double BCFreshCannabisPackagedUnitsSold = 0;
        public double BCFreshCannabisTotalValueSold = 0;
        public double FreshCannabisPackagedUnitsSold = 0;
        public double FreshCannabisTotalValueSold = 0;

        // Solid Edibles
        public int SolidEdiblesPackagedOpeningInventory = 0;
        public int SolidEdiblesPackagedAdditionsReceivedDomestic = 0;
        public int SolidEdiblesPackagedAdditionsReceivedReturned = 0;
        public int SolidEdiblesPackagedAdditionsOther = 0;
        public int SolidEdiblesPackagedReductionsShippedDomestic = 0;
        public int SolidEdiblesPackagedReductionsShippedReturned = 0;
        public int SolidEdiblesPackagedReductionsDestroyed = 0;
        public int SolidEdiblesPackagedReductionsLostStolen = 0;
        public int SolidEdiblesPackagedReductionsOther = 0;
        public int SolidEdiblesPackagedClosingInventory = 0;
        public double SolidEdiblesPackagedClosingInventoryValue = 0;
        public double SolidEdiblesPackagedClosingInventoryWeight = 0;
        public double BCSolidEdiblesPackagedUnitsSold = 0;
        public double BCSolidEdiblesTotalValueSold = 0;
        public double SolidEdiblesPackagedUnitsSold = 0;
        public double SolidEdiblesTotalValueSold = 0;

        // Non-Solid Edibles
        public int NonSolidEdiblesPackagedOpeningInventory = 0;
        public int NonSolidEdiblesPackagedAdditionsReceivedDomestic = 0;
        public int NonSolidEdiblesPackagedAdditionsReceivedReturned = 0;
        public int NonSolidEdiblesPackagedAdditionsOther = 0;
        public int NonSolidEdiblesPackagedReductionsShippedDomestic = 0;
        public int NonSolidEdiblesPackagedReductionsShippedReturned = 0;
        public int NonSolidEdiblesPackagedReductionsDestroyed = 0;
        public int NonSolidEdiblesPackagedReductionsLostStolen = 0;
        public int NonSolidEdiblesPackagedReductionsOther = 0;
        public int NonSolidEdiblesPackagedClosingInventory = 0;
        public double NonSolidEdiblesPackagedClosingInventoryValue = 0;
        public double NonSolidEdiblesPackagedClosingInventoryWeight = 0;
        public double BCNonSolidEdiblesPackagedUnitsSold = 0;
        public double BCNonSolidEdiblesTotalValueSold = 0;
        public double NonSolidEdiblesPackagedUnitsSold = 0;
        public double NonSolidEdiblesTotalValueSold = 0;

        // Inhaled Extracts
        public int InhaledExtractsPackagedOpeningInventory = 0;
        public int InhaledExtractsPackagedAdditionsReceivedDomestic = 0;
        public int InhaledExtractsPackagedAdditionsReceivedReturned = 0;
        public int InhaledExtractsPackagedAdditionsOther = 0;
        public int InhaledExtractsPackagedReductionsShippedDomestic = 0;
        public int InhaledExtractsPackagedReductionsShippedReturned = 0;
        public int InhaledExtractsPackagedReductionsDestroyed = 0;
        public int InhaledExtractsPackagedReductionsLostStolen = 0;
        public int InhaledExtractsPackagedReductionsOther = 0;
        public int InhaledExtractsPackagedClosingInventory = 0;
        public double InhaledExtractsPackagedClosingInventoryValue = 0;
        public double InhaledExtractsPackagedClosingInventoryWeight = 0;
        public double BCInhaledExtractsPackagedUnitsSold = 0;
        public double BCInhaledExtractsTotalValueSold = 0;
        public double InhaledExtractsPackagedUnitsSold = 0;
        public double InhaledExtractsTotalValueSold = 0;

        // Ingested Extracts
        public int IngestedExtractsPackagedOpeningInventory = 0;
        public int IngestedExtractsPackagedAdditionsReceivedDomestic = 0;
        public int IngestedExtractsPackagedAdditionsReceivedReturned = 0;
        public int IngestedExtractsPackagedAdditionsOther = 0;
        public int IngestedExtractsPackagedReductionsShippedDomestic = 0;
        public int IngestedExtractsPackagedReductionsShippedReturned = 0;
        public int IngestedExtractsPackagedReductionsDestroyed = 0;
        public int IngestedExtractsPackagedReductionsLostStolen = 0;
        public int IngestedExtractsPackagedReductionsOther = 0;
        public int IngestedExtractsPackagedClosingInventory = 0;
        public double IngestedExtractsPackagedClosingInventoryValue = 0;
        public double IngestedExtractsPackagedClosingInventoryWeight = 0;
        public double BCIngestedExtractsPackagedUnitsSold = 0;
        public double BCIngestedExtractsTotalValueSold = 0;
        public double IngestedExtractsPackagedUnitsSold = 0;
        public double IngestedExtractsTotalValueSold = 0;

        // OtherExtracts
        public int OtherExtractsPackagedOpeningInventory = 0;
        public int OtherExtractsPackagedAdditionsReceivedDomestic = 0;
        public int OtherExtractsPackagedAdditionsReceivedReturned = 0;
        public int OtherExtractsPackagedAdditionsOther = 0;
        public int OtherExtractsPackagedReductionsShippedDomestic = 0;
        public int OtherExtractsPackagedReductionsShippedReturned = 0;
        public int OtherExtractsPackagedReductionsDestroyed = 0;
        public int OtherExtractsPackagedReductionsLostStolen = 0;
        public int OtherExtractsPackagedReductionsOther = 0;
        public int OtherExtractsPackagedClosingInventory = 0;
        public double OtherExtractsPackagedClosingInventoryValue = 0;
        public double OtherExtractsPackagedClosingInventoryWeight = 0;
        public double BCOtherExtractsPackagedUnitsSold = 0;
        public double BCOtherExtractsTotalValueSold = 0;
        public double OtherExtractsPackagedUnitsSold = 0;
        public double OtherExtractsTotalValueSold = 0;

        // Topicals
        public int TopicalsPackagedOpeningInventory = 0;
        public int TopicalsPackagedAdditionsReceivedDomestic = 0;
        public int TopicalsPackagedAdditionsReceivedReturned = 0;
        public int TopicalsPackagedAdditionsOther = 0;
        public int TopicalsPackagedReductionsShippedDomestic = 0;
        public int TopicalsPackagedReductionsShippedReturned = 0;
        public int TopicalsPackagedReductionsDestroyed = 0;
        public int TopicalsPackagedReductionsLostStolen = 0;
        public int TopicalsPackagedReductionsOther = 0;
        public int TopicalsPackagedClosingInventory = 0;
        public double TopicalsPackagedClosingInventoryValue = 0;
        public double TopicalsPackagedClosingInventoryWeight = 0;
        public double BCTopicalsPackagedUnitsSold = 0;
        public double BCTopicalsTotalValueSold = 0;
        public double TopicalsPackagedUnitsSold = 0;
        public double TopicalsTotalValueSold = 0;

        // Other
        public int OtherPackagedOpeningInventory = 0;
        public int OtherPackagedAdditionsReceivedDomestic = 0;
        public int OtherPackagedAdditionsReceivedReturned = 0;
        public int OtherPackagedAdditionsOther = 0;
        public int OtherPackagedReductionsShippedDomestic = 0;
        public int OtherPackagedReductionsShippedReturned = 0;
        public int OtherPackagedReductionsDestroyed = 0;
        public int OtherPackagedReductionsLostStolen = 0;
        public int OtherPackagedReductionsOther = 0;
        public int OtherPackagedClosingInventory = 0;
        public double OtherPackagedClosingInventoryValue = 0;
        public double OtherPackagedClosingInventoryWeight = 0;
        public double BCOtherPackagedUnitsSold = 0;
        public double BCOtherTotalValueSold = 0;
        public double OtherPackagedUnitsSold = 0;
        public double OtherTotalValueSold = 0;

        public void PopulateProduct(adoxio_cannabisinventoryreport inventoryReport, string productName)
        {
            switch (productName)
            {
                case "Seeds":
                    SeedsPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    SeedsPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    SeedsPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    SeedsPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    SeedsPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    SeedsPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    SeedsPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    SeedsPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    SeedsPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    SeedsPackagedClosingInventoryTotal = inventoryReport.adoxio_ClosingInventory ?? 0;
                    SeedsPackagedClosingInventoryTotalValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    SeedsPackagedClosingTotalSeeds = inventoryReport.adoxio_TotalNumberSeeds ?? 0;
                    BCSeedsPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCSeedsTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    SeedsPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    SeedsTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
                case "Vegetative Cannabis":
                    VegetativeCannabisPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    VegetativeCannabisPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    VegetativeCannabisPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    VegetativeCannabisPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    VegetativeCannabisPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    VegetativeCannabisPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    VegetativeCannabisPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    VegetativeCannabisPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    VegetativeCannabisPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    VegetativeCannabisPackagedClosingInventoryTotal = inventoryReport.adoxio_ClosingInventory ?? 0;
                    VegetativeCannabisPackagedClosingInventoryTotalValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    BCVegetativeCannabisPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCVegetativeCannabisTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    VegetativeCannabisPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    VegetativeCannabisTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
                case "Fresh Cannabis":
                    FreshCannabisPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    FreshCannabisPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    FreshCannabisPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    FreshCannabisPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    FreshCannabisPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    FreshCannabisPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    FreshCannabisPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    FreshCannabisPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    FreshCannabisPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    FreshCannabisPackagedClosingInventory = inventoryReport.adoxio_ClosingInventory ?? 0;
                    FreshCannabisPackagedClosingInventoryValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    FreshCannabisPackagedClosingInventoryWeight = inventoryReport.adoxio_WeightofClosingInventory != null ? (double)inventoryReport.adoxio_WeightofClosingInventory.Value : 0;
                    BCFreshCannabisPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCFreshCannabisTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    FreshCannabisPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    FreshCannabisTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
                case "Dried Cannabis":
                    DriedCannabisPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    DriedCannabisPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    DriedCannabisPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    DriedCannabisPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    DriedCannabisPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    DriedCannabisPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    DriedCannabisPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    DriedCannabisPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    DriedCannabisPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    DriedCannabisPackagedClosingInventory = inventoryReport.adoxio_ClosingInventory ?? 0;
                    DriedCannabisPackagedClosingInventoryValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    DriedCannabisPackagedClosingInventoryWeight = inventoryReport.adoxio_WeightofClosingInventory != null ? (double)inventoryReport.adoxio_WeightofClosingInventory.Value : 0;
                    BCDriedCannabisPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCDriedCannabisTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    DriedCannabisPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    DriedCannabisTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
                case "Edibles - Solids":
                    SolidEdiblesPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    SolidEdiblesPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    SolidEdiblesPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    SolidEdiblesPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    SolidEdiblesPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    SolidEdiblesPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    SolidEdiblesPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    SolidEdiblesPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    SolidEdiblesPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    SolidEdiblesPackagedClosingInventory = inventoryReport.adoxio_ClosingInventory ?? 0;
                    SolidEdiblesPackagedClosingInventoryValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    SolidEdiblesPackagedClosingInventoryWeight = inventoryReport.adoxio_WeightofClosingInventory != null ? (double)inventoryReport.adoxio_WeightofClosingInventory.Value : 0;
                    BCSolidEdiblesPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCSolidEdiblesTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    SolidEdiblesPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    SolidEdiblesTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
                case "Edibles - Non-Solids":
                    NonSolidEdiblesPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    NonSolidEdiblesPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    NonSolidEdiblesPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    NonSolidEdiblesPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    NonSolidEdiblesPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    NonSolidEdiblesPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    NonSolidEdiblesPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    NonSolidEdiblesPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    NonSolidEdiblesPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    NonSolidEdiblesPackagedClosingInventory = inventoryReport.adoxio_ClosingInventory ?? 0;
                    NonSolidEdiblesPackagedClosingInventoryValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    NonSolidEdiblesPackagedClosingInventoryWeight = inventoryReport.adoxio_WeightofClosingInventory != null ? (double)inventoryReport.adoxio_WeightofClosingInventory.Value : 0;
                    BCNonSolidEdiblesPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCNonSolidEdiblesTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    NonSolidEdiblesPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    NonSolidEdiblesTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
                case "Extracts - Inhaled":
                    InhaledExtractsPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    InhaledExtractsPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    InhaledExtractsPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    InhaledExtractsPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    InhaledExtractsPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    InhaledExtractsPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    InhaledExtractsPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    InhaledExtractsPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    InhaledExtractsPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    InhaledExtractsPackagedClosingInventory = inventoryReport.adoxio_ClosingInventory ?? 0;
                    InhaledExtractsPackagedClosingInventoryValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    InhaledExtractsPackagedClosingInventoryWeight = inventoryReport.adoxio_WeightofClosingInventory != null ? (double)inventoryReport.adoxio_WeightofClosingInventory.Value : 0;
                    BCInhaledExtractsPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCInhaledExtractsTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    InhaledExtractsPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    InhaledExtractsTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
                case "Extracts - Ingested":
                    IngestedExtractsPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    IngestedExtractsPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    IngestedExtractsPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    IngestedExtractsPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    IngestedExtractsPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    IngestedExtractsPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    IngestedExtractsPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    IngestedExtractsPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    IngestedExtractsPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    IngestedExtractsPackagedClosingInventory = inventoryReport.adoxio_ClosingInventory ?? 0;
                    IngestedExtractsPackagedClosingInventoryValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    IngestedExtractsPackagedClosingInventoryWeight = inventoryReport.adoxio_WeightofClosingInventory != null ? (double)inventoryReport.adoxio_WeightofClosingInventory.Value : 0;
                    BCIngestedExtractsPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCIngestedExtractsTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    IngestedExtractsPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    IngestedExtractsTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
                case "Extracts - Other":
                    OtherExtractsPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    OtherExtractsPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    OtherExtractsPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    OtherExtractsPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    OtherExtractsPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    OtherExtractsPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    OtherExtractsPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    OtherExtractsPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    OtherExtractsPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    OtherExtractsPackagedClosingInventory = inventoryReport.adoxio_ClosingInventory ?? 0;
                    OtherExtractsPackagedClosingInventoryValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    OtherExtractsPackagedClosingInventoryWeight = inventoryReport.adoxio_WeightofClosingInventory != null ? (double)inventoryReport.adoxio_WeightofClosingInventory.Value : 0;
                    BCOtherExtractsPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCOtherExtractsTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    OtherExtractsPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    OtherExtractsTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
                case "Topicals":
                    TopicalsPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    TopicalsPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    TopicalsPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    TopicalsPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    TopicalsPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    TopicalsPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    TopicalsPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    TopicalsPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    TopicalsPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    TopicalsPackagedClosingInventory = inventoryReport.adoxio_ClosingInventory ?? 0;
                    TopicalsPackagedClosingInventoryValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    TopicalsPackagedClosingInventoryWeight = inventoryReport.adoxio_WeightofClosingInventory != null ? (double)inventoryReport.adoxio_WeightofClosingInventory.Value : 0;
                    BCTopicalsPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCTopicalsTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    TopicalsPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    TopicalsTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
                case "Other":
                    OtherPackagedOpeningInventory = inventoryReport.adoxio_OpeningInventory ?? 0;
                    OtherPackagedAdditionsReceivedDomestic = inventoryReport.adoxio_QtyReceivedDomestic ?? 0;
                    OtherPackagedAdditionsReceivedReturned = inventoryReport.adoxio_QtyReceivedReturns ?? 0;
                    OtherPackagedAdditionsOther = inventoryReport.adoxio_QtyReceivedOther ?? 0;
                    OtherPackagedReductionsShippedDomestic = inventoryReport.adoxio_QtyShippedDomestic ?? 0;
                    OtherPackagedReductionsShippedReturned = inventoryReport.adoxio_QtyShippedReturned ?? 0;
                    OtherPackagedReductionsDestroyed = inventoryReport.adoxio_QtyDestroyed ?? 0;
                    OtherPackagedReductionsLostStolen = inventoryReport.adoxio_QtyLostStolen ?? 0;
                    OtherPackagedReductionsOther = inventoryReport.adoxio_OtherReductions ?? 0;
                    OtherPackagedClosingInventory = inventoryReport.adoxio_ClosingInventory ?? 0;
                    OtherPackagedClosingInventoryValue = inventoryReport.adoxio_ValueofClosingInventory != null ? (double)inventoryReport.adoxio_ValueofClosingInventory.Value : 0;
                    OtherPackagedClosingInventoryWeight = inventoryReport.adoxio_WeightofClosingInventory != null ? (double)inventoryReport.adoxio_WeightofClosingInventory.Value : 0;
                    BCOtherPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    BCOtherTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    OtherPackagedUnitsSold = inventoryReport.adoxio_PackagedUnitsNumber != null ? (double)inventoryReport.adoxio_PackagedUnitsNumber.Value : 0;
                    OtherTotalValueSold = inventoryReport.adoxio_TotalValue != null ? (double)inventoryReport.adoxio_TotalValue.Value : 0;
                    break;
            }
        }
    }
}
