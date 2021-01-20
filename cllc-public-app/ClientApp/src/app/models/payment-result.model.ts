export interface PaymentResult {
  isApproved: boolean;
  cardType: string;
  trnAmount: string;
  trnDate: string;
  trnId: string;
  invoice: string;
  paymentTransactionTitle: string;
  paymentTransactionMessage: string[];
}
