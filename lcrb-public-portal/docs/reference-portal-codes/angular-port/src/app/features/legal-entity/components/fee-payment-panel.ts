/** Tied House fee and payment panel; liquor and cannabis streams are paid separately (mock payments). */
import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { Button } from '@shared/ui/primitives/button';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { formatCurrency } from '@domain/tied-house/tied-house';
import { type FeeBreakdown } from '@domain/models/tied-house';
import { cn } from '@shared/util/utils';

@Component({
  selector: 'div[feeStream]',
  imports: [Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'px-5 py-4' },
  template: `
    <div class="flex items-center justify-between">
      <h4 class="text-sm font-semibold text-foreground">{{ title() }}</h4>
      <span
        [class]="cn('rounded px-2 py-0.5 text-xs font-medium', accent() === 'cannabis' ? 'bg-emerald-500/15 text-emerald-700' : 'bg-[#013366]/10 text-[#013366]')"
        >{{ accent() === 'cannabis' ? 'Cannabis' : 'Liquor' }}</span
      >
    </div>

    <table class="mt-3 w-full text-sm">
      <thead>
        <tr class="text-left text-xs uppercase tracking-wide text-muted-foreground">
          <th class="pb-1 font-medium">Item</th>
          <th class="pb-1 text-center font-medium">No. Licences</th>
          <th class="pb-1 text-right font-medium">Cost</th>
        </tr>
      </thead>
      <tbody>
        @for (l of lines(); track l.declarationId) {
          <tr class="border-t border-border">
            <td class="py-2 pr-3 text-foreground">{{ l.label }}</td>
            <td class="py-2 text-center text-foreground">{{ l.licencesImpacted }}</td>
            <td class="py-2 text-right font-medium text-foreground">{{ formatCurrency(l.amount) }}</td>
          </tr>
        }
      </tbody>
      <tfoot>
        <tr class="border-t border-border">
          <td class="pt-2 font-semibold text-foreground" colSpan="2">Subtotal</td>
          <td class="pt-2 text-right font-semibold text-foreground">{{ formatCurrency(total()) }}</td>
        </tr>
      </tfoot>
    </table>

    <div class="mt-3 flex items-center justify-end">
      @if (receipt()) {
        <span class="flex items-center gap-1.5 text-sm font-medium text-[#2E8540]">
          <svg lucide="CheckCircle2" class="h-4 w-4"></svg> Paid · {{ receipt() }}
        </span>
      } @else {
        <button
          uiButton
          (click)="pay.emit()"
          [disabled]="disabled()"
          [class]="accent() === 'cannabis' ? 'bg-emerald-600 hover:bg-emerald-700' : 'bg-[#013366] hover:bg-[#013366]/90'"
        >
          Pay now · {{ formatCurrency(total()) }}
        </button>
      }
    </div>

    @if (!receipt()) {
      <p class="mt-2 flex items-center gap-1.5 text-xs text-muted-foreground">
        <svg lucide="Info" class="h-3.5 w-3.5"></svg> Payment is processed by the government payment provider.
      </p>
    }
  `,
})
export class FeeStream {
  readonly title = input.required<string>();
  readonly lines = input.required<FeeBreakdown['liquor']['lines']>();
  readonly total = input.required<number>();
  readonly receipt = input<string | undefined>(undefined);
  readonly pay = output<void>();
  readonly disabled = input<boolean | undefined>(undefined);
  readonly accent = input.required<'liquor' | 'cannabis'>();

  protected readonly cn = cn;
  protected readonly formatCurrency = formatCurrency;
}

@Component({
  selector: 'section[feePaymentPanel]',
  imports: [LucideIcon, FeeStream],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'rounded-xl border border-border bg-card shadow-sm', 'aria-labelledby': 'th-payment-heading' },
  template: `
    <div class="border-b border-border px-5 py-4">
      <h3 id="th-payment-heading" class="flex items-center gap-2 text-base font-semibold text-foreground">
        <svg lucide="CreditCard" class="h-5 w-5 text-[#013366]"></svg> Payment
      </h3>
      <p class="mt-1 text-sm text-muted-foreground">Liquor and cannabis fees are paid separately. Both must be paid before you can submit to the Branch.</p>
    </div>

    <div class="divide-y divide-border">
      @if (fees().liquor.total > 0) {
        <div
          feeStream
          [title]="'Liquor fees'"
          [lines]="fees().liquor.lines"
          [total]="fees().liquor.total"
          [receipt]="liquorReceipt()"
          (pay)="payLiquor.emit()"
          [disabled]="disabled()"
          [accent]="'liquor'"
        ></div>
      }
      @if (fees().cannabis.total > 0) {
        <div
          feeStream
          [title]="'Cannabis fees'"
          [lines]="fees().cannabis.lines"
          [total]="fees().cannabis.total"
          [receipt]="cannabisReceipt()"
          (pay)="payCannabis.emit()"
          [disabled]="disabled()"
          [accent]="'cannabis'"
        ></div>
      }
    </div>

    <div class="flex items-center justify-between gap-4 border-t border-border bg-muted/40 px-5 py-4">
      <span class="text-sm font-medium text-muted-foreground">Total fees</span>
      <span class="text-lg font-bold text-foreground">{{ formatCurrency(fees().grandTotal) }}</span>
    </div>
  `,
})
export class FeePaymentPanel {
  readonly fees = input.required<FeeBreakdown>();
  readonly liquorReceipt = input<string | undefined>(undefined);
  readonly cannabisReceipt = input<string | undefined>(undefined);
  readonly payLiquor = output<void>();
  readonly payCannabis = output<void>();
  readonly disabled = input<boolean | undefined>(undefined);

  protected readonly formatCurrency = formatCurrency;
}
