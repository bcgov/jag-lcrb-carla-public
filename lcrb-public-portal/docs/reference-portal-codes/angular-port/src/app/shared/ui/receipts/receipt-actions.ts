/** Receipt actions: download PDF, show where and when emailed, resend, preview. toast() has no outlet, as in the prototype, so toasts never show. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { injectSession } from '@core/session/session-provider';
import { Button } from '../primitives/button';
import { DIALOG } from '../primitives/dialog';
import { LucideIcon } from '../primitives/lucide-icon';
import { injectToast } from '../primitives/use-toast';
import { formatReceiptDate, receiptFilename } from '@domain/payments/receipt';
import { type Receipt } from '@domain/models/payments';
import { downloadReceiptPdf } from '@shared/util/pdf/receipt-pdf';
import { cn } from '@shared/util/utils';
import { PAYMENTS_REPOSITORY } from '@data-access/repositories/payments/payments.repository';

function formatBytes(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  return `${Math.max(1, Math.round(bytes / 1024))} KB`;
}

@Component({
  selector: 'receipt-download-button',
  imports: [Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <button
      uiButton
      type="button"
      [variant]="variant()"
      [size]="size()"
      [class]="buttonClass()"
      (click)="download()"
      [attr.aria-label]="label() + ' ' + receipt().receiptNumber"
    >
      <svg lucide="Download" class="mr-2 h-4 w-4" aria-hidden="true"></svg>{{ label() }}
    </button>
  `,
})
export class ReceiptDownloadButton {
  readonly receipt = input.required<Receipt>();
  readonly variant = input<'default' | 'outline' | 'ghost' | 'secondary'>('default');
  readonly size = input<'default' | 'sm'>('default');
  readonly className = input<string | undefined>(undefined);
  readonly label = input('Download PDF receipt');

  protected readonly buttonClass = computed(() =>
    cn(this.variant() === 'default' && 'bg-[#013366] hover:bg-[#38598A]', this.className()),
  );

  protected download(): void {
    downloadReceiptPdf(this.receipt());
  }
}

/** "Emailed to ... with the PDF attached" status with resend and preview; receipts that pre-date the session fall back to "Email me this receipt". */
@Component({
  selector: 'receipt-email-status',
  imports: [Button, DIALOG, LucideIcon, ReceiptDownloadButton],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div [class]="rootClass()">
      <div class="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
        <div class="flex gap-3">
          @if (latest(); as l) {
            <svg lucide="MailCheck" class="mt-0.5 h-5 w-5 shrink-0 text-[#2E8540]" aria-hidden="true"></svg>
          } @else {
            <svg lucide="Mail" class="mt-0.5 h-5 w-5 shrink-0 text-[#013366]" aria-hidden="true"></svg>
          }
          <div class="min-w-0">
            @if (latest(); as l) {
              <p class="text-sm font-medium text-foreground">
                Receipt emailed to <span class="font-mono">{{ l.to }}</span>
              </p>
              <p class="text-xs text-muted-foreground">
                Sent {{ formatDate(l.at) }}{{ emails().length > 1 ? ' · ' + emails().length + ' sends' : '' }}
              </p>
              @if (attachment(); as a) {
                <p
                  class="mt-2 inline-flex items-center gap-1.5 rounded-md border border-border bg-card px-2 py-1 text-xs text-foreground"
                >
                  <svg lucide="Paperclip" class="h-3.5 w-3.5 text-muted-foreground" aria-hidden="true"></svg>
                  <span class="font-mono">{{ a.filename }}</span>
                  <span class="text-muted-foreground">({{ bytes(a.sizeBytes) }})</span>
                </p>
              }
            } @else {
              <p class="text-sm font-medium text-foreground">Email a copy of this receipt</p>
              <p class="text-xs text-muted-foreground">
                @if (to()) {
                  We'll send the receipt text and the PDF to <span class="font-mono">{{ to() }}</span>.
                } @else {
                  Sign in to email this receipt to your account address.
                }
              </p>
            }
          </div>
        </div>
        <div class="flex shrink-0 flex-wrap gap-2">
          <button uiButton type="button" variant="outline" size="sm" (click)="previewOpen.set(true)">
            <svg lucide="Eye" class="mr-1.5 h-4 w-4" aria-hidden="true"></svg>
            View email
          </button>
          <button
            uiButton
            type="button"
            variant="outline"
            size="sm"
            [disabled]="!to() || sending()"
            (click)="send(!!latest())"
          >
            @if (latest()) {
              <svg lucide="RefreshCw" class="mr-1.5 h-4 w-4" aria-hidden="true"></svg>
            } @else {
              <svg lucide="Mail" class="mr-1.5 h-4 w-4" aria-hidden="true"></svg>
            }
            {{ latest() ? 'Resend' : 'Email receipt' }}
          </button>
        </div>
      </div>

      <ui-dialog [state]="previewOpen() ? 'open' : 'closed'" (stateChanged)="previewOpen.set($event === 'open')">
        <ui-dialog-content *uiDialogPortal class="max-w-2xl">
          <div uiDialogHeader>
            <h2 uiDialogTitle>{{ preview().subject }}</h2>
            <p uiDialogDescription>{{ dialogDescription() }}</p>
          </div>
          <div class="rounded-md border border-border bg-card">
            <div class="flex items-center gap-2 border-b border-border px-4 py-2 text-xs text-muted-foreground">
              <svg lucide="Paperclip" class="h-3.5 w-3.5" aria-hidden="true"></svg>
              <span class="font-mono text-foreground">{{ filename() }}</span>
              <span>· PDF attachment</span>
              <receipt-download-button
                [receipt]="receipt()"
                variant="ghost"
                size="sm"
                className="ml-auto h-7 px-2 text-xs"
                label="Download"
              />
            </div>
            <pre
              class="max-h-[50vh] overflow-auto whitespace-pre-wrap px-4 py-3 font-sans text-sm leading-relaxed text-foreground"
              >{{ preview().body }}</pre
            >
          </div>
        </ui-dialog-content>
      </ui-dialog>
    </div>
  `,
})
export class ReceiptEmailStatus {
  private readonly paymentsRepository = inject(PAYMENTS_REPOSITORY);
  readonly receipt = input.required<Receipt>();
  readonly className = input<string | undefined>(undefined);

  private readonly sessionProvider = injectSession();
  private readonly toastApi = injectToast();
  protected readonly emails = this.paymentsRepository.receiptEmails(() => this.receipt().receiptNumber);
  protected readonly latest = computed(() => this.emails()[0]);
  protected readonly previewOpen = signal(false);
  protected readonly sending = signal(false);

  protected readonly rootClass = computed(() =>
    cn('rounded-lg border border-border bg-muted/30 p-4', this.className()),
  );
  protected readonly to = computed(
    () => this.latest()?.to ?? this.receipt().payerEmail ?? this.sessionProvider.session()?.email ?? '',
  );
  protected readonly preview = computed(() =>
    this.paymentsRepository.buildReceiptEmail(this.receipt(), typeof window === 'undefined' ? '' : window.location.origin),
  );
  protected readonly attachment = computed(() => this.latest()?.attachments?.[0]);
  protected readonly filename = computed(() => receiptFilename(this.receipt()));
  protected readonly dialogDescription = computed(() => {
    const latest = this.latest();
    return latest
      ? `Sent to ${latest.to} on ${formatReceiptDate(latest.at)}`
      : `Will be sent to ${this.to() || 'your account email'}`;
  });

  protected readonly formatDate = formatReceiptDate;
  protected readonly bytes = formatBytes;

  protected send(resend: boolean): void {
    const to = this.to();
    if (!to) return;
    const session = this.sessionProvider.session();
    this.sending.set(true);
    try {
      this.paymentsRepository.sendReceiptEmail({
        receipt: this.receipt(),
        to,
        recipientRole: session?.role,
        actor: session ? `${session.firstName} ${session.lastName}` : 'Applicant',
        resend,
      });
      this.toastApi.toast({
        title: resend ? 'Receipt re-sent' : 'Receipt emailed',
        description: `${receiptFilename(this.receipt())} was sent to ${to}.`,
      });
    } finally {
      this.sending.set(false);
    }
  }
}
