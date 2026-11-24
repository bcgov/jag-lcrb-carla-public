/** Receipt block with PDF download and email status; renders nothing when the application had no fee. */
import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { ReceiptDownloadButton, ReceiptEmailStatus } from '@shared/ui/receipts/receipt-actions';
import { PAYMENTS_REPOSITORY } from '@data-access/repositories/payments/payments.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'application-receipt-card',
  imports: [LucideIcon, ReceiptDownloadButton, ReceiptEmailStatus, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (ready() && receipt(); as r) {
      <div class="flex flex-col gap-3 border-t border-border pt-4">
        <div class="flex flex-wrap items-center justify-between gap-3">
          <div class="flex items-center gap-2 text-sm">
            <svg lucide="ReceiptText" class="h-4 w-4 text-[#013366]" aria-hidden="true"></svg>
            <span class="text-muted-foreground">Receipt</span>
            <span class="font-mono font-medium text-foreground">{{ r.receiptNumber }}</span>
          </div>
          <receipt-download-button [receipt]="r" size="sm" />
        </div>
        <receipt-email-status [receipt]="r" />
        <a [routerLink]="path('account.receipts')" class="text-xs font-medium text-[#38598A] underline-offset-4 hover:underline">View all receipts</a>
      </div>
    }
  `,
})
export class ApplicationReceiptCard {
  protected readonly path = path;
  private readonly paymentsRepository = inject(PAYMENTS_REPOSITORY);
  readonly jobNumber = input.required<string>();

  protected readonly ready = this.paymentsRepository.receiptsReady();
  protected readonly receipt = this.paymentsRepository.receiptForJob(() => this.jobNumber());
}
