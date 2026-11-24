/** Tied House payment confirmation: success state, printable breakdown, references. The application loads after first render, so "Loading…" shows first. */
import { afterNextRender, ChangeDetectionStrategy, Component, computed, effect, inject, input, signal, untracked } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Button } from '@shared/ui/primitives/button';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { CHANGE_TYPE_LABEL, formatCurrency, formatDate } from '@domain/tied-house/tied-house';
import { type TiedHouseApplication } from '@domain/models/tied-house';
import { getApplication } from '../state/tied-house-store';
import { TIED_HOUSE_REPOSITORY } from '@data-access/repositories/tied-house/tied-house.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'tied-house-confirmation',
  imports: [RouterLink, Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (app() === undefined) {
      <div class="mx-auto max-w-2xl px-4 py-16 text-center text-muted-foreground">Loading…</div>
    } @else if (!app()) {
      <div class="mx-auto max-w-2xl px-4 py-16 text-center">
        <h1 class="text-xl font-semibold text-foreground">Confirmation not found</h1>
        <p class="mt-2 text-muted-foreground">We couldn't find this submission. It may have been made on another device.</p>
        <a uiButton [routerLink]="path('legal-entity.tied-house')" class="mt-6 bg-[#013366] hover:bg-[#38598A]">Back to Tied House</a>
      </div>
    } @else {
      <div class="mx-auto max-w-3xl px-4 py-8 sm:py-12">
        <!-- Success banner -->
        <div class="flex flex-col items-center text-center">
          <div class="flex h-16 w-16 items-center justify-center rounded-full bg-[#2E8540]/15">
            <svg lucide="CheckCircle2" class="h-9 w-9 text-[#2E8540]" aria-hidden="true"></svg>
          </div>
          <h1 class="mt-4 text-pretty text-2xl font-bold text-foreground sm:text-3xl">
            {{ fees()!.grandTotal > 0 ? 'Payment received' : 'Declarations submitted' }}
          </h1>
          <p class="mt-2 max-w-xl text-pretty leading-relaxed text-muted-foreground">
            Your tied house declarations have been submitted to the Liquor and Cannabis Regulation Branch{{
              fees()!.grandTotal > 0 ? ' and your payment has been received.' : '.'
            }}
            A copy has been sent to your email.
          </p>
        </div>

        <!-- Receipt card -->
        <div class="mt-8 overflow-hidden rounded-xl border border-border bg-card shadow-sm">
          <div class="border-b border-border bg-muted/40 px-6 py-5">
            <div class="flex flex-wrap items-start justify-between gap-4">
              <div>
                <p class="text-xs font-medium uppercase tracking-wide text-muted-foreground">Reference</p>
                <p class="mt-1 font-mono text-lg font-semibold text-foreground">{{ app()!.jobNumber }}</p>
              </div>
              <div class="text-right">
                <p class="text-xs font-medium uppercase tracking-wide text-muted-foreground">Submitted</p>
                <p class="mt-1 font-medium text-foreground">{{ app()!.submittedAt ? formatDate(app()!.submittedAt!) : '—' }}</p>
              </div>
            </div>
          </div>

          <!-- Declarations -->
          <div class="px-6 py-5">
            <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Declarations submitted</h2>
            <ul class="mt-3 space-y-2 text-sm">
              @for (d of saved(); track d.id) {
                <li class="flex items-start justify-between gap-4 border-b border-border pb-2">
                  <div>
                    <p class="font-medium text-foreground">{{ declarationName(d) || 'Declaration' }}</p>
                    <p class="text-xs text-muted-foreground">{{ changeTypeLabel[d.changeType] }} · {{ d.associatedEstablishmentName }} (#{{ d.associatedLicenceNumber }})</p>
                  </div>
                  <span
                    [class]="
                      d.domain === 'cannabis'
                        ? 'shrink-0 rounded bg-emerald-500/15 px-2 py-0.5 text-xs font-medium text-emerald-700'
                        : 'shrink-0 rounded bg-[#013366]/10 px-2 py-0.5 text-xs font-medium text-[#013366]'
                    "
                    >{{ d.domain === 'cannabis' ? 'Cannabis' : 'Liquor' }}</span
                  >
                </li>
              }
            </ul>
          </div>

          <!-- Payment details -->
          @if (fees()!.grandTotal > 0) {
            <div class="border-t border-border px-6 py-5">
              <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Payment details</h2>
              <dl class="mt-3 space-y-2 text-sm">
                @if (fees()!.liquor.total > 0) {
                  <div class="flex items-center justify-between gap-4">
                    <dt class="text-muted-foreground">Liquor fees {{ app()!.liquorReceipt ? '· ' + app()!.liquorReceipt : '' }}</dt>
                    <dd class="font-medium text-foreground">{{ formatCurrency(fees()!.liquor.total) }}</dd>
                  </div>
                }
                @if (fees()!.cannabis.total > 0) {
                  <div class="flex items-center justify-between gap-4">
                    <dt class="text-muted-foreground">Cannabis fees {{ app()!.cannabisReceipt ? '· ' + app()!.cannabisReceipt : '' }}</dt>
                    <dd class="font-medium text-foreground">{{ formatCurrency(fees()!.cannabis.total) }}</dd>
                  </div>
                }
                <div class="mt-2 flex items-center justify-between border-t border-border pt-3">
                  <dt class="text-base font-semibold text-foreground">Total paid</dt>
                  <dd class="text-base font-bold text-foreground">{{ formatCurrency(fees()!.grandTotal) }}</dd>
                </div>
                <div class="flex items-center justify-between gap-4 pt-1">
                  <dt class="text-muted-foreground">Payment method</dt>
                  <dd class="font-medium text-foreground">Government payment provider</dd>
                </div>
              </dl>
            </div>
          }
        </div>

        <!-- What happens next -->
        <div class="mt-6 rounded-xl border border-border bg-card p-6">
          <h2 class="text-base font-semibold text-foreground">What happens next</h2>
          <ul class="mt-4 space-y-4">
            <li class="flex gap-3">
              <svg lucide="Mail" class="mt-0.5 h-5 w-5 shrink-0 text-[#013366]" aria-hidden="true"></svg>
              <div>
                <p class="text-sm font-medium text-foreground">Confirmation email</p>
                <p class="text-sm leading-relaxed text-muted-foreground">We've emailed your receipt and reference number for your records.</p>
              </div>
            </li>
            <li class="flex gap-3">
              <svg lucide="Clock" class="mt-0.5 h-5 w-5 shrink-0 text-[#013366]" aria-hidden="true"></svg>
              <div>
                <p class="text-sm font-medium text-foreground">Review by the Branch</p>
                <p class="text-sm leading-relaxed text-muted-foreground">A licensing analyst will review your declarations. Any licence cap implications are assessed by staff.</p>
              </div>
            </li>
          </ul>
        </div>

        <!-- Actions -->
        <div class="mt-6 flex flex-col gap-3 sm:flex-row sm:justify-center print:hidden">
          <a uiButton [routerLink]="path('legal-entity.tied-house')" class="bg-[#013366] hover:bg-[#38598A]">
            Back to Tied House
            <svg lucide="ArrowRight" class="ml-2 h-4 w-4"></svg>
          </a>
          <button uiButton variant="outline" (click)="print()">
            <svg lucide="Printer" class="mr-2 h-4 w-4"></svg>
            Print receipt
          </button>
        </div>
      </div>
    }
  `,
})
export class TiedHouseConfirmation {
  protected readonly path = path;
  private readonly tiedHouseRepository = inject(TIED_HOUSE_REPOSITORY);
  readonly id = input.required<string>();

  protected readonly changeTypeLabel = CHANGE_TYPE_LABEL;
  protected readonly formatCurrency = formatCurrency;
  protected readonly formatDate = formatDate;

  /** undefined while loading, null when the id is unknown. */
  protected readonly app = signal<TiedHouseApplication | null | undefined>(undefined);
  protected readonly fees = computed(() => {
    const app = this.app();
    return app ? this.tiedHouseRepository.computeFees(app.declarations, app.jobNumber) : null;
  });
  protected readonly saved = computed(() => this.app()?.declarations.filter((d) => d.status === 'saved') ?? []);

  private mounted = false;

  constructor() {
    // The prototype's useEffect([id]): runs after the first render and whenever the id changes.
    afterNextRender(() => {
      this.mounted = true;
      this.app.set(getApplication(this.id()) ?? null);
    });
    effect(() => {
      const id = this.id();
      untracked(() => {
        if (this.mounted) this.app.set(getApplication(id) ?? null);
      });
    });
  }

  protected declarationName(d: TiedHouseApplication['declarations'][number]): string | undefined {
    return d.type === 'individual' ? [d.individual?.firstName, d.individual?.lastName].filter(Boolean).join(' ') : d.entity?.legalEntityName;
  }

  protected print(): void {
    window.print();
  }
}
