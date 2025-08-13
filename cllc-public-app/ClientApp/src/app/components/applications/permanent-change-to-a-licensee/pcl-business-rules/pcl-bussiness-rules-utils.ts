import { PCLFormControlDefinitions } from '@components/applications/permanent-change-to-a-licensee/pcl-business-rules/pcl-bussiness-rules-content';
import {
  AccountType,
  LiquorLicenceTypeNames,
  PCLFormControlDefinitionOption,
  PCLFormControlName,
  PCLMatrixConditionalGroup,
  PCLMatrixLicenceGroup
} from '@components/applications/permanent-change-to-a-licensee/pcl-business-rules/pcl-bussiness-rules-types';
import { AccountSummary, LicenceTypeCategory } from '@models/account-summary.model';

/**
 * This file contains fairly complex business logic that is used to determine which sections of the
 * Permanent Change to a Licensee (PCL) application should be displayed based on the user's licences and the
 * selected PCL section(s).
 *
 * Any changes to the logic here should be made with caution.
 *
 * Original ticket: https://jira.justice.gov.bc.ca/browse/LCSD-7706
 *
 * @example
 * const account: Account = {...};
 * const accountSummary: AccountSummary = {...};
 *
 * const pclLicenceGroup = getPCLMatrixGroup(accountSummary);
 *
 * const pclConditionalGroup = getPCLMatrixConditionalGroup({
 *   selectedPCLSections: ['csAdditionalReceiverOrExecutor']
 * });
 *
 * const pclSectionBusinessRules = getPCLMatrixSectionBusinessRules({
 *   accountType: this.account.businessType; // PrivateCorporation
 *   conditionalGroup: pclConditionalGroup;
 *   licenceGroup: pclLicenceGroup;
 * });
 */

/**
 * Get the PCLMatrixLicenceGroup based on the user's licences.
 *
 * Business logic to determine which PCLMatrixLicenceGroup the user belongs to based on their licences.
 *
 * @param {AccountSummary} accountSummary
 * @return {*}  {PCLMatrixLicenceGroup}
 */
export const getPCLMatrixGroup = (accountSummary: AccountSummary): PCLMatrixLicenceGroup => {
  if (hasNoLicences(accountSummary)) {
    return PCLMatrixLicenceGroup.Default;
  }

  /*************************************************************
   * If the user only has liquor licences
   ************************************************************/

  if (hasOnlyLiquorLicences(accountSummary)) {
    if (hasSomeLiquorLicences(accountSummary, [LiquorLicenceTypeNames.Manufacturer, LiquorLicenceTypeNames.Agent])) {
      if (
        hasAtLeastOneOtherLiquorLicenceType(accountSummary, [
          LiquorLicenceTypeNames.Manufacturer,
          LiquorLicenceTypeNames.Agent
        ])
      ) {
        return PCLMatrixLicenceGroup.Liquor3;
      }

      return PCLMatrixLicenceGroup.Liquor2;
    }

    if (
      hasSomeLiquorLicences(accountSummary, [
        LiquorLicenceTypeNames.FoodPrimary,
        LiquorLicenceTypeNames.LiquorPrimary,
        LiquorLicenceTypeNames.LiquorPrimaryClub,
        LiquorLicenceTypeNames.Catering,
        LiquorLicenceTypeNames.LicenseeRetailStore,
        LiquorLicenceTypeNames.WineStore,
        LiquorLicenceTypeNames.RuralLicenseeRetailStore,
        LiquorLicenceTypeNames.UBrewAndUVin
      ])
    ) {
      return PCLMatrixLicenceGroup.Liquor1;
    }
  }

  /*************************************************************
   * If the user only has cannabis licences
   *************************************************************/

  if (hasOnlyCannabisLicences(accountSummary)) {
    return PCLMatrixLicenceGroup.Cannabis1;
  }

  /*************************************************************
   * If the user has a mixture of liquor and cannabis licences
   *************************************************************/

  if (hasSomeLiquorLicences(accountSummary, [LiquorLicenceTypeNames.Manufacturer, LiquorLicenceTypeNames.Agent])) {
    if (
      hasAtLeastOneOtherLiquorLicenceType(accountSummary, [
        LiquorLicenceTypeNames.Manufacturer,
        LiquorLicenceTypeNames.Agent
      ])
    ) {
      return PCLMatrixLicenceGroup.Cannabis4;
    }

    return PCLMatrixLicenceGroup.Cannabis2;
  }

  if (
    hasSomeLiquorLicences(accountSummary, [
      LiquorLicenceTypeNames.FoodPrimary,
      LiquorLicenceTypeNames.LiquorPrimary,
      LiquorLicenceTypeNames.LiquorPrimaryClub,
      LiquorLicenceTypeNames.Catering,
      LiquorLicenceTypeNames.LicenseeRetailStore,
      LiquorLicenceTypeNames.WineStore,
      LiquorLicenceTypeNames.RuralLicenseeRetailStore,
      LiquorLicenceTypeNames.UBrewAndUVin
    ])
  ) {
    return PCLMatrixLicenceGroup.Cannabis3;
  }
};

/**
 * Get the current PCLMatrixConditionalGroup based on the selected PCL sections.
 * Returns 'Default' if no sections are selected or if no matching group is found.
 *
 * Why? Depending on which PCL sections the user has already selected, other PCL sections may be displayed or hidden.
 *
 * @param {PCLMatrixConditionalGroupOptions} options
 * @return {*}  {(PCLMatrixConditionalGroup | null)}
 */
export const getPCLMatrixConditionalGroup = (
  options: PCLMatrixConditionalGroupOptions
): PCLMatrixConditionalGroup | null => {
  const { selectedPCLSections } = options;

  if (!selectedPCLSections?.length) {
    return PCLMatrixConditionalGroup.Default;
  }

  // PCL Matrix - Private Corporation - If Add/Remove Receiver Executor selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csAdditionalReceiverOrExecutor],
      selectedSections: selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.ReceiverOrExecutor;
  }

  // PCL Matrix - Private Corporation - If TSE is Selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csExternalTransferOfShares],
      selectedSections: selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.TSE;
  }

  // PCL Matrix - Private Corporation - If TSI is Selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csInternalTransferOfShares],
      selectedSections: selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.TSI;
  }

  // PCL Matrix - Private Corporation - If CoD is Selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csChangeOfDirectorsOrOfficers],
      selectedSections: selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.CoD;
  }

  // PCL Matrix - Private Corporation - If CoD AND TSE selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csChangeOfDirectorsOrOfficers, PCLFormControlName.csExternalTransferOfShares],
      selectedSections: selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.CoDAndTSE;
  }

  // PCL Matrix - Private Corporation - If CoD AND TSI selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csChangeOfDirectorsOrOfficers, PCLFormControlName.csInternalTransferOfShares],
      selectedSections: selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.CoDAndTSI;
  }

  // PCL Matrix - Private Corporation - If LE Name Change Corporation is Selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csNameChangeLicenseeCorporation],
      selectedSections: selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.LENameChangeCorporation;
  }

  // PCL Matrix - Partnership - If LE Name Change Partnership is Selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csNameChangeLicenseePartnership],
      selectedSections: options.selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.LENameChangePartnership;
  }

  // PCL Matrix - Society - If LE Name Change Society is Selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csNameChangeLicenseeSociety],
      selectedSections: options.selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.LENameChangeSociety;
  }

  // PCL Matrix - Private Corporation - If LE Name Change Individual is Selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csNameChangeLicenseePerson],
      selectedSections: options.selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.LENameChangeIndividual;
  }

  // PCL Matrix - Private Corporation - If Report Tied House Change is Selected
  if (
    hasAllMatchSections({
      matchSections: [PCLFormControlName.csTiedHouseDeclaration],
      selectedSections: options.selectedPCLSections
    })
  ) {
    return PCLMatrixConditionalGroup.TiedHouseChange;
  }

  return PCLMatrixConditionalGroup.Default;
};

/**
 * Get the PCL PCLFormControlDefinitionOptions based on the provided options.
 *
 * @param {PCLSectionNamesFromBitmaskOptions} options
 * @return {*}  {PCLFormControlDefinitionOption[]}
 */
export const getPCLMatrixSectionBusinessRules = (
  options: PCLSectionNamesFromBitmaskOptions
): PCLFormControlDefinitionOption[] => {
  const accountTypeBitMasks =
    PCLSectionBusinessRules[options.accountType] ?? PCLSectionBusinessRules[AccountType.Default];

  if (!accountTypeBitMasks) {
    const pclFormControlNames = getPCLSectionNamesFromBitmask(FULL_BITMASK);
    return getPCLFormControlDefinitions(pclFormControlNames);
  }

  const conditionalGroupBitMasks =
    accountTypeBitMasks[options.conditionalGroup] ?? accountTypeBitMasks[PCLMatrixConditionalGroup.Default];

  if (!conditionalGroupBitMasks) {
    const pclFormControlNames = getPCLSectionNamesFromBitmask(FULL_BITMASK);
    return getPCLFormControlDefinitions(pclFormControlNames);
  }

  const licenceGroupBitMasks =
    conditionalGroupBitMasks[options.licenceGroup] ?? conditionalGroupBitMasks[PCLMatrixLicenceGroup.Default];

  if (!licenceGroupBitMasks) {
    const pclFormControlNames = getPCLSectionNamesFromBitmask(FULL_BITMASK);
    return getPCLFormControlDefinitions(pclFormControlNames);
  }

  const bitmask = licenceGroupBitMasks();

  const pclFormControlNames = getPCLSectionNamesFromBitmask(bitmask);

  const pclFormControlDefinitions = getPCLFormControlDefinitions(pclFormControlNames);

  return pclFormControlDefinitions;
};

const FULL_BITMASK = 0b111111111;
const EMPTY_BITMASK = 0b000000000;

/**
 * The unique bitmask for each PCL section. These are used in combination with the business logic to determine which
 * sections should be displayed based on the user's licences and selected PCL section.
 *
 * Important: These bitmask values are tightly coupled to the values in both `PCLMatrixGroup_Section_Bitmasks`
 * and `PCLMatrixGroup_ConditionalSection_Bitmasks`. If you change these values, you must also update those
 * mappings accordingly.
 */
const PCLSectionBitmasks: Record<PCLFormControlName, number> = {
  csAdditionalReceiverOrExecutor: 0b100000000,
  csExternalTransferOfShares: 0b010000000,
  csInternalTransferOfShares: 0b001000000,
  csChangeOfDirectorsOrOfficers: 0b000100000,
  csNameChangeLicenseeCorporation: 0b000010000,
  csNameChangeLicenseePartnership: 0b000001000,
  csNameChangeLicenseeSociety: 0b000000100,
  csNameChangeLicenseePerson: 0b000000010,
  csTiedHouseDeclaration: 0b000000001
};

const hasNoLicences = (accountSummary: AccountSummary): boolean => {
  return accountSummary.licences.length === 0;
};

/**
 * Returns `true` if all licences are of type Liquor.
 *
 * @param {AccountSummary} accountSummary
 * @return {*}  {boolean}
 */
const hasOnlyLiquorLicences = (accountSummary: AccountSummary): boolean => {
  return accountSummary.licences.every((licence) => {
    return licence.licenceTypeCategory === LicenceTypeCategory.Liquor;
  });
};

/**
 * Returns `true` if all licences are of type Cannabis.
 *
 * @param {AccountSummary} accountSummary
 * @return {*}  {boolean}
 */
const hasOnlyCannabisLicences = (accountSummary: AccountSummary): boolean => {
  return accountSummary.licences.every((licence) => {
    return licence.licenceTypeCategory === LicenceTypeCategory.Cannabis;
  });
};

/**
 * Returns `true` if at least one liquor licence exists of the specified licence type names.
 *
 * @param {AccountSummary} accountSummary
 * @param {LiquorLicenceTypeNames[]} licenceTypeNames
 * @return {*}  {boolean}
 */
const hasSomeLiquorLicences = (accountSummary: AccountSummary, licenceTypeNames: LiquorLicenceTypeNames[]): boolean => {
  return licenceTypeNames.some((licenceTypeName) => {
    return accountSummary.licences.some((licence) => {
      return licence.licenceTypeCategory === LicenceTypeCategory.Liquor && licence.licenceType === licenceTypeName;
    });
  });
};

/**
 * Returns `true` if at least one liquor licence exists that is NOT of the specified licence type names.
 *
 * @param {AccountSummary} accountSummary
 * @param {LiquorLicenceTypeNames[]} licenceTypeNames
 * @return {*}  {boolean}
 */
const hasAtLeastOneOtherLiquorLicenceType = (
  accountSummary: AccountSummary,
  licenceTypeNames: LiquorLicenceTypeNames[]
): boolean => {
  return licenceTypeNames.some((licenceTypeName) =>
    accountSummary.licences.some((licence) => {
      return licence.licenceTypeCategory === LicenceTypeCategory.Liquor && licence.licenceType !== licenceTypeName;
    })
  );
};

/**
 * Given a bitmask, returns the names of the PCL sections that are represented by that bitmask.
 *
 * @param {number} bitmask
 * @return {*}  {PCLFormControlName[]}
 */
const getPCLSectionNamesFromBitmask = (bitmask: number): PCLFormControlName[] => {
  const entries = Object.entries(PCLSectionBitmasks) as [PCLFormControlName, number][];
  const matchedValues = entries.filter(([_, value]) => (bitmask & value) !== 0);
  return matchedValues.map(([item]) => item);
};

type PCLSectionNamesFromBitmaskOptions = {
  accountType: AccountType;
  conditionalGroup: PCLMatrixConditionalGroup;
  licenceGroup: PCLMatrixLicenceGroup;
};

/**
 * Given an array of PCLFormControlName, returns the corresponding PCLFormControlDefinitionOption objects.
 *
 * @param {PCLFormControlName[]} pclFormControlNames
 * @return {*}
 */
const getPCLFormControlDefinitions = (pclFormControlNames: PCLFormControlName[]) => {
  return PCLFormControlDefinitions.filter((option) => {
    return pclFormControlNames.includes(option.formControlName);
  });
};

/**
 * Return `true` if the `matchSections` are all included in the `selectedSections`.
 *
 * @example
 * ```typescript
 * hasAllMatchSections({
 *   matchSections: [PCLFormControlName.Section1, PCLFormControlName.Section2],
 *   selectedSections: [PCLFormControlName.Section1, PCLFormControlName.Section2, PCLFormControlName.Section3]
 * }); // true
 *
 * hasAllMatchSections({
 *   matchSections: [PCLFormControlName.Section1, PCLFormControlName.Section2],
 *   selectedSections: [PCLFormControlName.Section1, PCLFormControlName.Section3]
 * }); // false
 * ```
 *
 * @param {{
 *   matchSections: PCLFormControlName[];
 *   selectedSections: PCLFormControlName[];
 * }} options
 * @return {*}  {boolean}
 */
const hasAllMatchSections = (options: {
  matchSections: PCLFormControlName[];
  selectedSections: PCLFormControlName[];
}): boolean => {
  return options.matchSections.every((item) => options.selectedSections.includes(item));
};

type PCLMatrixConditionalGroupOptions = {
  selectedPCLSections?: PCLFormControlName[];
};

const _PCLSectionGroupedBusinessRules: Partial<
  Record<
    AccountType,
    Partial<Record<PCLMatrixConditionalGroup, Partial<Record<PCLMatrixLicenceGroup, () => number | null>>>>
  >
> = {
  [AccountType.Default]: {
    [PCLMatrixConditionalGroup.Default]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      }
    }
  },
  [AccountType.PrivateCorporation]: {
    [PCLMatrixConditionalGroup.Default]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b100110011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b110110011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b111110011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b111110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b111110011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b111110011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b111110011;
      }
    },
    [PCLMatrixConditionalGroup.ReceiverOrExecutor]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b100000000;
      }
    },
    [PCLMatrixConditionalGroup.TSE]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b010010011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b010110011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b010110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b010110011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b010110011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b010110011;
      }
    },
    [PCLMatrixConditionalGroup.TSI]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b001010011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b001110011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b001110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b001110011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b001110011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b001110011;
      }
    },
    [PCLMatrixConditionalGroup.CoDAndTSE]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b010110010;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b010110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b010110010;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b010110010;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b010110010;
      }
    },
    [PCLMatrixConditionalGroup.CoDAndTSI]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000110011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b011010011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b011110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b011110011;
      }
    },
    [PCLMatrixConditionalGroup.CoD]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000110011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b011110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b011110011;
      }
    },
    [PCLMatrixConditionalGroup.LENameChangeCorporation]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000110011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b011010011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b011110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b011110011;
      }
    },
    // Not applicable for Private Corporation
    [PCLMatrixConditionalGroup.LENameChangePartnership]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    // Not applicable for Private Corporation
    [PCLMatrixConditionalGroup.LENameChangeSociety]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    [PCLMatrixConditionalGroup.LENameChangeIndividual]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000110011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b011010011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b011110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b011110011;
      }
    },
    [PCLMatrixConditionalGroup.TiedHouseChange]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000110011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b011010011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b011110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b011110011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b011110011;
      }
    }
  },
  [AccountType.PublicCorporation]: {
    [PCLMatrixConditionalGroup.Default]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b100110010;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b100110010;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b100110010;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b100110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b100110010;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b100110010;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b100110010;
      }
    },
    [PCLMatrixConditionalGroup.ReceiverOrExecutor]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b100000000;
      }
    },
    // Not applicable for Public Corporation
    [PCLMatrixConditionalGroup.TSE]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    // Not applicable for Public Corporation
    [PCLMatrixConditionalGroup.TSI]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    [PCLMatrixConditionalGroup.CoD]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b000110010;
      }
    },
    // Not applicable for Public Corporation
    [PCLMatrixConditionalGroup.CoDAndTSE]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    // Not applicable for Public Corporation
    [PCLMatrixConditionalGroup.CoDAndTSI]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    [PCLMatrixConditionalGroup.LENameChangeCorporation]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b000110010;
      }
    },
    // Not applicable for Public Corporation
    [PCLMatrixConditionalGroup.LENameChangePartnership]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    // Not applicable for Public Corporation
    [PCLMatrixConditionalGroup.LENameChangeSociety]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    [PCLMatrixConditionalGroup.LENameChangeIndividual]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b000110010;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b000110010;
      }
    },
    // Not applicable for Public Corporation
    [PCLMatrixConditionalGroup.TiedHouseChange]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    }
  },
  [AccountType.Partnership]: {
    [PCLMatrixConditionalGroup.Default]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b100101011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b110101011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b111101011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b111101010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b111101011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b111101011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b111101011;
      }
    },
    [PCLMatrixConditionalGroup.ReceiverOrExecutor]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b100000000;
      }
    },
    [PCLMatrixConditionalGroup.TSE]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b010001011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b010101011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b010101010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b010101011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b010101011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b010101011;
      }
    },
    [PCLMatrixConditionalGroup.TSI]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b001001011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b001101011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b001101010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b001101011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b001101011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b001101011;
      }
    },
    [PCLMatrixConditionalGroup.CoD]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b001101010;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b001101010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b001101010;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b001101010;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b001101010;
      }
    },
    [PCLMatrixConditionalGroup.CoDAndTSE]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000101011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b011101010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b011101011;
      }
    },
    [PCLMatrixConditionalGroup.CoDAndTSI]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b010101010;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b010101010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b010101010;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b010101010;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b010101010;
      }
    },
    // Not applicable for Partnership
    [PCLMatrixConditionalGroup.LENameChangeCorporation]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    [PCLMatrixConditionalGroup.LENameChangePartnership]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000101011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b011001011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b011101010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b011101011;
      }
    },
    // Not applicable for Partnership
    [PCLMatrixConditionalGroup.LENameChangeSociety]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    [PCLMatrixConditionalGroup.LENameChangeIndividual]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000101011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b011001011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b011101010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b011101011;
      }
    },
    [PCLMatrixConditionalGroup.TiedHouseChange]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000101011;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b011001011;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b011101010;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b011101011;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b011101011;
      }
    }
  },
  [AccountType.Society]: {
    [PCLMatrixConditionalGroup.Default]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b100100110;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b100100110;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b100100110;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b100100110;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b100100110;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b100100110;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b100100110;
      }
    },
    [PCLMatrixConditionalGroup.ReceiverOrExecutor]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b100000000;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b100000000;
      }
    },
    // Not applicable for Society
    [PCLMatrixConditionalGroup.TSE]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    // Not applicable for Society
    [PCLMatrixConditionalGroup.TSI]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    [PCLMatrixConditionalGroup.CoD]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b000100110;
      }
    },
    // Not applicable for Society
    [PCLMatrixConditionalGroup.CoDAndTSE]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    // Not applicable for Society
    [PCLMatrixConditionalGroup.CoDAndTSI]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    // Not applicable for Society
    [PCLMatrixConditionalGroup.LENameChangeCorporation]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    // Not applicable for Society
    [PCLMatrixConditionalGroup.LENameChangePartnership]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    },
    [PCLMatrixConditionalGroup.LENameChangeSociety]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b000100110;
      }
    },
    [PCLMatrixConditionalGroup.LENameChangeIndividual]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return 0b000100110;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return 0b000100110;
      }
    },
    // Not applicable for Society
    [PCLMatrixConditionalGroup.TiedHouseChange]: {
      [PCLMatrixLicenceGroup.Default]: () => {
        return FULL_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Liquor3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis1]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis2]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis3]: () => {
        return EMPTY_BITMASK;
      },
      [PCLMatrixLicenceGroup.Cannabis4]: () => {
        return EMPTY_BITMASK;
      }
    }
  }
};

/**
 * This encodes all of the business rules for which PCL sections should be visible/editable, based on:
 * - The user's account type (e.g., Private Corporation)
 * - The selected PCL section (e.g., csChangeOfDirectorsOrOfficers)
 * - The licences the user has (e.g., Liquor Primary)
 *
 * Note: Some account types have the same rules, and so share the same values from _PCLSectionGroupedBusinessRules.
 */
const PCLSectionBusinessRules: Partial<
  Record<
    AccountType,
    Partial<Record<PCLMatrixConditionalGroup, Partial<Record<PCLMatrixLicenceGroup, () => number | null>>>>
  >
> = {
  // Default rules
  [AccountType.Default]: _PCLSectionGroupedBusinessRules[AccountType.Default],

  // Private Corporation rules
  [AccountType.PrivateCorporation]: _PCLSectionGroupedBusinessRules[AccountType.PrivateCorporation],

  // Public Corporation rules
  [AccountType.PublicCorporation]: _PCLSectionGroupedBusinessRules[AccountType.PublicCorporation],

  // Partnership rules
  [AccountType.Partnership]: _PCLSectionGroupedBusinessRules[AccountType.Partnership],
  [AccountType.GeneralPartnership]: _PCLSectionGroupedBusinessRules[AccountType.Partnership],
  [AccountType.LimitedLiabilityPartnership]: _PCLSectionGroupedBusinessRules[AccountType.Partnership],

  // Society rules
  [AccountType.Society]: _PCLSectionGroupedBusinessRules[AccountType.Society],
  [AccountType.IndigenousNation]: _PCLSectionGroupedBusinessRules[AccountType.Society],
  [AccountType.LocalGovernment]: _PCLSectionGroupedBusinessRules[AccountType.Society],
  [AccountType.Coop]: _PCLSectionGroupedBusinessRules[AccountType.Society],
  [AccountType.MilitaryMess]: _PCLSectionGroupedBusinessRules[AccountType.Society],
  [AccountType.University]: _PCLSectionGroupedBusinessRules[AccountType.Society],
  [AccountType.SoleProprietorship]: _PCLSectionGroupedBusinessRules[AccountType.Society]
};
