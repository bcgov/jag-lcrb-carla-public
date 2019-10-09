using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class FederalTrackingMonthlyExport
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

        public void PopulateProduct(MicrosoftDynamicsCRMadoxioCannabisinventoryreport inventoryReport, MicrosoftDynamicsCRMadoxioCannabisproductadmin product)
        {
            switch (product.AdoxioName)
            {
                case "Seeds":
                    SeedsPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    SeedsPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    SeedsPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    SeedsPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    SeedsPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    SeedsPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    SeedsPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    SeedsPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    SeedsPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    SeedsPackagedClosingInventoryTotal = inventoryReport.AdoxioClosinginventory ?? 0;
                    SeedsPackagedClosingInventoryTotalValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    // TODO needs "Total number of seeds (#)" field
                    // SeedsPackagedClosingTotalSeeds = inventoryReport.AdoxioWeightofclosinginventory;
                    BCSeedsPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCSeedsTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    SeedsPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    SeedsTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
                case "Vegetative Cannabis":
                    VegetativeCannabisPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    VegetativeCannabisPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    VegetativeCannabisPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    VegetativeCannabisPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    VegetativeCannabisPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    VegetativeCannabisPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    VegetativeCannabisPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    VegetativeCannabisPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    VegetativeCannabisPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    VegetativeCannabisPackagedClosingInventoryTotal = inventoryReport.AdoxioClosinginventory ?? 0;
                    VegetativeCannabisPackagedClosingInventoryTotalValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    BCVegetativeCannabisPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCVegetativeCannabisTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    VegetativeCannabisPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    VegetativeCannabisTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
                case "Fresh Cannabis":
                    FreshCannabisPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    FreshCannabisPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    FreshCannabisPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    FreshCannabisPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    FreshCannabisPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    FreshCannabisPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    FreshCannabisPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    FreshCannabisPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    FreshCannabisPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    FreshCannabisPackagedClosingInventory = inventoryReport.AdoxioClosinginventory ?? 0;
                    FreshCannabisPackagedClosingInventoryValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    FreshCannabisPackagedClosingInventoryWeight = inventoryReport.AdoxioWeightofclosinginventory != null ? (double)inventoryReport.AdoxioWeightofclosinginventory : 0;
                    BCFreshCannabisPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCFreshCannabisTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    FreshCannabisPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    FreshCannabisTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
                case "Dried Cannabis":
                    DriedCannabisPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    DriedCannabisPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    DriedCannabisPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    DriedCannabisPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    DriedCannabisPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    DriedCannabisPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    DriedCannabisPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    DriedCannabisPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    DriedCannabisPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    DriedCannabisPackagedClosingInventory = inventoryReport.AdoxioClosinginventory ?? 0;
                    DriedCannabisPackagedClosingInventoryValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    DriedCannabisPackagedClosingInventoryWeight = inventoryReport.AdoxioWeightofclosinginventory != null ? (double)inventoryReport.AdoxioWeightofclosinginventory : 0;
                    BCDriedCannabisPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCDriedCannabisTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    DriedCannabisPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    DriedCannabisTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
                case "Edibles - Solids":
                    SolidEdiblesPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    SolidEdiblesPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    SolidEdiblesPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    SolidEdiblesPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    SolidEdiblesPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    SolidEdiblesPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    SolidEdiblesPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    SolidEdiblesPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    SolidEdiblesPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    SolidEdiblesPackagedClosingInventory = inventoryReport.AdoxioClosinginventory ?? 0;
                    SolidEdiblesPackagedClosingInventoryValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    SolidEdiblesPackagedClosingInventoryWeight = inventoryReport.AdoxioWeightofclosinginventory != null ? (double)inventoryReport.AdoxioWeightofclosinginventory : 0;
                    BCSolidEdiblesPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCSolidEdiblesTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    SolidEdiblesPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    SolidEdiblesTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
                case "Edibles - Non-Solids":
                    NonSolidEdiblesPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    NonSolidEdiblesPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    NonSolidEdiblesPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    NonSolidEdiblesPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    NonSolidEdiblesPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    NonSolidEdiblesPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    NonSolidEdiblesPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    NonSolidEdiblesPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    NonSolidEdiblesPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    NonSolidEdiblesPackagedClosingInventory = inventoryReport.AdoxioClosinginventory ?? 0;
                    NonSolidEdiblesPackagedClosingInventoryValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    NonSolidEdiblesPackagedClosingInventoryWeight = inventoryReport.AdoxioWeightofclosinginventory != null ? (double)inventoryReport.AdoxioWeightofclosinginventory : 0;
                    BCNonSolidEdiblesPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCNonSolidEdiblesTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    NonSolidEdiblesPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    NonSolidEdiblesTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
                case "Extracts - Inhaled":
                    InhaledExtractsPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    InhaledExtractsPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    InhaledExtractsPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    InhaledExtractsPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    InhaledExtractsPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    InhaledExtractsPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    InhaledExtractsPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    InhaledExtractsPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    InhaledExtractsPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    InhaledExtractsPackagedClosingInventory = inventoryReport.AdoxioClosinginventory ?? 0;
                    InhaledExtractsPackagedClosingInventoryValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    InhaledExtractsPackagedClosingInventoryWeight = inventoryReport.AdoxioWeightofclosinginventory != null ? (double)inventoryReport.AdoxioWeightofclosinginventory : 0;
                    BCInhaledExtractsPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCInhaledExtractsTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    InhaledExtractsPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    InhaledExtractsTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
                case "Extracts - Ingested":
                    IngestedExtractsPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    IngestedExtractsPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    IngestedExtractsPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    IngestedExtractsPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    IngestedExtractsPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    IngestedExtractsPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    IngestedExtractsPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    IngestedExtractsPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    IngestedExtractsPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    IngestedExtractsPackagedClosingInventory = inventoryReport.AdoxioClosinginventory ?? 0;
                    IngestedExtractsPackagedClosingInventoryValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    IngestedExtractsPackagedClosingInventoryWeight = inventoryReport.AdoxioWeightofclosinginventory != null ? (double)inventoryReport.AdoxioWeightofclosinginventory : 0;
                    BCIngestedExtractsPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCIngestedExtractsTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    IngestedExtractsPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    IngestedExtractsTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
                case "Extracts - Other":
                    OtherExtractsPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    OtherExtractsPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    OtherExtractsPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    OtherExtractsPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    OtherExtractsPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    OtherExtractsPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    OtherExtractsPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    OtherExtractsPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    OtherExtractsPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    OtherExtractsPackagedClosingInventory = inventoryReport.AdoxioClosinginventory ?? 0;
                    OtherExtractsPackagedClosingInventoryValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    OtherExtractsPackagedClosingInventoryWeight = inventoryReport.AdoxioWeightofclosinginventory != null ? (double)inventoryReport.AdoxioWeightofclosinginventory : 0;
                    BCOtherExtractsPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCOtherExtractsTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    OtherExtractsPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    OtherExtractsTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
                case "Topicals":
                    TopicalsPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    TopicalsPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    TopicalsPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    TopicalsPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    TopicalsPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    TopicalsPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    TopicalsPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    TopicalsPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    TopicalsPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    TopicalsPackagedClosingInventory = inventoryReport.AdoxioClosinginventory ?? 0;
                    TopicalsPackagedClosingInventoryValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    TopicalsPackagedClosingInventoryWeight = inventoryReport.AdoxioWeightofclosinginventory != null ? (double)inventoryReport.AdoxioWeightofclosinginventory : 0;
                    BCTopicalsPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCTopicalsTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    TopicalsPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    TopicalsTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
                case "Other":
                    OtherPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                    OtherPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                    OtherPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                    OtherPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                    OtherPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                    OtherPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                    OtherPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                    OtherPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                    OtherPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                    OtherPackagedClosingInventory = inventoryReport.AdoxioClosinginventory ?? 0;
                    OtherPackagedClosingInventoryValue = inventoryReport.AdoxioValueofclosinginventory != null ? (double)inventoryReport.AdoxioValueofclosinginventory : 0;
                    OtherPackagedClosingInventoryWeight = inventoryReport.AdoxioWeightofclosinginventory != null ? (double)inventoryReport.AdoxioWeightofclosinginventory : 0;
                    BCOtherPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    BCOtherTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    OtherPackagedUnitsSold = inventoryReport.AdoxioPackagedunitsnumber != null ? (double)inventoryReport.AdoxioPackagedunitsnumber : 0;
                    OtherTotalValueSold = inventoryReport.AdoxioTotalvalue != null ? (double)inventoryReport.AdoxioTotalvalue : 0;
                    break;
            }
        }
    }
}
