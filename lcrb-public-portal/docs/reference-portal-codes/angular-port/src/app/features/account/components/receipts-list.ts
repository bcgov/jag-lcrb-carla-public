/** `?receipt=<number>` highlights a receipt and scrolls it into view when the param or `ready` changes. */
import {
  afterNextRender,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  computed,
  effect,
  inject,
  Injector,
  signal,
  untracked,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Badge } from '@shared/ui/primitives/badge';
import { Card, CardContent } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { formatReceiptCurrency, formatReceiptDate } from '@domain/payments/receipt';
import { type Receipt } from '@domain/models/payments';
import { ReceiptDownloadButton, ReceiptEmailStatus } from '@shared/ui/receipts/receipt-actions';
import { PAYMENTS_REPOSITORY } from '@data-access/repositories/payments/payments.repository';

const STATUS: Record<Receipt['status'], { label: string; className: string }> = {
  paid: { label: 'Paid', className: 'bg-[#2E8540]/10 text-[#1F5E2C] border-transparent' },
  refunded: { label: 'Refunded', className: 'bg-muted text-muted-foreground border-transparent' },
  'partially-refunded': { label: 'Partially refunded', className: 'bg-[#FCBA19]/20 text-[#6B4E00] border-transparent' },
};

@Component({
  selector: 'receipts-list',
  imports: [Badge, Card, CardContent, LucideIcon, ReceiptDownloadButton, ReceiptEmailStatus, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (ready()) {
      @if (receipts().length === 0) {
        <div uiCard>
          <div uiCardContent class="flex flex-col items-center gap-3 py-16 text-center">
            <svg lucide="ReceiptText" class="h-10 w-10 text-muted-foreground" aria-hidden="true"></svg>
            <p class="text-base font-medium text-foreground">No receipts yet</p>
            <p class="max-w-sm text-sm text-muted-foreground">
              When you pay an application or permit fee, the official receipt appears here and is emailed to you.
            </p>
          </div>
        </div>
      } @else {
        <div class="flex flex-col gap-4">
          <p class="text-sm text-muted-foreground">
            {{ receipts().length }} {{ receipts().length === 1 ? 'receipt' : 'receipts' }} ·
            {{ currency(totalPaid()) }} paid
          </p>
          <ul class="flex flex-col gap-4">
            @for (r of receipts(); track r.receiptNumber) {
              @let status = statusOf[r.status];
              @let isHighlighted = r.receiptNumber === highlighted();
              <li>
                <div uiCard [class]="isHighlighted ? 'ring-2 ring-[#FCBA19]' : ''">
                  <div uiCardContent class="flex flex-col gap-4 p-5">
                    <div class="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                      <div class="min-w-0">
                        <div class="flex flex-wrap items-center gap-2">
                          <h2 class="text-base font-semibold text-foreground">{{ r.title }}</h2>
                          <span uiBadge variant="outline" [class]="status.className">
                            {{ status.label }}
                          </span>
                        </div>
                        <dl class="mt-1 flex flex-wrap gap-x-4 gap-y-1 text-xs text-muted-foreground">
                          <div class="flex gap-1">
                            <dt>Receipt</dt>
                            <dd class="font-mono text-foreground">{{ r.receiptNumber }}</dd>
                          </div>
                          <div class="flex gap-1">
                            <dt>Reference</dt>
                            <dd class="font-mono text-foreground">{{ r.jobNumber }}</dd>
                          </div>
                          <div class="flex gap-1">
                            <dt>Paid</dt>
                            <dd>{{ date(r.paidAt) }}</dd>
                          </div>
                          <div class="flex gap-1">
                            <dt>Method</dt>
                            <dd>{{ r.method }}</dd>
                          </div>
                        </dl>
                      </div>
                      <div class="flex items-center gap-3 sm:flex-col sm:items-end">
                        <p class="text-xl font-bold text-foreground">{{ currency(r.total, r.currency) }}</p>
                        <div class="flex gap-2">
                          @if (r.applicationHref) {
                            <a
                              [routerLink]="tree(r.applicationHref)"
                              class="inline-flex items-center gap-1 text-sm font-medium text-[#38598A] underline-offset-4 hover:underline"
                            >
                              Application
                              <svg lucide="ArrowUpRight" class="h-3.5 w-3.5" aria-hidden="true"></svg>
                            </a>
                          }
                          <receipt-download-button [receipt]="r" size="sm" label="Download PDF" />
                        </div>
                      </div>
                    </div>
                    <receipt-email-status [receipt]="r" />
                  </div>
                </div>
              </li>
            }
          </ul>
        </div>
      }
    }
  `,
})
export class ReceiptsList {
  private readonly paymentsRepository = inject(PAYMENTS_REPOSITORY);
  protected readonly ready = this.paymentsRepository.receiptsReady();
  protected readonly receipts = this.paymentsRepository.receipts();
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly injector = inject(Injector);
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef).nativeElement;
  private readonly params = toSignal(this.route.queryParamMap, { initialValue: this.route.snapshot.queryParamMap });
  protected readonly highlighted = computed(() => this.params().get('receipt'));
  protected readonly statusOf = STATUS;
  protected readonly currency = formatReceiptCurrency;
  protected readonly date = formatReceiptDate;
  protected readonly totalPaid = computed(() => this.receipts().reduce((a, r) => a + r.total - r.refundedAmount, 0));
  private readonly mounted = signal(false);

  constructor() {
    afterNextRender(() => this.mounted.set(true));
    // Scroll the highlighted card into view when the param or `ready` changes.
    effect(() => {
      if (!this.mounted()) return;
      const highlighted = this.highlighted();
      this.ready();
      untracked(() => {
        // Run once the new state is rendered, like React's post-commit effect.
        afterNextRender(
          () => {
            if (!highlighted) return;
            // The ref sits on the highlighted <li>; with duplicate numbers React keeps the last one.
            const list = this.receipts();
            let index = -1;
            list.forEach((r, i) => {
              if (r.receiptNumber === highlighted) index = i;
            });
            const li = index >= 0 ? this.host.querySelectorAll('ul > li')[index] : undefined;
            li?.scrollIntoView({ block: 'center', behavior: 'smooth' });
          },
          { injector: this.injector },
        );
      });
    });
  }

  /** Parsed so a `?query` in the href renders as-is (a string routerLink would encode the `?`). */
  protected tree(href: string) {
    return this.router.parseUrl(href);
  }
}
