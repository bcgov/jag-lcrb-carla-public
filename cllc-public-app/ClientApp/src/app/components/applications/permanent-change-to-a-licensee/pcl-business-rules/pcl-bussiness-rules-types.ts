export interface PCLFormControlDefinitionOption {
  /**
   * The name of the type of change, used to display the option in the UI.
   */
  name: string;
  /**
   * The name, if the businessType of the account is classified as other.
   *
   * See `isOtherBusinessType` in `account.model.ts`
   */
  otherName?: string;
  /**
   * The form control name for the option, used to bind the option to a form control in the Angular form.
   *
   */
  formControlName: PCLFormControlName;
  /**
   * The fee for this type of change to a cannabis application, if applicable.
   */
  CannabisFee: number;
  /**
   * The fee for this type of change to a liquor application, if applicable.
   */
  LiquorFee: number;
  /**
   * The header text for the help text section, providing context for the type of change.
   */
  helpTextHeader: string;
  /**
   * The help text for the type of change, providing additional information about the type of change.
   */
  helpText: string[];
  /**
   * The link to additional help text or documentation for the type of change, if applicable.
   */
  helpTextLink?: string;
}

/**
 * Enum for all License Type Names.
 *
 * TODO: tiedhouse - This was pulled from Dev. Is it the same in Prod?
 *
 * @export
 * @enum {number}
 */
export enum LicenceTypeNames {
  // Liquor
  'Agent' = 'Agent',
  'Catering' = 'Catering',
  'FoodPrimary' = 'Food Primary',
  'LicenseeRetailStore' = 'Licensee Retail Store',
  'LiquorPrimary' = 'Liquor Primary',
  'LiquorPrimaryClub' = 'Liquor Primary Club',
  'Manufacturer' = 'Manufacturer',
  'RuralLicenseeRetailStore' = 'Rural Licensee Retail Store',
  'UBrewAndUVin' = 'UBrew and UVin',
  'WineStore' = 'Wine Store',
  // Special Event Permit
  'SpecialEventPermit' = 'Special Event Permit',
  // Cannabis
  'CannabisRetailStore' = 'Cannabis Retail Store',
  'Marketing' = 'Marketing',
  'ProducerRetailStore' = 'Producer Retail Store',
  // Other
  'FarmToGate' = 'Farm to Gate',
  'Licensee' = 'Licensee',
  'Liquor-TemporaryRelocation' = 'Liquor - Temporary Relocation',
  'S119CRSAuthorization' = 'S119 CRS Authorization',
  'S119PRSAuthorization' = 'S119 PRS Authorization'
}

/**
 * Enum for Liquor Licence Type Names.
 *
 * @export
 * @enum {number}
 */
export enum LiquorLicenceTypeNames {
  'Agent' = 'Agent',
  'Catering' = 'Catering',
  'FoodPrimary' = 'Food Primary',
  'LicenseeRetailStore' = 'Licensee Retail Store',
  'LiquorPrimary' = 'Liquor Primary',
  'LiquorPrimaryClub' = 'Liquor Primary Club',
  'Manufacturer' = 'Manufacturer',
  'RuralLicenseeRetailStore' = 'Rural Licensee Retail Store',
  'UBrewAndUVin' = 'UBrew and UVin',
  'WineStore' = 'Wine Store'
}

/**
 * Enum for Cannabis Licence Type Names.
 *
 * @export
 * @enum {number}
 */
export enum CannabisLicenceTypeNames {
  'CannabisRetailStore' = 'Cannabis Retail Store',
  'Marketing' = 'Marketing',
  'ProducerRetailStore' = 'Producer Retail Store'
}

export enum PCLFormControlName {
  csAdditionalReceiverOrExecutor = 'csAdditionalReceiverOrExecutor',
  csExternalTransferOfShares = 'csExternalTransferOfShares',
  csInternalTransferOfShares = 'csInternalTransferOfShares',
  csChangeOfDirectorsOrOfficers = 'csChangeOfDirectorsOrOfficers',
  csNameChangeLicenseeCorporation = 'csNameChangeLicenseeCorporation',
  csNameChangeLicenseePartnership = 'csNameChangeLicenseePartnership',
  csNameChangeLicenseeSociety = 'csNameChangeLicenseeSociety',
  csNameChangeLicenseePerson = 'csNameChangeLicenseePerson',
  csTiedHouseDeclaration = 'csTiedHouseDeclaration'
}

export enum AccountType {
  // Default
  Default = 'Default',
  // Private Corporation
  PrivateCorporation = 'PrivateCorporation',
  // Public Corporation
  PublicCorporation = 'PublicCorporation',
  // Partnership
  Partnership = 'Partnership',
  GeneralPartnership = 'GeneralPartnership',
  LimitedLiabilityPartnership = 'LimitedLiabilityPartnership',
  // Society
  Society = 'Society',
  IndigenousNation = 'IndigenousNation',
  LocalGovernment = 'LocalGovernment',
  Coop = 'Coop',
  MilitaryMess = 'MilitaryMess',
  University = 'University',
  SoleProprietorship = 'SoleProprietorship'
}

export enum PCLMatrixConditionalGroup {
  Default = 'Default', // If no sections are selected
  ReceiverOrExecutor = 'ReceiverOrExecutor', // If Add/Remove Receiver Executor selected
  TSE = 'TSE', // If Transfer of Shares External is Selected
  TSI = 'TSI', // If Transfer of Shares Internal is Selected
  CoD = 'CoD', // If Change of Directors or Officers is Selected
  CoDAndTSE = 'CoDAndTSE', // If Change of Directors or Officers AND Transfer of Shares External is Selected
  CoDAndTSI = 'CoDAndTSI', // If Change of Directors or Officers AND Transfer of Shares Internal is Selected
  LENameChangeCorporation = 'LENameChangeCorporation', // If LE Name Change Corporation is Selected
  LENameChangePartnership = 'LENameChangePartnership', // If LE Name Change Partnership is Selected
  LENameChangeSociety = 'LENameChangeSociety', // If LE Name Change Society is Selected
  LENameChangeIndividual = 'LENameChangeIndividual', // If LE Name Change Individual is Selected
  TiedHouseChange = 'TiedHouseChange' // If Report Tied House Declaration is Selected
}

/**
 * Represents all of the unique groups of licences, each with a unique set of conditional logic.
 */
export enum PCLMatrixLicenceGroup {
  /**
   * Default fallback group if no other group matches
   */
  Default = 'Default',
  /**
   * Rule: Liquor (Any of FP, LP, LPC, Catering, LRS, WS, RLRS, Ubrew/Uvin)
   */
  Liquor1 = 'Liquor1',
  /**
   * Rule: Liquor (Any of MFR, Agent) AND Liquor (No Other)
   */
  Liquor2 = 'Liquor2',
  /**
   * Rule: Liquor (Any of MFR, Agent) AND Liquor (At least one other (excluding MFR, Agent))
   */
  Liquor3 = 'Liquor3',
  /**
   * Rule: Cannabis (Any of CRS, PRS, Marketing)
   */
  Cannabis1 = 'Cannabis1',
  /**
   * Rule: Cannabis (Any of CRS, PRS, Marketing) AND Liquor (Any of MFR, Agent) AND Liquor (No Other)
   */
  Cannabis2 = 'Cannabis2',
  /**
   * Rule: Cannabis (Any of CRS, PRS, Marketing) AND Liquor (Any of FP, LP, LPC, Catering, LRS, WS, RLRS, Ubrew/Uvin)
   */
  Cannabis3 = 'Cannabis3',
  /**
   * Rule: Cannabis (Any of CRS, PRS, Marketing) AND Liquor (Any of MFR, Agent) AND Liquor (At least one other (excluding MFR, Agent))
   */
  Cannabis4 = 'Cannabis4'
}
