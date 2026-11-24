import { Injectable } from '@angular/core';
import { createPayment, processPayment } from './payment-service';
import { buildReceiptEmail, sendReceiptEmail } from './receipt-email';
import { injectReceipts, injectReceiptsReady, injectReceipt, injectReceiptForJob, injectReceiptEmails, injectTransactions, injectTransaction } from './payments.signals';
import type { PaymentsRepository } from './payments.repository';

/** Mock PaymentsRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockPaymentsRepository implements PaymentsRepository {
  readonly receipts = injectReceipts;

  readonly receiptsReady = injectReceiptsReady;

  readonly receipt = injectReceipt;

  readonly receiptForJob = injectReceiptForJob;

  readonly receiptEmails = injectReceiptEmails;

  readonly transactions = injectTransactions;

  readonly transaction = injectTransaction;

  readonly createPayment = createPayment;

  readonly processPayment = processPayment;

  readonly buildReceiptEmail = buildReceiptEmail;

  readonly sendReceiptEmail = sendReceiptEmail;
}
