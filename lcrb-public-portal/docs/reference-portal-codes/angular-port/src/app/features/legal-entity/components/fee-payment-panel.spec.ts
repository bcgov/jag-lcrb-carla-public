import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { afterEach, describe, expect, it } from 'vitest';
import { formatCurrency } from '@domain/tied-house/tied-house';
import { type FeeBreakdown } from '@domain/models/tied-house';
import { AssociatedLicenceCombo } from './associated-licence-combo';
import { FeePaymentPanel } from './fee-payment-panel';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

const line = (declarationId: string, amount: number, domain: 'liquor' | 'cannabis') => ({
  declarationId,
  label: `Add tied house declaration (individual) — ${declarationId}`,
  licencesImpacted: 3,
  rate: 220,
  amount,
  domain,
});
const FEES: FeeBreakdown = {
  liquor: { lines: [line('d1', 660, 'liquor')], total: 660 },
  cannabis: { lines: [line('d2', 220, 'cannabis')], total: 220 },
  grandTotal: 880,
};

@Component({
  imports: [FeePaymentPanel],
  template: `<section
    feePaymentPanel
    [fees]="fees()"
    [liquorReceipt]="liquorReceipt()"
    [cannabisReceipt]="cannabisReceipt()"
    [disabled]="disabled()"
    (payLiquor)="paid.push('liquor')"
    (payCannabis)="paid.push('cannabis')"
  ></section>`,
})
class PanelHost {
  fees = signal(FEES);
  liquorReceipt = signal<string | undefined>(undefined);
  cannabisReceipt = signal<string | undefined>(undefined);
  disabled = signal(false);
  paid: string[] = [];
}

@Component({
  imports: [AssociatedLicenceCombo],
  template: `<div associatedLicenceCombo [id]="'th-assoc'" [value]="value()" (changed)="value.set($event?.number ?? '')"></div><p id="outside">outside</p>`,
})
class ComboHost {
  value = signal('');
}

async function mount<T>(cmp: new () => T) {
  const fixture = TestBed.createComponent(cmp);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, host: fixture.componentInstance, el: fixture.nativeElement as HTMLElement, next: () => settle(fixture) };
}

afterEach(() => (document.body.innerHTML = ''));

describe('FeePaymentPanel', () => {
  it('itemises the liquor and cannabis streams and pays them separately', async () => {
    const { host, el, next } = await mount(PanelHost);
    const panel = el.querySelector('section')!;
    expect(panel.getAttribute('aria-labelledby')).toBe('th-payment-heading');
    expect(panel.textContent).toContain('Liquor and cannabis fees are paid separately.');
    expect(panel.textContent).toContain('Liquor fees');
    expect(panel.textContent).toContain('Cannabis fees');
    expect(panel.querySelectorAll('tbody tr').length).toBe(2);
    expect(panel.textContent).toContain(`Subtotal${formatCurrency(660)}`);
    expect(panel.textContent).toContain(formatCurrency(880));
    const pay = [...panel.querySelectorAll('button')];
    expect(pay.map((b) => b.textContent!.trim())).toEqual([`Pay now · ${formatCurrency(660)}`, `Pay now · ${formatCurrency(220)}`]);
    expect(pay[1].className).toContain('bg-emerald-600');
    pay[0].click();
    pay[1].click();
    expect(host.paid).toEqual(['liquor', 'cannabis']);

    host.liquorReceipt.set('RCPT-1');
    await next();
    expect(panel.textContent).toContain('Paid · RCPT-1');
    expect(panel.querySelectorAll('button').length).toBe(1);
    host.disabled.set(true);
    await next();
    expect(panel.querySelector('button')!.disabled).toBe(true);
  });

  it('omits a stream with no fee', async () => {
    const { host, el, next } = await mount(PanelHost);
    host.fees.set({ ...FEES, cannabis: { lines: [], total: 0 }, grandTotal: 660 });
    await next();
    expect(el.textContent).toContain('Liquor fees');
    expect(el.textContent).not.toContain('Cannabis fees');
  });
});

describe('AssociatedLicenceCombo', () => {
  it('searches, selects a licence, shows the summary and clears it', async () => {
    const { host, el, next } = await mount(ComboHost);
    const combo = el.querySelector('[associatedLicenceCombo]')!;
    expect(combo.className).toBe('relative');
    expect(combo.hasAttribute('id')).toBe(false);
    const input = combo.querySelector('#th-assoc') as HTMLInputElement;
    expect(input.placeholder).toContain('Search by licence number (6 digits) or establishment name');
    input.dispatchEvent(new Event('focus'));
    await next();
    expect(combo.querySelectorAll('button').length).toBeGreaterThan(3);
    input.value = 'zzzz';
    input.dispatchEvent(new Event('input'));
    await next();
    expect(combo.textContent).toContain('No matching liquor or cannabis licence found.');
    input.value = '608114';
    input.dispatchEvent(new Event('input'));
    await next();
    const result = [...combo.querySelectorAll('button')].find((b) => b.textContent!.includes('Harbour Cannabis Co.'))!;
    result.click();
    await next();
    expect(host.value()).toBe('608114');
    expect(combo.className).toContain('bg-muted/40');
    expect(combo.textContent).toContain('#608114 · Cannabis Retail Store · Harbourside Hospitality Group Ltd.');
    expect(combo.textContent).toContain('Cannabis');
    (combo.querySelector('button[aria-label="Clear selected licence"]') as HTMLElement).click();
    await next();
    expect(host.value()).toBe('');
    expect(combo.querySelector('#th-assoc')).not.toBeNull();
  });

  it('closes the results when the user clicks outside', async () => {
    const { el, next } = await mount(ComboHost);
    const combo = el.querySelector('[associatedLicenceCombo]')!;
    (combo.querySelector('#th-assoc') as HTMLInputElement).dispatchEvent(new Event('focus'));
    await next();
    expect(combo.querySelector('.absolute.z-20')).not.toBeNull();
    el.querySelector('#outside')!.dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));
    await next();
    expect(combo.querySelector('.absolute.z-20')).toBeNull();
  });
});

