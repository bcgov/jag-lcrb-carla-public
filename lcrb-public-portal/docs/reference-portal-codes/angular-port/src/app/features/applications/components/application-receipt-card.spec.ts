import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { listReceipts } from '@data-access/repositories/payments/receipt-model';
import { ApplicationReceiptCard } from './application-receipt-card';

async function mount(jobNumber: string, settleTimes = 3) {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  const fixture = TestBed.createComponent(ApplicationReceiptCard);
  fixture.componentRef.setInput('jobNumber', jobNumber);
  const el = fixture.nativeElement as HTMLElement;
  const first = el.innerHTML;
  fixture.detectChanges();
  for (let i = 0; i < settleTimes; i++) {
    await fixture.whenStable();
    fixture.detectChanges();
  }
  return { el, first };
}

describe('ApplicationReceiptCard', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders nothing on the first render, then the receipt block for a job with a fee', async () => {
    const receipt = listReceipts()[0];
    const { el, first } = await mount(receipt.jobNumber);
    expect(first).not.toContain('Receipt');
    const root = el.firstElementChild!;
    expect(root.className).toBe('flex flex-col gap-3 border-t border-border pt-4');
    expect(el.textContent).toContain('Receipt');
    expect(el.textContent).toContain(receipt.receiptNumber);
    expect(el.querySelector('span.font-mono')!.className).toBe('font-mono font-medium text-foreground');
    expect(el.querySelector('button[aria-label^="Download PDF receipt"]')).toBeTruthy();
    const link = el.querySelector('a[href="/receipts"]')!;
    expect(link.textContent!.trim()).toBe('View all receipts');
    expect(link.className).toBe('text-xs font-medium text-[#38598A] underline-offset-4 hover:underline');
  });

  it('renders nothing when the application has no receipt', async () => {
    const { el } = await mount('NO-SUCH-JOB');
    expect(el.querySelector('div')).toBeNull();
  });
});
