import { InjectionToken } from '@angular/core';
import type { createPayment, processPayment } from './payment-service';
import type { buildReceiptEmail, sendReceiptEmail } from './receipt-email';
import type { injectReceipts, injectReceiptsReady, injectReceipt, injectReceiptForJob, injectReceiptEmails, injectTransactions, injectTransaction } from './payments.signals';
import { MockPaymentsRepository } from './payments.repository.mock';

/** Payments repository (backend area: 6.10 Payments and receipts). */
export interface PaymentsRepository {
  readonly receipts: typeof injectReceipts;
  readonly receiptsReady: typeof injectReceiptsReady;
  readonly receipt: typeof injectReceipt;
  readonly receiptForJob: typeof injectReceiptForJob;
  readonly receiptEmails: typeof injectReceiptEmails;
  readonly transactions: typeof injectTransactions;
  readonly transaction: typeof injectTransaction;
  readonly createPayment: typeof createPayment;
  readonly processPayment: typeof processPayment;
  readonly buildReceiptEmail: typeof buildReceiptEmail;
  readonly sendReceiptEmail: typeof sendReceiptEmail;
}

export const PAYMENTS_REPOSITORY = new InjectionToken<PaymentsRepository>('PaymentsRepository', {
  providedIn: 'root',
  factory: () => new MockPaymentsRepository(),
});
