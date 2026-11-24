import { computed, type Signal } from "@angular/core"
import { injectExternalStore, injectHydrated, read, type Arg } from "@shared/util/external-store"
import { transactionCollection, listTransactions, getTransaction, type Transaction } from "./payment-service"
import { injectCollectionVersion } from "../mock-storage/collection-version"
import { notificationDeliveryLog, listDeliveriesForSubject, type DeliveryLogEntry } from "../notifications/notification-service"
import { getReceipt, getReceiptForJob, listReceipts } from "./receipt-model"
import { type Receipt } from "@domain/models/payments"

export function injectTransactions(): Signal<Transaction[]> {
  const version = injectCollectionVersion(transactionCollection)
  return computed(() => (version(), listTransactions()))
}

export function injectTransaction(id: Arg<string | undefined>): Signal<Transaction | undefined> {
  const version = injectCollectionVersion(transactionCollection)
  return computed(() => {
    version()
    const i = read(id)
    return i ? getTransaction(i) : undefined
  })
}

const SUBMISSIONS_KEY = "lcrb.app.submissions"

/** Receipts also read the submissions store (confirmation numbers), so re-render when it changes. */
function injectSubmissionsVersion() {
  return injectExternalStore(
    (cb) => {
      const onStorage = (e: StorageEvent) => {
        if (e.key === SUBMISSIONS_KEY) cb()
      }
      window.addEventListener("storage", onStorage)
      return () => window.removeEventListener("storage", onStorage)
    },
    () => (typeof window === "undefined" ? "" : localStorage.getItem(SUBMISSIONS_KEY) ?? ""),
  )
}

/** Returns false during SSR / before hydration so receipt lists don't flash the seeded set. */
export function injectReceiptsReady(): Signal<boolean> {
  return injectHydrated()
}

export function injectReceipts(): Signal<Receipt[]> {
  const transactions = injectCollectionVersion(transactionCollection)
  const submissions = injectSubmissionsVersion()
  return computed(() => (transactions(), submissions(), listReceipts()))
}

export function injectReceipt(receiptNumber: Arg<string | undefined>): Signal<Receipt | undefined> {
  const transactions = injectCollectionVersion(transactionCollection)
  const submissions = injectSubmissionsVersion()
  return computed(() => {
    transactions()
    submissions()
    const n = read(receiptNumber)
    return n ? getReceipt(n) : undefined
  })
}

export function injectReceiptForJob(jobNumber: Arg<string | undefined>): Signal<Receipt | undefined> {
  const transactions = injectCollectionVersion(transactionCollection)
  const submissions = injectSubmissionsVersion()
  return computed(() => {
    transactions()
    submissions()
    const j = read(jobNumber)
    return j ? getReceiptForJob(j) : undefined
  })
}

/** Emails sent about one receipt, newest first. */
export function injectReceiptEmails(receiptNumber: Arg<string | undefined>): Signal<DeliveryLogEntry[]> {
  const version = injectCollectionVersion(notificationDeliveryLog)
  return computed(() => {
    version()
    const n = read(receiptNumber)
    return n ? listDeliveriesForSubject(n).filter((d) => d.channel === "email") : []
  })
}
