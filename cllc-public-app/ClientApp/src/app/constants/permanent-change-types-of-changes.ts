export interface PermanentChangeTypesOfChangesOption {
  /**
   * The name of the type of change, used to display the option in the UI.
   *
   * @type {string}
   * @memberof PermanentChangeTypesOfChangesOption
   */
  name: string;
  /**
   * The name, if the businessType of the account is classified as other.
   *
   * See `isOtherBusinessType` in `account.model.ts`
   *
   * @type {string}
   * @memberof PermanentChangeTypesOfChangesOption
   */
  otherName?: string;
  /**
   * The form control name for the option, used to bind the option to a form control in the Angular form.
   *
   * @type {string}
   * @memberof PermanentChangeTypesOfChangesOption
   */
  formControlName: string;
  /**
   * The list of business types that this type of change is available to.
   *
   * Uses `businessType` from `account.model.ts`.
   *
   * @type {string[]}
   * @memberof PermanentChangeTypesOfChangesOption
   */
  availableTo: string[];
  /**
   * The fee for this type of change to a cannabis application, if applicable.
   *
   * @type {number}
   * @memberof PermanentChangeTypesOfChangesOption
   */
  CannabisFee: number;
  /**
   * The fee for this type of change to a liquor application, if applicable.
   *
   * @type {number}
   * @memberof PermanentChangeTypesOfChangesOption
   */
  LiquorFee: number;
  /**
   * The header text for the help text section, providing context for the type of change.
   *
   * @type {string}
   * @memberof PermanentChangeTypesOfChangesOption
   */
  helpTextHeader: string;
  /**
   * The help text for the type of change, providing additional information about the type of change.
   *
   * @type {string[]}
   * @memberof PermanentChangeTypesOfChangesOption
   */
  helpText: string[];
  /**
   * The link to additional help text or documentation for the type of change, if applicable.
   *
   * @type {string}
   * @memberof PermanentChangeTypesOfChangesOption
   */
  helpTextLink?: string;
}

/**
 * The list of options for the types of changes in a permanent change application.
 */
export const permanentChangeTypesOfChanges: PermanentChangeTypesOfChangesOption[] = [
  {
    name: 'Internal Transfer of Shares',
    formControlName: 'csInternalTransferOfShares',
    availableTo: ['PrivateCorporation', 'LimitedLiabilityPartnership'],
    CannabisFee: 110,
    LiquorFee: 110,
    helpTextHeader: 'Use this option to report:',
    helpText: [
      'When shares or partnership units are redistributed between existing shareholders/partners  but no new shareholders/partners are added to the business structure (if new shareholders are added, see external transfer of shares)',
      'Removal of shareholders/unit holders',
      'Amalgamations that do not add new shareholders or legal entities to the licensee  corporation',
      'Holding companies within the licensee corporation and/or third party operators should also complete this section when an internal share transfer or an amalgamation occurs'
    ]
    // helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'External Transfer of Shares',
    formControlName: 'csExternalTransferOfShares',
    availableTo: ['PrivateCorporation', 'LimitedLiabilityPartnership'],
    CannabisFee: 330,
    LiquorFee: 330,
    helpTextHeader: 'Use this option to report:',
    helpText: [
      'When new shareholders (companies or individuals) have been added to the licensee corporation or holding companies as a result of a transfer of existing shares or the issuance of new shares',
      'Amalgamations that add new shareholders or legal entities to the licensee corporation',
      'Third party operators should also complete this section when an external transfer occurs'
    ]
    // helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Change of Directors or Officers',
    formControlName: 'csChangeOfDirectorsOrOfficers',
    availableTo: [
      'PrivateCorporation',
      'PublicCorporation',
      'Society',
      'Coop',
      'MilitaryMess',
      'LocalGovernment',
      'University'
    ],
    CannabisFee: 500,
    LiquorFee: 220,
    helpTextHeader: 'Use this option to report:',
    helpText: [
      'For liquor licensees - when there are changes in directors or officers, as defined by the <a href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/02057_00_multi#section1" target="_blank">BC Corporations Act</a> or the <a href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/15018_01#section1" target="_blank">BC Societies Act</a>, of a public corporation or society that holds a licence, or of a public corporation or society within the licensee legal entity',
      'For cannabis licensees – when there are changes in directors or officers of a private or public  corporation or society that holds a licence, or of a public or private corporation or society within the licensee legal entity'
    ]
    // helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Name Change, Licensee -- Corporation',
    otherName: 'Name Change, Licensee -- Organization',
    formControlName: 'csNameChangeLicenseeCorporation',
    availableTo: [
      'PrivateCorporation',
      'PublicCorporation',
      'SoleProprietorship',
      'Coop',
      'MilitaryMess',
      'University'
    ],
    CannabisFee: 220,
    LiquorFee: 220,
    helpTextHeader: 'Use this option to report:',
    helpText: [
      'When a corporation with an interest in a licence has legally changed its name, but existing corporate shareholders, directors and officers, and certificate number on the certificate of incorporation have not changed'
    ]

    // helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Name Change, Licensee -- Partnership',
    formControlName: 'csNameChangeLicenseePartnership',
    availableTo: ['GeneralPartnership', 'Partnership', 'LimitedLiabilityPartnership'],
    CannabisFee: 220,
    LiquorFee: 220,
    helpTextHeader: 'Use this option to report:',
    helpText: ['When a person holding an interest in a licence has legally changed their name']
    // helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Name Change, Licensee -- Society',
    formControlName: 'csNameChangeLicenseeSociety',
    availableTo: ['Society'],
    CannabisFee: 220,
    LiquorFee: 220,
    helpTextHeader: 'Use this option to report:',
    helpText: [
      'When the legal name of a society is changed, but the society structure, membership and certification number on the certificate of incorporation does not change'
    ]
    // helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Name Change, Person',
    formControlName: 'csNameChangeLicenseePerson',
    availableTo: [
      'PrivateCorporation',
      'PublicCorporation',
      'GeneralPartnership',
      'Partnership',
      'LimitedLiabilityPartnership',
      'IndigenousNation',
      'LocalGovernment',
      'Society',
      'Coop',
      'MilitaryMess'
    ],
    CannabisFee: 220,
    LiquorFee: 220,
    helpTextHeader: 'Use this option to report:',
    helpText: ['when a person holding an interest in a licence has legally changed their name']
    // helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Addition of Receiver or Executor',
    formControlName: 'csAdditionalReceiverOrExecutor',
    availableTo: [
      'PrivateCorporation',
      'PublicCorporation',
      'GeneralPartnership',
      'Partnership',
      'LimitedLiabilityPartnership',
      'Society',
      'MilitaryMess'
    ],
    CannabisFee: 220,
    LiquorFee: 220,
    helpTextHeader: 'Use this option to report:',
    helpText: ['Upon the death, bankruptcy or receivership of a licensee']
    // helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Tied House Declaration',
    formControlName: 'csTiedHouseDeclaration',
    availableTo: [
      'PrivateCorporation',
      'PublicCorporation',
      'GeneralPartnership',
      'Partnership',
      'LimitedLiabilityPartnership',
      'Society',
      'MilitaryMess'
    ],
    CannabisFee: 220,
    LiquorFee: 220,
    helpTextHeader: 'Use this option to report:',
    helpText: ['Upon the death, bankruptcy or receivership of a licensee']
    //helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  }
];
