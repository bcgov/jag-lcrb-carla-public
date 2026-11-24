/** Payment confirmation and receipt: success state, printable breakdown, reference numbers and next steps. The AI assessment is staff-only and intentionally not shown. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from "@angular/core"
import { RouterLink } from "@angular/router"
import { DocumentManager } from "@shared/ui/documents/document-manager"
import { ReceiptDownloadButton, ReceiptEmailStatus } from "@shared/ui/receipts/receipt-actions"
import { Button } from "@shared/ui/primitives/button"
import { LucideIcon } from "@shared/ui/primitives/lucide-icon"
import { type SubmissionRecord } from "@data-access/repositories/applications/submissions"
import { APPLICATIONS_REPOSITORY } from "@data-access/repositories/applications/applications.repository"
import { PAYMENTS_REPOSITORY } from "@data-access/repositories/payments/payments.repository"
import { path } from "@/app/route-registry"

function formatCurrency(amount: number, currency: string) {
  return new Intl.NumberFormat("en-CA", { style: "currency", currency }).format(amount)
}

function formatDateTime(iso: string) {
  try {
    return new Date(iso).toLocaleString("en-CA", {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "numeric",
      minute: "2-digit",
    })
  } catch {
    return iso
  }
}

@Component({
  selector: "receipt-body",
  imports: [
    DocumentManager,
    LucideIcon,
    ReceiptDownloadButton,
    ReceiptEmailStatus,
    RouterLink,
    Button,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <div class="mx-auto max-w-3xl px-4 py-8 sm:py-12">
      <!-- Success banner -->
      <div class="flex flex-col items-center text-center">
        <div class="flex h-16 w-16 items-center justify-center rounded-full bg-[#2E8540]/15">
          <svg lucide="CheckCircle2" class="h-9 w-9 text-[#2E8540]" aria-hidden="true"></svg>
        </div>
        <h1 class="mt-4 text-pretty text-2xl font-bold text-foreground sm:text-3xl">
          {{ isPaid() ? "Payment successful" : "Application submitted" }}
        </h1>
        <p class="mt-2 max-w-xl text-pretty leading-relaxed text-muted-foreground">{{ intro() }}</p>
      </div>

      <!-- Receipt card -->
      <div class="mt-8 overflow-hidden rounded-xl border border-border bg-card shadow-sm">
        <!-- Card header with key references -->
        <div class="border-b border-border bg-muted/40 px-6 py-5">
          <div class="flex flex-wrap items-start justify-between gap-4">
            <div>
              <p class="text-xs font-medium uppercase tracking-wide text-muted-foreground">Confirmation number</p>
              <p class="mt-1 font-mono text-lg font-semibold text-foreground">{{ record().confirmationNumber }}</p>
            </div>
            <div class="text-right">
              <p class="text-xs font-medium uppercase tracking-wide text-muted-foreground">Application reference</p>
              <p class="mt-1 font-mono text-lg font-semibold text-foreground">{{ record().jobNumber }}</p>
            </div>
          </div>
        </div>

        <!-- What was paid for -->
        <div class="px-6 py-5">
          <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">What you paid for</h2>
          <dl class="mt-3 space-y-3 text-sm">
            <div class="flex items-start justify-between gap-4">
              <dt class="text-muted-foreground">Application type</dt>
              <dd class="text-right font-medium text-foreground">{{ record().title }}</dd>
            </div>
            <div class="flex items-start justify-between gap-4">
              <dt class="text-muted-foreground">Applicant / business</dt>
              <dd class="text-right font-medium text-foreground">{{ record().applicant }}</dd>
            </div>
            <div class="flex items-start justify-between gap-4">
              <dt class="text-muted-foreground">Date &amp; time</dt>
              <dd class="text-right font-medium text-foreground">{{ submittedAt() }}</dd>
            </div>
          </dl>
        </div>

        <!-- Fee breakdown -->
        @if (isPaid()) {
          <div class="border-t border-border px-6 py-5">
            <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Payment details</h2>
            <dl class="mt-3 space-y-2 text-sm">
              @for (item of record().lineItems; track $index) {
                <div class="flex items-start justify-between gap-4">
                  <dt class="text-muted-foreground">{{ item.label }}</dt>
                  <dd class="font-medium text-foreground">{{ money(item.amount) }}</dd>
                </div>
              }
              <div class="mt-2 flex items-center justify-between border-t border-border pt-3">
                <dt class="text-base font-semibold text-foreground">Total paid</dt>
                <dd class="text-base font-bold text-foreground">{{ money(record().total) }}</dd>
              </div>
              <div class="flex items-start justify-between gap-4 pt-1">
                <dt class="text-muted-foreground">Payment method</dt>
                <dd class="font-medium text-foreground">{{ record().paymentMethod }}</dd>
              </div>
              @if (record().receiptNumber) {
                <div class="flex items-start justify-between gap-4">
                  <dt class="text-muted-foreground">Receipt number</dt>
                  <dd class="font-mono font-medium text-foreground">{{ record().receiptNumber }}</dd>
                </div>
              }
            </dl>
            @if (receipt(); as r) {
              <div class="mt-5 flex flex-col gap-3 print:hidden">
                <div class="flex flex-wrap items-center justify-between gap-3">
                  <p class="flex items-center gap-2 text-sm text-muted-foreground">
                    <svg lucide="ReceiptText" class="h-4 w-4 text-[#013366]" aria-hidden="true"></svg>
                    Official PDF receipt
                  </p>
                  <receipt-download-button [receipt]="r" />
                </div>
                <receipt-email-status [receipt]="r" />
              </div>
            }
          </div>
        }

        <!-- Submitted details summary -->
        @if (record().summary.length > 0) {
          <details class="group border-t border-border px-6 py-5">
            <summary
              class="flex cursor-pointer list-none items-center justify-between text-sm font-semibold uppercase tracking-wide text-muted-foreground"
            >
              <span class="flex items-center gap-2">
                <svg lucide="FileText" class="h-4 w-4" aria-hidden="true"></svg>
                Submitted information
              </span>
              <span class="text-xs font-normal normal-case text-muted-foreground group-open:hidden">Show</span>
              <span class="hidden text-xs font-normal normal-case text-muted-foreground group-open:inline">Hide</span>
            </summary>
            <dl class="mt-4 divide-y divide-border">
              @for (row of record().summary; track $index) {
                <div class="grid grid-cols-1 gap-1 py-2 text-sm sm:grid-cols-3">
                  <dt class="text-muted-foreground sm:col-span-1">{{ row.label }}</dt>
                  <dd class="font-medium text-foreground sm:col-span-2">{{ row.value }}</dd>
                </div>
              }
            </dl>
          </details>
        }
      </div>

      <!-- Supporting documents -->
      <div class="mt-6 rounded-xl border border-border bg-card p-6 print:hidden">
        <h2 class="text-base font-semibold text-foreground">Supporting documents</h2>
        <p class="mt-1 text-sm leading-relaxed text-muted-foreground">
          Attach floor plans, leases, or any documents the branch has requested. Files are scanned before a reviewer can
          open them.
        </p>
        <div class="mt-4">
          <document-manager [subjectId]="record().jobNumber" [actor]="record().applicant" />
        </div>
      </div>

      <!-- What happens next -->
      <div class="mt-6 rounded-xl border border-border bg-card p-6">
        <h2 class="text-base font-semibold text-foreground">What happens next</h2>
        <ul class="mt-4 space-y-4">
          <li class="flex gap-3">
            <svg lucide="Mail" class="mt-0.5 h-5 w-5 shrink-0 text-[#013366]" aria-hidden="true"></svg>
            <div>
              <p class="text-sm font-medium text-foreground">Confirmation email</p>
              <p class="text-sm leading-relaxed text-muted-foreground">
                We've emailed your confirmation number{{ isPaid() ? " and official receipt (PDF attached)" : "" }} to&ngsp;@if (record().payerEmail) {<span class="font-mono">{{ record().payerEmail }}</span>} @else {your account email}. You can download any receipt again from&ngsp;<a [routerLink]="path('account.receipts')" class="font-medium text-[#38598A] underline-offset-4 hover:underline">Receipts</a>.
              </p>
            </div>
          </li>
          <li class="flex gap-3">
            <svg lucide="Clock" class="mt-0.5 h-5 w-5 shrink-0 text-[#013366]" aria-hidden="true"></svg>
            <div>
              <p class="text-sm font-medium text-foreground">Review by LCRB</p>
              <p class="text-sm leading-relaxed text-muted-foreground">
                A licensing analyst will review your application. You can track its status anytime under My Applications.
              </p>
            </div>
          </li>
        </ul>
      </div>

      <!-- Actions -->
      <div class="mt-6 flex flex-col gap-3 sm:flex-row sm:justify-center print:hidden">
        <a uiButton [routerLink]="path('applications.list')" class="bg-[#013366] hover:bg-[#38598A]">
          Go to My Applications
          <svg lucide="ArrowRight" class="ml-2 h-4 w-4" aria-hidden="true"></svg>
        </a>
        @if (receipt(); as r) {
          <receipt-download-button [receipt]="r" variant="outline" />
        }
        <button uiButton variant="outline" (click)="print()">
          <svg lucide="Printer" class="mr-2 h-4 w-4" aria-hidden="true"></svg>
          Print receipt
        </button>
      </div>
    </div>
  `,
})
export class ReceiptBody {
  protected readonly path = path;
  private readonly paymentsRepository = inject(PAYMENTS_REPOSITORY)
  readonly record = input.required<SubmissionRecord>()

  protected readonly isPaid = computed(() => this.record().total > 0)
  protected readonly receipt = this.paymentsRepository.receipt(() => this.record().receiptNumber)
  protected readonly submittedAt = computed(() => formatDateTime(this.record().submittedAt))
  protected readonly intro = computed(() => {
    const paid = this.isPaid()
    return (
      (paid
        ? "Thank you. Your payment has been processed and your application has been submitted to the Liquor and Cannabis Regulation Branch."
        : "Your application has been submitted to the Liquor and Cannabis Regulation Branch.") +
      " " +
      (paid
        ? "Your official receipt has been emailed to you with a PDF copy attached."
        : "A confirmation has been sent to your email.")
    )
  })

  protected money(amount: number): string {
    return formatCurrency(amount, this.record().currency)
  }

  protected print(): void {
    window.print()
  }
}

@Component({
  selector: "payment-receipt",
  imports: [ReceiptBody, RouterLink, Button],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    @if (record(); as r) {
      <receipt-body [record]="r" />
    } @else {
      <div class="mx-auto max-w-2xl px-4 py-16 text-center">
        <h1 class="text-xl font-semibold text-foreground">Confirmation not found</h1>
        <p class="mt-2 text-muted-foreground">
          We couldn't find this confirmation. It may have been submitted on another device.
        </p>
        <a uiButton [routerLink]="path('applications.list')" class="mt-6 bg-[#013366] hover:bg-[#38598A]">Go to My Applications</a>
      </div>
    }
  `,
})
export class PaymentReceipt {
  protected readonly path = path;
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY)
  readonly id = input.required<string>()
  protected readonly record = this.applicationsRepository.submission(() => this.id())
}
