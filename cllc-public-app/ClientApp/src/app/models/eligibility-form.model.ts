export class EligibilityForm {
  // question 1
  isConnectedToUnlicencedStore?: boolean;
  nameLocationUnlicencedRetailer?: string;
  isRetailerStillOperating?: boolean;
  dateOperationsCeased?: Date;

  // question 2
  isInvolvedIllegalDistribution?: boolean;
  nameLocationRetailer?: string;
  illegalDistributionInvolvementDetails?: string;
  isInvolvementContinuing?: boolean;
  dateInvolvementCeased?: Date;

  // "e-sig"
  isEligibilityCertified: boolean;
  eligibilitySignature?: string;
  dateSignedOrDismissed: Date;
}
